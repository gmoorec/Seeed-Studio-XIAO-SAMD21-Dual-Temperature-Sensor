public void InitializeAndLoad(IPluginSensorsContainer _container)
        {
            // Check if reader is disposed or null
            if (reader == null || reader.IsDisposed)
            {
                string com = Environment.GetEnvironmentVariable("SENSOR_COM_PORT");
                if (com == null)
                {
                    throw new Exception("SENSOR_COM_PORT variable is not set!");
                }

                reader = new COMReader(com);
            }

            // Check if temp1Sensor is disposed or null
            if (temp1Sensor == null || temp1Sensor.IsDisposed)
            {
                temp1Sensor = new TempSensor(reader, "temp1");
            }

            // Check if temp2Sensor is disposed or null
            if (temp2Sensor == null || temp2Sensor.IsDisposed)
            {
                temp2Sensor = new TempSensor(reader, "temp2");
            }

            // Add sensors to the container
            _container.TempSensors.Add(temp1Sensor);
            _container.TempSensors.Add(temp2Sensor);
        }

        public void Close()
        {
            DisposeObjects();
        }

        private void DisposeObjects()
        {
            reader?.Dispose();
            temp1Sensor?.Dispose();
            temp2Sensor?.Dispose();
        }

        public string Name => "FanControlPlugin.SerialCOMSensor";
    }
}
