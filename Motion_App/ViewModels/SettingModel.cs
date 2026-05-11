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

        public ObservableCollection<SelectionItem<string>> Ports { get; }
            = new()
        {
            new() { Value = "COM3", Display = "COM3" },
            new() { Value = "COM5", Display = "COM5" },
            new() { Value = "COM7", Display = "COM7" },
            new() { Value = "COM9", Display = "COM9" },
            new() { Value = "COM11", Display = "COM11" }
        };

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

            SelectedPort = Ports[1];

            SelectedBaudRate = BaudRates[4];

            SelectedDataBits = DataBits[1];

            SelectedParity = Parities[0];

        }

        // =====================================================
        // SCAN CAMERA
        // =====================================================

        [RelayCommand]
        private async Task ScanCameras()
        {
            var cameras = await Task.Run(() =>
            {
                return WebCamService.Instance.ScanCameras();
            });

            CameraList.Clear();

            foreach (var cam in cameras)
            {
                CameraList.Add(cam);
            }

            if (CameraList.Count > 0)
            {
                SelectedCamera = CameraList[0];
            }      
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