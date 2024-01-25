using System;
using System.IO.Ports;

namespace FanControl.SerialComSensor
{
    public class COMReader : IDisposable
    {
        private SerialPort port;

        public string Data { get; private set; }

        public COMReader(string com)
        {
            InitializeSerialPort(com);
        }

        private void InitializeSerialPort(string com)
        {
            port = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
            port.DtrEnable = true;
            port.DataReceived += new SerialDataReceivedEventHandler(dataReceived);
            port.Open();
            Data = States.PORT_OPENED.ToString();
        }

        public void Dispose()
        {
            CloseSerialPort();
        }

        private void CloseSerialPort()
        {
            if (port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                port.Dispose();
            }
        }

        public void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Data = port.ReadLine()?.Trim(); // Updated to handle JSON data
        }
    }
}
