using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Motion_App.Service;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Motion_App.ViewModels
{
    internal partial class SettingModel : ObservableObject
    {
        // =====================================================
        // SERIAL PARAMETERS
        // =====================================================

        [ObservableProperty]
        private ObservableCollection<SelectionItem<string>> _ports = new();

        [ObservableProperty]
        private SelectionItem<string>? _selectedPort;

        // -----------------------------------------------------

        public ObservableCollection<SelectionItem<int>> BaudRates { get; }
            = new()
        {
            new() { Value = 9600, Display = "9600" },
            new() { Value = 19200, Display = "19200" },
            new() { Value = 38400, Display = "38400" },
            new() { Value = 57600, Display = "57600" },
            new() { Value = 115200, Display = "115200" }
        };

        [ObservableProperty]
        private SelectionItem<int>? _selectedBaudRate;

        // -----------------------------------------------------

        public ObservableCollection<SelectionItem<int>> DataBits { get; }
            = new()
        {
            new() { Value = 7, Display = "7" },
            new() { Value = 8, Display = "8" }
        };

        [ObservableProperty]
        private SelectionItem<int>? _selectedDataBits;

        // -----------------------------------------------------

        public ObservableCollection<SelectionItem<string>> Parities { get; }
            = new()
        {
            new() { Value = "None", Display = "None" },
            new() { Value = "Even", Display = "Even" },
            new() { Value = "Odd", Display = "Odd" }
        };

        [ObservableProperty]
        private SelectionItem<string>? _selectedParity;

        // =====================================================
        // CAMERA PARAMETERS
        // =====================================================

        [ObservableProperty]
        private ObservableCollection<SelectionItem<int>> _cameraList = new();

        [ObservableProperty]
        private SelectionItem<int>? _selectedCamera;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public SettingModel()
        {
            // Default selections
            SelectedBaudRate = BaudRates[4];
            SelectedDataBits = DataBits[1];
            SelectedParity = Parities[0];

            // Auto scan on initialization
            // _ = ScanCameras();
            // _ = ScanPorts();

            // Subscribe to hardware changes (Plug & Play)
            HardwareWatcherService.Instance.HardwareChanged += OnHardwareChanged;
        }

        private void OnHardwareChanged()
        {
            // Trigger scans when hardware changes are detected
            _ = ScanCameras();
            _ = ScanPorts();
        }

        // =====================================================
        // SCAN CAMERA
        // =====================================================

        [RelayCommand]
        private async Task ScanCameras()
        {
            var cameras = await Task.Run(() => WebCamService.Instance.ScanCameras());

            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentSelection = SelectedCamera?.Value;
                
                CameraList.Clear();
                foreach (var cam in cameras)
                {
                    CameraList.Add(cam);
                }

                // Restore selection if it still exists
                if (currentSelection != null)
                {
                    var found = CameraList.FirstOrDefault(c => c.Value == currentSelection);
                    if (found != null)
                    {
                        SelectedCamera = found;
                        return;
                    }
                }

                // Fallback to first if nothing selected or current gone
                if (CameraList.Count > 0)
                {
                    SelectedCamera = CameraList[0];
                }
                else
                {
                    SelectedCamera = null;
                }
            });
        }

        // =====================================================
        // SCAN PORTS
        // =====================================================

        [RelayCommand]
        private async Task ScanPorts()
        {
            var portNames = await Task.Run(() => SerialService.ScanSerialPort());

            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentSelection = SelectedPort?.Value;

                Ports.Clear();
                foreach (var name in portNames)
                {
                    Ports.Add(new SelectionItem<string> { Value = name, Display = name });
                }

                // Restore selection if it still exists
                if (currentSelection != null)
                {
                    var found = Ports.FirstOrDefault(p => p.Value == currentSelection);
                    if (found != null)
                    {
                        SelectedPort = found;
                        return;
                    }
                }

                // Fallback
                if (Ports.Count > 0)
                {
                    SelectedPort = Ports[0];
                }
                else
                {
                    SelectedPort = null;
                }
            });
        }
        // =====================================================
        // APPLY CAMERA SOURCE
        // =====================================================
        [RelayCommand]
        private async Task ApplySource()
        {
            if (SelectedCamera == null)
            {
                MessageBox.Show(
                    "Please select a camera.",
                    "Camera",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            await Task.Run(() =>
            {
                WebCamService.Instance.Start(SelectedCamera.Value);
            });

            MessageBox.Show(
                $"Connected to Camera {SelectedCamera.Value}",
                "Camera Connected",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // =====================================================
        // SERIAL CONNECT
        // =====================================================

        [RelayCommand]
        private void Connect()
        {
            if (SelectedPort == null ||
                SelectedBaudRate == null ||
                SelectedDataBits == null ||
                SelectedParity == null)
            {
                return;
            }

            bool ok = SerialService.Instance.Connect(
                SelectedPort.Value,
                SelectedBaudRate.Value,
                SelectedDataBits.Value,
                SelectedParity.Value);

            if (ok)
            {
                MessageBox.Show(
                    $"Connected to {SelectedPort.Value} at {SelectedBaudRate.Value} baud.",
                    "Connection Status",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Failed to open {SelectedPort.Value}.",
                    "Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =====================================================
        // SERIAL DISCONNECT
        // =====================================================

        [RelayCommand]
        private void Disconnect()
        {
            if (SelectedPort == null)
                return;

            SerialService.Instance.Disconnect();

            MessageBox.Show(
                $"Disconnected from {SelectedPort.Value}.",
                "Connection Status",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}