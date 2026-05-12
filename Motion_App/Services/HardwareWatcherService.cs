using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Motion_App.Service
{
    public class HardwareWatcherService
    {
        private static readonly Lazy<HardwareWatcherService> _instance = new(() => new HardwareWatcherService());
        public static HardwareWatcherService Instance => _instance.Value;

        /// <summary>
        /// Occurs when a hardware device (like USB, COM port, or Camera) is connected or disconnected.
        /// </summary>
        public event Action? HardwareChanged;

        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        private HardwareWatcherService() { }

        /// <summary>
        /// Initializes the hardware watcher by hooking into the window's message loop.
        /// Must be called after the window handle is created.
        /// </summary>
        /// <param name="window">The main window of the application.</param>
        public void Init(Window window)
        {
            if (window == null) return;

            var helper = new WindowInteropHelper(window);
            IntPtr handle = helper.Handle;

            if (handle == IntPtr.Zero)
            {
                window.SourceInitialized += (s, e) =>
                {
                    var h = new WindowInteropHelper(window);
                    HwndSource source = HwndSource.FromHwnd(h.Handle);
                    source.AddHook(HwndHandler);
                };
            }
            else
            {
                HwndSource source = HwndSource.FromHwnd(handle);
                source.AddHook(HwndHandler);
            }
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                int eventCode = wparam.ToInt32();
                // We care about device arrival and removal
                if (eventCode == DBT_DEVICEARRIVAL || eventCode == DBT_DEVICEREMOVECOMPLETE)
                {
                    // Delay slightly to allow the OS to finish initializing the device
                    NotifyHardwareChanged();
                }
            }
            return IntPtr.Zero;
        }

        private async void NotifyHardwareChanged()
        {
            // Wait a bit for the device to be fully ready in the system
            await System.Threading.Tasks.Task.Delay(1000);
            HardwareChanged?.Invoke();
        }
    }
}
