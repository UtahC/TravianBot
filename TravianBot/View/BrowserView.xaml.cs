using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.IO;
using TravianBot.ViewModel;
using EO.WebBrowser;
using EO.Base;
using TravianBot.Core.Models;
using TravianBot.Core;
using TravianBot.Core.Extensions;
using System.Globalization;
using TravianBot.Core.Tasks;

namespace TravianBot.View
{
    /// <summary>
    /// Interaction logic for webControl.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        MainViewModel mainViewModel;
        bool isBrowserLoaded = false;

        public BrowserView()
        {
            EO.WebBrowser.Runtime.AddLicense("yuGhWabCnrWfWbP3+hLtmuv5AxC9seLXCNzDf9vKyN/QgbrNwdvBfLDZ+Oi8dab3+hLtmuv5AxC9RoGkseeupeDn9hnynrWRm3Xj7fQQ7azcwp61n1mz8PoO5Kfq6doPvWmstMjitWqstcXnrqXg5/YZ8p7A6M+4iVmXwP0U4p7l9/YQn6fY8fbooX7GsugQ4Xvp8wge5KuZws3a66La6f8e5J61kZvLn3XY8P0a9neEjrHLu5rb6LEf+KncwbPwzme67AMa7J6ZpLEh5Kvq7QAZvFuour/boVmmwp61n1mzs/IX66juwp61n1mz8wMP5KvA8vcan53Y+PbooWmps8HdrmuntcfNn6/c9gQU7qe0psI=");
            EO.Wpf.Runtime.AddLicense("oBnlqJfo8h/kdpm0w9qva6a2wdy1W5f69h3youbyzs2wb5mkwOmMQ5ekzR7ooOXlBSDxnrXl6xjlr/D6xR/NbMn8/fXisrj79yXmdrTAwB7ooOXlBSDxnrWRm8ufdabw+g7kp+rpz7iJdePt9BDtrNzCnrWfWbPw+g7kp+rp2g+9bqm2yeGwb6y8x+eupeDn9hnynsDoz7iJWZfA/RTinuX39hCfp9jx9uihfsay6BvlW7XAwBfonNzyBBDkd4SOscu7muPwACK9RoGksefgndukBSTvnrSm1vqtkOfqs8ufr9z2BBTup7SmwuGtaZmkwOmMQ5ekzdrgpePzCOmMQ5ekzRrxndz22g==");

            InitializeBrowserRuntime();
            InitializeComponent();
            mainViewModel = DataContext as MainViewModel;
            InitializeWebControl();
        }

        private void InitializeBrowserRuntime()
        {
            EO.WebBrowser.Runtime.CachePath = Path.Combine(Client.Default.BasePath, "Cache");

            var setting = Setting.Default;
            if (setting.IsUseProxy)
            {
                if (setting.ProxyLogin.IsNullOrEmptyOrWhiteSpace() || setting.ProxyPassword.IsNullOrEmptyOrWhiteSpace())
                    EO.WebBrowser.Runtime.Proxy = new ProxyInfo(ProxyType.HTTP, setting.ProxyHost, setting.ProxyPort);
                else
                    EO.WebBrowser.Runtime.Proxy = new ProxyInfo(ProxyType.HTTP, setting.ProxyHost, setting.ProxyPort, setting.ProxyLogin, setting.ProxyPassword);   
            }
            //EO.WebBrowser.Runtime.UILanguage = CultureInfo.CurrentCulture.IetfLanguageTag;
        }

        private void InitializeWebControl()
        {
            if (Setting.Default.UserAgent != Core.Enums.UserAgents.Default)
                webControl.WebView.CustomUserAgent = Setting.Default.UserAgentString;
            
            webControl.WebView.LoadCompleted += BrowserLoaded;
            //webControl.WebView.UrlChanged += (s, e) =>
            //    mainViewModel.Client.Url = webControl.WebView.Url;
            webControl.WebView.MouseUp += (s, e) => 
                mainViewModel.Client.SetBotUnavailableSpan(5000);
            webControl.WebView.CanGoBackChanged += (s, e) => 
                btnGoBack.IsEnabled = webControl.WebView.CanGoBack;
            webControl.WebView.CanGoForwardChanged += (s, e) =>
                btnGoForward.IsEnabled = webControl.WebView.CanGoForward;
            webControl.WebView.IsLoadingChanged += (s, e) => 
            {
                if (!webControl.WebView.IsLoading)
                {
                    mainViewModel.Client.Url = webControl.WebView.Url;
                    mainViewModel.Client.HtmlAvailableSignal.Set();
                    mainViewModel.Client.Html = webControl.WebView.GetHtml();
                    UITask.LoadVillages(webControl.WebView.GetHtml());
                    UITask.GetUpdatedActivedVillage(webControl.WebView.GetHtml());
                    if (webControl.WebView.Url.Contains(UriGenerator.UrlSuburbs) ||
                        webControl.WebView.Url.Contains(UriGenerator.UrlCity))
                    {
                        UtilityTask.LoadBuildingsInCurrentPage();
                    }
                    
                }
            };
            //
            //webControl.WebView.LoadCompleted += (s, e) => MessageBox.Show("LoadCompleted");
            //

            mainViewModel.Client.PropertyChanged += (s, e) =>
            {
                if (!isBrowserLoaded)
                    return;

                mainViewModel.Client.HtmlAvailableSignal.Reset();
                switch (e.PropertyName)
                {
                    case "Url":
                        if (mainViewModel.Client.Url == webControl.WebView.Url)
                            webControl.WebView.Reload(false);
                        else
                            webControl.WebView.LoadUrl(mainViewModel.Client.Url);
                        //need to block client.html
                        break;
                    case "Javascript":
                        webControl.WebView.QueueScriptCall(
                            new ScriptCall(mainViewModel.Client.Javascript, () => 
                            {
                                //mainViewModel.Client.HtmlAvailableSignal.Set();
                                //MessageBox.Show("scriptcall callback");
                            }));
                        
                        //webControl.WebView.EvalScript(mainViewModel.Client.Javascript, true);
                        break;
                }
            };
        }

        private void BrowserLoaded(object sender, LoadCompletedEventArgs e)
        {
            isBrowserLoaded = true;
            webControl.WebView.LoadCompleted -= BrowserLoaded;
        }

        private void txtUrl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                webControl.WebView.Url = txtUrl.Text.Trim();
                txtUrl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void txtUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUrl.SelectAll();
        }

        private void txtUrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!txtUrl.IsKeyboardFocusWithin)
            {
                txtUrl.Focus();
                txtUrl.SelectAll();
                e.Handled = true;
            }
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            webControl.WebView.GoBack();
        }

        private void btnGoForward_Click(object sender, RoutedEventArgs e)
        {
            webControl.WebView.GoForward();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if ((webControl != null) &&
                !string.IsNullOrEmpty(webControl.WebView.Url))
            {
                webControl.WebView.Reload(true);
            }
        }

        private void btnBottingMessage_Click(object sender, RoutedEventArgs e)
        {
            Client.Default.SetBotUnavailableSpan(60000);
            btnBottingMessage.Visibility = Visibility.Hidden;
        }
    }
}
