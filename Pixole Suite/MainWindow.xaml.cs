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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Pixole_Suite
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        static String REPO_URL = "https://github.com/pixole-studios/Pixole-Suite";

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Pixole Suite";
            NavigateToView("HomePage");
        }

        private NavLink _lastNavItem;
        private void NavLinksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as NavLink;
            if (item == null || item == _lastNavItem)
                return;
            var clickedView = item.PageName?.ToString() ?? "SettingsView";
            if (!NavigateToView(clickedView)) return;
            _lastNavItem = item;
        }
        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"Pixole_Suite.Views.{clickedView}");
            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
            {
                //TODO : Handle this error with dialog box
                return false;
            }

            this.contentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        public class NavLink
        {
            public string Label { get; set; }
            public Symbol Symbol { get; set; }
            public string PageName { get; set; }
        }

        private ObservableCollection<NavLink> _navLinks = new ObservableCollection<NavLink>()
        {
            new NavLink() { Label = "Home", PageName="HomePage"},
            new NavLink() { Label = "Password Hasher", PageName="HomePage"},
            new NavLink() { Label = "Petal Keyboard", PageName="DemoPage1"},
            new NavLink() { Label = "Task Manager", PageName="DemoPage1"},
        };

        public ObservableCollection<NavLink> NavLinks
        {
            get { return _navLinks; }
        }


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
                    case "Button3":
                        Debug.Print("clicked source" + name);
                        break;
                    default:
                        Debug.Print("Unhandled button click");
                        break;

                }
            }
        }

        private void OpenRepoInBrowser()
        {
            Process.Start(new ProcessStartInfo(REPO_URL) { UseShellExecute = true });
        }

        private void ShowAboutDialog()
        {
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Vertical };
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
    }
}
