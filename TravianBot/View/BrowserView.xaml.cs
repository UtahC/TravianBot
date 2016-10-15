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
        Client client;
        DevToolsWindow devToolsWindow;
        bool isBrowserLoaded = false;

        public BrowserView()
        {
            EO.WebBrowser.Runtime.AddLicense("yuGhWabCnrWfWbP3+hLtmuv5AxC9seLXCNzDf9vKyN/QgbrNwdvBfLDZ+Oi8dab3+hLtmuv5AxC9RoGkseeupeDn9hnynrWRm3Xj7fQQ7azcwp61n1mz8PoO5Kfq6doPvWmstMjitWqstcXnrqXg5/YZ8p7A6M+4iVmXwP0U4p7l9/YQn6fY8fbooX7GsugQ4Xvp8wge5KuZws3a66La6f8e5J61kZvLn3XY8P0a9neEjrHLu5rb6LEf+KncwbPwzme67AMa7J6ZpLEh5Kvq7QAZvFuour/boVmmwp61n1mzs/IX66juwp61n1mz8wMP5KvA8vcan53Y+PbooWmps8HdrmuntcfNn6/c9gQU7qe0psI=");
            EO.Wpf.Runtime.AddLicense("oBnlqJfo8h/kdpm0w9qva6a2wdy1W5f69h3youbyzs2wb5mkwOmMQ5ekzR7ooOXlBSDxnrXl6xjlr/D6xR/NbMn8/fXisrj79yXmdrTAwB7ooOXlBSDxnrWRm8ufdabw+g7kp+rpz7iJdePt9BDtrNzCnrWfWbPw+g7kp+rp2g+9bqm2yeGwb6y8x+eupeDn9hnynsDoz7iJWZfA/RTinuX39hCfp9jx9uihfsay6BvlW7XAwBfonNzyBBDkd4SOscu7muPwACK9RoGksefgndukBSTvnrSm1vqtkOfqs8ufr9z2BBTup7SmwuGtaZmkwOmMQ5ekzdrgpePzCOmMQ5ekzRrxndz22g==");

            InitializeBrowserRuntime();
            InitializeComponent();
            mainViewModel = DataContext as MainViewModel;
            client = mainViewModel.Client;
            InitializeWebView();
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

        private void InitializeWebView()
        {
            if (Setting.Default.UserAgent != Core.Enums.UserAgents.Default)
                webView.CustomUserAgent = Setting.Default.UserAgentString;
            
            webView.LoadCompleted += BrowserLoaded;
            webView.IsLoadingChanged += WebView_IsLoadingChanged;
            webView.MouseUp += (s, e) => client.SetBotUnavailableSpan(5000);
            webView.CanGoBackChanged += (s, e) => btnGoBack.IsEnabled = webView.CanGoBack;
            webView.CanGoForwardChanged += (s, e) => btnGoForward.IsEnabled = webView.CanGoForward;
            
            client.PropertyChanged += (s, e) =>
            {
                if (!isBrowserLoaded)
                    return;

                client.HtmlAvailableSignal.Reset();
                switch (e.PropertyName)
                {
                    case "Url":
                        if (client.Url == webView.Url)
                            webView.Reload(false);
                        else
                            webView.LoadUrl(client.Url);
                        //need to block client.html
                        break;
                    case "Javascript":
                        webView.QueueScriptCall(
                            new ScriptCall(client.Javascript, () => 
                            {
                                //client.HtmlAvailableSignal.Set();
                                //MessageBox.Show("scriptcall callback");
                            }));
                        
                        //webView.EvalScript(client.Javascript, true);
                        break;
                }
            };
        }

        private void WebView_IsLoadingChanged(object sender, EventArgs e)
        {
            if (!webView.IsLoading)
            {
                webView.EvalScript(File.ReadAllText(Path.Combine(client.BasePath, "jquery-1.12.4.min.js")));
                client.Url = webView.Url;
                client.Html = webView.GetHtml();
                client.HtmlAvailableSignal.Set();
                UITask.LoadVillages(webView.GetHtml());
                UITask.GetUpdatedActivedVillage(webView.GetHtml());
                if (webView.Url.Contains(UriGenerator.UrlSuburbs) || webView.Url.Contains(UriGenerator.UrlCity))
                {
                    UITask.LoadCurrentBuildings(webView.GetHtml());
                    //
                    var village = client.Villages.Where(v => v.IsActive == true).FirstOrDefault();
                    if (village != null)
                    {
                        foreach (var building in village.Buildings)
                            client.Logger.Write($"{building.VillageId} {building.BuildingId:00} {building.BuildingType} {building.Level}");
                    }
                    //
                }
                MessageBox.Show("loaded");
            }
        }

        private void BrowserLoaded(object sender, LoadCompletedEventArgs e)
        {
            webView.LoadCompleted -= BrowserLoaded;
            isBrowserLoaded = true;

            
            if (devToolsWindow == null)
            {
                devToolsWindow = new DevToolsWindow();
                devToolsWindow.Attach(webView);
                devToolsWindow.Closed += (s, arg) => devToolsWindow = null;
            }
            devToolsWindow.Show();
        }

        private void txtUrl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                webView.Url = txtUrl.Text.Trim();
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
            webView.GoBack();
        }

        private void btnGoForward_Click(object sender, RoutedEventArgs e)
        {
            webView.GoForward();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            if ((webControl != null) &&
                !string.IsNullOrEmpty(webView.Url))
            {
                webView.Reload(true);
            }
        }

        private void btnBottingMessage_Click(object sender, RoutedEventArgs e)
        {
            Client.Default.SetBotUnavailableSpan(60000);
            btnBottingMessage.Visibility = Visibility.Hidden;
        }
    }
}
