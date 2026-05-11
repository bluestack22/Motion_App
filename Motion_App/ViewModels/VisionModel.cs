using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Motion_App.Service;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;




namespace Motion_App.ViewModels
{
    public enum VisionMode
    {
        OpenCV,
        AI
    }
    public partial class VisionModel : ObservableObject
    {
        public VisionModel()
        {
            // subscribe to camera frames from WebCamService (existing service layer)
            WebCamService.Instance.FrameReady += OnFrameReady;
        }
        [ObservableProperty] private VisionMode _currentMode = VisionMode.OpenCV;
        //-----------------Feeds-----------------
        // Backend image data
        private Mat? _rawFrame;
        private Mat? _processedFrame;
        [ObservableProperty] private BitmapSource? _cameraFeed;
        [ObservableProperty] private BitmapSource? _processedFeed;

        private void OnFrameReady(Mat mat)
        {
            try
            {
                // Convert sang WPF image
                var bmp = mat.ToBitmapSource();

                // Freeze để cross-thread safe
                if (bmp.CanFreeze)
                    bmp.Freeze();

                // Update UI thread
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    CameraFeed = bmp;
                });
            }
            catch
            {
            }
            finally
            {
                // QUAN TRỌNG
                // Dispose Mat để tránh leak RAM
                mat.Dispose();
            }
        }

        public void Dispose()
        {
            WebCamService.Instance.FrameReady -= OnFrameReady;
        }

        //-----------------OpenCV Parameters-----------------
        [ObservableProperty] private int _threshold = 128;
        [ObservableProperty] private int _blur = 5;
        [ObservableProperty] private int _cannyLow = 0;
        [ObservableProperty] private int _cannyHigh = 255;
        public ObservableCollection<int> KernelSizes { get; } = new() { 3, 5, 7, 9, 11 };
        [ObservableProperty] private int _selectedKernel = 5;
        [ObservableProperty] private Int32 _area = 100;
        public ObservableCollection<int> Layer { get; } = new() { 1, 2, 3 };
        [ObservableProperty] private int _selectedLayer = 1;

        //-----------------AI Parameters-----------------
        [ObservableProperty] private float _confidenceThreshold = 0.5f;
        [ObservableProperty] private float _nmsThreshold = 0.4f;
        public ObservableCollection<string> SaveOptions { get; } = new() { "yolo.txt", "yolo.json" };
        [ObservableProperty] private string _selectedSaveOption = "yolo.txt";


        
        //------------------MOde Switch------------------
        [RelayCommand] private void OpenCVMode()
        {
            CurrentMode = VisionMode.OpenCV;
            System.Windows.MessageBox.Show("Switched to OpenCV Mode!", "Mode Switch", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void AIMode()
        {
            CurrentMode = VisionMode.AI;
            System.Windows.MessageBox.Show("Switched to AI Mode!", "Mode Switch", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        //------------------Button States------------------
        [RelayCommand] private void Save()
        {
            // Implement saving logic here, e.g., write to file or apply settings
            // For demonstration, we'll just show a message box (requires System.Windows)
            System.Windows.MessageBox.Show("Settings saved successfully!", "Save Settings", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void Capture()
        {
            // Reset all parameters to default values
            System.Windows.MessageBox.Show("Settings Capture successfully!", "Capture Settings", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

        }
        [RelayCommand] private void ThresholdView()
        {
            // Implement logic to handle threshold changes
            System.Windows.MessageBox.Show("Threshold changed!", "Threshold Change", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void BlurView()
        {
            // Implement logic to handle blur changes
            System.Windows.MessageBox.Show("Blur changed!", "Blur Change", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void EdgesView()
        {
            // Implement logic to handle Canny low threshold changes
            System.Windows.MessageBox.Show("Edges changed!", "Edges Change", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void Prev()
        {
            // Implement logic to update the processed feed preview
            System.Windows.MessageBox.Show("Switched to previous layer!", "Layer Switch", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        [RelayCommand] private void Next()
        {
            System.Windows.MessageBox.Show("Switched to next layer!", "Layer Switch", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            // Implement logic to switch to the next layer
        }
    }
}