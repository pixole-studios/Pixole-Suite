using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Reflection;
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.Collections.ObjectModel;
//using System.Windows.Forms;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixole_Suite
{

    // Main Window which holds the panel and contents of each module
    public sealed partial class MainWindow : Window
    {
        static String REPO_URL = "https://github.com/pixole-studios/Pixole-Suite";
        static String MODULE_RELATIVE_DIR = "\\Modules\\";
        static String MODULE_LIST_FILENAME = MODULE_RELATIVE_DIR + "modules.csv";

        /// <summary>
        /// Initialize the main Window and load installed modules
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Pixole Suite";
            NavigateToView("HomePage");
            LoadInstalledModules();
        }

        /// <summary>
        /// Reads the CSV file of all installed modules, and loads the views into the program
        /// </summary>
        private void LoadInstalledModules()
        {
            // CSV in format Filename,Label,PageName
            var path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + MODULE_LIST_FILENAME;
            try
            {
                var contents = File.ReadAllText(path);
                var lines = contents.Split("\n");
                foreach (string line in lines)
                {
                    var vals = line.Split(",");
                    _navLinks.Add(new NavLink() { Label = vals[1], PageName = vals[2] });
                }
            }
            catch
            {
                Debug.Print("Error occurred whilst reading file: " + path);
            }
        }

        /// <summary>
        /// Puts a view as the contents of the main window
        /// </summary>
        /// <param name="clickedView">Name of the view to navigate to</param>
        /// <returns></returns>
        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"Pixole_Suite.Views.{clickedView}");
            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
            {
                //TODO : Handle this error with dialog box
                ShowErrorDialog("Unable to locate view: " + clickedView);
                return false;
            }

            this.contentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        /// <summary>
        /// Class bind for each element on the navigation panel
        /// </summary>
        public class NavLink
        {
            public string Label { get; set; }
            public Symbol Symbol { get; set; }
            public string PageName { get; set; }
        }

        /// <summary>
        /// Collection of all the NavLinks on the panel
        /// </summary>
        private ObservableCollection<NavLink> _navLinks = new ObservableCollection<NavLink>()
        {
            new NavLink() { Label = "Home", PageName="HomePage"},
        };

        /// <summary>
        /// Getter for navlinks
        /// </summary>
        public ObservableCollection<NavLink> NavLinks
        {
            get { return _navLinks; }
        }

        /// <summary>
        /// On clicking a menu item in the more flyout - perform required actions
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">RoutedEventArgs</param>
        public void More_Menu_click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem i)
            {
                string name = i.Name;

                switch (name)
                {
                    case "source":
                        OpenRepoInBrowser();
                        break;
                    case "about":
                        ShowAboutDialog();
                        break;
                    case "findOnline":
                        //TODO: Implement module search online
                        break;
                    default:
                        Debug.Print("Unhandled button click");
                        break;
                }
            }
        }


        public void Add_click(object sender, RoutedEventArgs e)
        {
            // TODO update to System.Windows.Forms.FolderBrowserDialog()
        }


        private NavLink _lastNavItem;
        /// <summary>
        /// When a nav item is clicked, navigate to it
        /// </summary>
        /// <param name="sender">The event sender</param>
        /// <param name="e">ItemClickEventArgs</param>
        private void NavLinksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as NavLink;
            if (item == null || item == _lastNavItem)
                return;
            var clickedView = item.PageName?.ToString() ?? "SettingsView";
            if (!NavigateToView(clickedView)) return;
            _lastNavItem = item;
        }

        /// <summary>
        /// Launch the git repository in browser
        /// </summary>
        private void OpenRepoInBrowser()
        {
            Process.Start(new ProcessStartInfo(REPO_URL) { UseShellExecute = true });
        }

        /// <summary>
        /// Displays and About dialog with version info etc
        /// </summary>
        private void ShowAboutDialog()
        {
            StackPanel stackPanel = new StackPanel() { Orientation = Microsoft.UI.Xaml.Controls.Orientation.Vertical };
            stackPanel.Children.Add(new TextBlock() { Text = "Version: ", FontWeight = Microsoft.UI.Text.FontWeights.Bold });
            stackPanel.Children.Add(new TextBlock() { Text = "alpha-0.1\n" });
            stackPanel.Children.Add(new TextBlock() { Text = "GitHub Repository: ", FontWeight = Microsoft.UI.Text.FontWeights.Bold });
            stackPanel.Children.Add(new TextBlock() { Text = REPO_URL});
            var aboutDialog = new ContentDialog
            {
                Title = "About",
                Content = stackPanel,
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Close
            };
            aboutDialog.XamlRoot = this.Content.XamlRoot;
            var task = aboutDialog.ShowAsync();
        }

        /// <summary>
        /// Display an error dialog with the provided message
        /// </summary>
        /// <param name="message">The error message to display</param>
        private void ShowErrorDialog(string message)
        {
            var errDialog = new ContentDialog
            {
                Title = "Error",
                Content = new TextBlock() {Text = message },
                CloseButtonText = "Close",
                DefaultButton = ContentDialogButton.Close
            };
            errDialog.XamlRoot = this.Content.XamlRoot;
            var task = errDialog.ShowAsync();
        }
    }
}
