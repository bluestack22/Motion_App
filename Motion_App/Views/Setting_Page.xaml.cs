using Motion_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Motion_App.View
{
    /// <summary>
    /// Interaction logic for Setting_Page.xaml
    /// </summary>
    public partial class Setting_Page : Page
    {
        private readonly SettingModel _viewModel;
        public Setting_Page()
        {
            InitializeComponent();
            // Create VM
            _viewModel = new SettingModel();

            // Bind DataContext
            DataContext = _viewModel;

            // UI init
            UpdateConnectionPanels();
            UpdateCameraPanels();

            // Scan camera sau khi UI loaded
            Loaded += Setting_Page_Loaded;
        }
        private async void Setting_Page_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.ScanCamerasCommand.ExecuteAsync(null);
        }

        private void ConnectionTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateConnectionPanels();
        }

        private void CameraSourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCameraPanels();
        }

        private void UpdateConnectionPanels()
        {
            if (ConnectionTypeList == null)
                return;

            var selectedItem = ConnectionTypeList.SelectedItem as ListBoxItem;
            var text = selectedItem?.Content?.ToString() ?? string.Empty;

            void Show(Grid panel, bool visible)
            {
                if (panel != null)
                    panel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }

            if (text.StartsWith("Serial", StringComparison.OrdinalIgnoreCase))
            {
                Show(SerialConnectionPanel, true);
                Show(CanConnectionPanel, false);
                Show(EthernetConnectionPanel, false);
            }
            else if (text.StartsWith("CAN", StringComparison.OrdinalIgnoreCase))
            {
                Show(SerialConnectionPanel, false);
                Show(CanConnectionPanel, true);
                Show(EthernetConnectionPanel, false);
            }
            else if (text.StartsWith("Ethernet", StringComparison.OrdinalIgnoreCase))
            {
                Show(SerialConnectionPanel, false);
                Show(CanConnectionPanel, false);
                Show(EthernetConnectionPanel, true);
            }
            else
            {
                // Các loại khác (I2C, LIN, SPI) tạm thời dùng lại Serial layout
                Show(SerialConnectionPanel, true);
                Show(CanConnectionPanel, false);
                Show(EthernetConnectionPanel, false);
            }
        }

        private void UpdateCameraPanels()
        {
            if (CameraSourceList == null)
                return;

            var selectedItem = CameraSourceList.SelectedItem as ListBoxItem;
            var text = selectedItem?.Content?.ToString() ?? string.Empty;

            void Show(Grid panel, bool visible)
            {
                if (panel != null)
                    panel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }

            if (text.StartsWith("Camera thuong", StringComparison.OrdinalIgnoreCase))
            {
                Show(NormalCameraPanel, true);
                Show(BaslerCameraPanel, false);
                Show(FolderImagePanel, false);
            }
            else if (text.StartsWith("Camera Basler", StringComparison.OrdinalIgnoreCase))
            {
                Show(NormalCameraPanel, false);
                Show(BaslerCameraPanel, true);
                Show(FolderImagePanel, false);
            }
            else if (text.StartsWith("Folder", StringComparison.OrdinalIgnoreCase))
            {
                Show(NormalCameraPanel, false);
                Show(BaslerCameraPanel, false);
                Show(FolderImagePanel, true);
            }
            else
            {
                // Camera khác (SDK) tạm dùng layout normal
                Show(NormalCameraPanel, true);
                Show(BaslerCameraPanel, false);
                Show(FolderImagePanel, false);
            }
        }

  
    }
}
