using Motion_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Motion_App.View
{
    /// <summary>
    /// Interaction logic for Vision_Page.xaml
    /// </summary>
    public partial class Vision_Page : UserControl
    {
        public Vision_Page()
        {
            InitializeComponent();
            // Ensure initial mode state after controls are created
            // Set the default toggle programmatically so Checked fires when all named controls are initialized
            BtnOpenCVMode.IsChecked = true;
            SetMode(isAIMode: false);
        }

        private void Mode_Checked(object sender, RoutedEventArgs e)
        {
            // Guard: if controls are not yet wired up, ignore event
            if (BtnAIMode == null || BtnOpenCVMode == null)
                return;

            if (sender is ToggleButton tb)
            {
                if (tb == BtnAIMode)
                {
                    // AI mode selected
                    BtnOpenCVMode.IsChecked = false;
                    SetMode(isAIMode: true);
                }
                else if (tb == BtnOpenCVMode)
                {
                    // OpenCV mode selected
                    BtnAIMode.IsChecked = false;
                    SetMode(isAIMode: false);
                }
            }
        }

        private void SetMode(bool isAIMode)
        {
            if (isAIMode)
            {
                OpenCV_LeftPanel.Visibility = Visibility.Collapsed;
                OpenCV_RightPanel.Visibility = Visibility.Collapsed;

                AI_LeftPanel.Visibility = Visibility.Visible;
                AI_RightPanel.Visibility = Visibility.Visible;
            }
            else
            {
                OpenCV_LeftPanel.Visibility = Visibility.Visible;
                OpenCV_RightPanel.Visibility = Visibility.Visible;

                AI_LeftPanel.Visibility = Visibility.Collapsed;
                AI_RightPanel.Visibility = Visibility.Collapsed;
            }
        }

    }
}
