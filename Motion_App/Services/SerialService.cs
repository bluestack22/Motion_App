using System;
using System.IO.Ports;

namespace Motion_App.Service
{
    public class SerialService
    {
        private static readonly Lazy<SerialService> _instance = new(() => new SerialService());
        public static SerialService Instance => _instance.Value;

        private SerialPort? _port;

        public bool IsOpen => _port?.IsOpen ?? false;

        public event Action<string>? DataReceived;

        private SerialService() { }
        public static string[] ScanSerialPort() => SerialPort.GetPortNames();  
        public bool Connect(string portName, int baudRate, int dataBits, string parityString)
        {
            try
            {
                var parity = parityString?.ToLower() switch
                {
                    "even" => Parity.Even,
                    "odd" => Parity.Odd,
                    _ => Parity.None,
                };

                _port = new SerialPort(portName, baudRate, parity, dataBits);
                _port.DataReceived += OnDataReceived;
                _port.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            if (_port == null)
                return;

            try
            {
                _port.DataReceived -= OnDataReceived;
                if (_port.IsOpen)
                    _port.Close();
                _port.Dispose();
            }
            catch { }
            finally { _port = null; }
        }

        private void OnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = _port?.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                    DataReceived?.Invoke(data);
            }
            catch { }
        }
    }
}
