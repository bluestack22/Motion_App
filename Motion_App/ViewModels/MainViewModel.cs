using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Motion_App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object? _currentViewModel;

        // Cached instances of ViewModels
        private readonly VisionModel _visionViewModel;
        // Mock view models for other pages if they don't exist yet, or just create empty ones.
        // Wait, Motor, Trace don't have ViewModels yet in the previous snippet, they were just empty pages.
        // I will just use object for now, or instantiate basic instances if needed.
        // Wait, Setting_Page has SettingModel.
        private readonly SettingModel _settingViewModel;

        // For Motor and Trace, if they don't have VM, we can just use empty objects or create basic VMs.
        // Since MVVM is object-based, we can just bind to dummy objects if VMs aren't ready.
        private readonly MotorModel _motorViewModel;
        private readonly TraceModel _traceViewModel;

        public MainViewModel()
        {
            _visionViewModel = new VisionModel();
            _settingViewModel = new SettingModel();
            _motorViewModel = new MotorModel();
            _traceViewModel = new TraceModel();

            // Default view
            CurrentViewModel = _visionViewModel;
        }

        [RelayCommand]
        private void Navigate(string viewName)
        {
            switch (viewName)
            {
                case "Vision":
                    CurrentViewModel = _visionViewModel;
                    break;
                case "Motor":
                    CurrentViewModel = _motorViewModel;
                    break;
                case "Trace":
                    CurrentViewModel = _traceViewModel;
                    break;
                case "Setting":
                    CurrentViewModel = _settingViewModel;
                    break;
                case "Main":
                    CurrentViewModel = null; // null means show Main content
                    break;
            }
        }
    }
}
