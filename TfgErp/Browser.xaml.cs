using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace TfgErp
{
    /// <summary>
    /// Lógica de interacción para Browser.xaml
    /// </summary>
    public partial class Browser : Window, IWindowWithIcon
    {
        public Browser()
        {
            InitializeComponent();
            SetBrowserCompatibilityMode();
            webBrowser.Navigated += new NavigatedEventHandler(SuppressScriptErrors);
        }

        public ImageSource GetIcon()
        {
            return new BitmapImage(new Uri("https://cdn-icons-png.flaticon.com/512/6392/6392819.png", UriKind.RelativeOrAbsolute));
        }

        public string GetTitle()
        {
            // Retorna el título de la ventana
            return "Browser by Vsoftware";
        }
        void SuppressScriptErrors(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;
            if (browser != null)
            {
                browser.SuppressScriptErrors();
            }
        }

        private void SetBrowserCompatibilityMode()
        {
            string appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
            using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
            {
                key.SetValue(appName, 11000, RegistryValueKind.DWord); // 11000 corresponde a IE11
            }
        }


        private void Go_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl();
        }

        private void UrlBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl();
            }
        }

        private void NavigateToUrl()
        {
            if (!string.IsNullOrWhiteSpace(UrlBox.Text))
            {
                webBrowser.Navigate(FormatUrl(UrlBox.Text));
            }
        }

        public static string FormatUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("La URL proporcionada es inválida.");
            }

            if (!url.StartsWith("https://"))
            {
                if (url.StartsWith("www."))
                {
                    url = "https://" + url;
                }
                else
                {
                    url = "https://www." + url;
                }
            }
            else if (!url.StartsWith("https://www."))
            {
                url = "https://www." + url.Substring(8);
            }

            return url;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoBack)
            {
                webBrowser.GoBack();
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (webBrowser.CanGoForward)
            {
                webBrowser.GoForward();
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Refresh();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate("about:blank");
        }
    }
    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide = true)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;
            var objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            objComWebBrowser?.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
