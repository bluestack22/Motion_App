using System.Text;
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
using Motion_App.View;

namespace Motion_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, Page> _cachedPages = new();
        private string _currentTabKey = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigate to Vision page by default (Main tab)
            NavigateTo("Main");
        }

        private void TopTab_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            if (btn == null || TopTabPanel == null)
                return;

            foreach (var child in TopTabPanel.Children)
            {
                if (child is ToggleButton tb)
                    tb.IsChecked = false;
            }

            btn.IsChecked = true;
            // navigate based on button name or content
            string key = btn.Name.Replace("TopTab", "");
            NavigateTo(key);
        }

        
        private void NavigateTo(string key)
        {
            if (string.Equals(_currentTabKey, key, StringComparison.OrdinalIgnoreCase))
                return;

            // Show or hide main content vs pages in the frame depending on tab
            switch (key)
            {
                case "Main":
                    if (MainContent != null) MainContent.Visibility = Visibility.Visible;
                    if (MainFrame != null) MainFrame.Visibility = Visibility.Collapsed;
                    _currentTabKey = key;
                    break;
                case "Vision":
                    ShowPage(key, () => new Vision_Page());
                    break;
                case "Motor":
                    ShowPage(key, () => new Motor_Page());
                    break;
                case "Trace":
                    ShowPage(key, () => new Trace_Page());
                    break;
                case "Setting":
                    ShowPage(key, () => new Setting_Page());
                    break;
                default:
                    break;
            }
        }

        private void ShowPage(string key, Func<Page> pageFactory)
        {
            if (MainContent != null)
                MainContent.Visibility = Visibility.Collapsed;

            if (MainFrame == null)
                return;

            MainFrame.Visibility = Visibility.Visible;

            if (!_cachedPages.TryGetValue(key, out var page))
            {
                page = pageFactory();
                _cachedPages[key] = page;
            }

            // Reuse the same page instance to preserve UI state
            if (!ReferenceEquals(MainFrame.Content, page))
                MainFrame.Content = page;

            _currentTabKey = key;
        }
    }
}