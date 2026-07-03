using System;
using System.IO.Ports;
using System.Diagnostics;
using System.Globalization;
using FanControl.Plugins;

namespace FanControl.CustomLoop
{
    public class ArduinoLoopPlugin : IPlugin, IDisposable
    {
        private SerialPort _port;
        private string _latestData;
        private TempSensor _temp1Sensor;
        private TempSensor _temp2Sensor;

        private PerformanceCounter _pcTemp1;
        private PerformanceCounter _pcTemp2;

        // C# 5.0 compatible property getter
        public string Name
        {
            get { return "Arduino Loop"; }
        }

        public void Initialize()
        {
            string com = Environment.GetEnvironmentVariable("SENSOR_COM_PORT");
            if (string.IsNullOrEmpty(com))
            {
                throw new Exception("SENSOR_COM_PORT environment variable is not set!");
            }

            // The ID stays temp1/temp2 to match your Arduino's JSON output, 
            // but the Display Name becomes much cleaner for your FanControl cards.
            _temp1Sensor = new TempSensor("temp1", "Ambient");
            _temp2Sensor = new TempSensor("temp2", "Coolant");

            // Connect to the Windows Performance Counters for RTSS
            try
            {
                _pcTemp1 = new PerformanceCounter("ArduinoSensors", "Temp1", false);
                _pcTemp2 = new PerformanceCounter("ArduinoSensors", "Temp2", false);
            }
            catch { /* Silently ignore if RTSS counters aren't created yet */ }

            InitializeSerialPort(com);
        }

        private void InitializeSerialPort(string com)
        {
            _port = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
            _port.DtrEnable = true;
            _port.DataReceived += DataReceived;
            _port.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_port != null && _port.IsOpen)
            {
                try
                {
                    // C# 5.0 compatible null check
                    string line = _port.ReadLine();
                    if (line != null)
                    {
                        _latestData = line.Trim();
                        ParseAndUpdateData();
                    }
                }
                catch { /* Catch serial read overlaps */ }
            }
        }

        private void ParseAndUpdateData()
        {
            if (string.IsNullOrEmpty(_latestData)) return;

            // Ultra-lightweight JSON parse bypassing heavy dependencies
            string cleanData = _latestData.Replace("{", "").Replace("}", "").Replace("\"", "");
            string[] parts = cleanData.Split(',');

            float temp1 = float.NaN;
            float temp2 = float.NaN;

            foreach (string part in parts)
            {
                string[] kvp = part.Split(':');
                if (kvp.Length == 2)
                {
                    // C# 5.0 requires out variables to be declared beforehand
                    float parsedVal;
                    if (kvp[0] == "temp1" && float.TryParse(kvp[1], NumberStyles.Float, CultureInfo.InvariantCulture, out parsedVal))
                        temp1 = parsedVal;
                    if (kvp[0] == "temp2" && float.TryParse(kvp[1], NumberStyles.Float, CultureInfo.InvariantCulture, out parsedVal))
                        temp2 = parsedVal;
                }
            }

            if (!float.IsNaN(temp1))
            {
                _temp1Sensor.SetValue(temp1);
                if (_pcTemp1 != null) { try { _pcTemp1.RawValue = (long)(temp1 * 100); } catch { } }
            }

            if (!float.IsNaN(temp2))
            {
                _temp2Sensor.SetValue(temp2);
                if (_pcTemp2 != null) { try { _pcTemp2.RawValue = (long)(temp2 * 100); } catch { } }
            }
        }

        public void Load(IPluginSensorsContainer container)
        {
            container.TempSensors.Add(_temp1Sensor);
            container.TempSensors.Add(_temp2Sensor);
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_port != null)
            {
                if (_port.IsOpen) _port.Close();
                _port.Dispose();
            }
            if (_pcTemp1 != null) _pcTemp1.Dispose();
            if (_pcTemp2 != null) _pcTemp2.Dispose();
        }
    }

    public class TempSensor : IPluginSensor
    {
        // C# 5.0 requires a private setter for auto-properties
        public string Id { get; private set; }
        public string Name { get; private set; }
        public float? Value { get; private set; }

        public TempSensor(string id, string displayName)
        {
            Id = id;
            Name = displayName; 
        }

        public void SetValue(float val)
        {
            Value = val;
        }

        public void Update() { }
    }
}
