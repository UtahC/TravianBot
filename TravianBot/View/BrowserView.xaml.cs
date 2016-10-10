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
using System.Windows.Shapes;
using TravianBot.ViewModel;
using mshtml;

namespace TravianBot.View
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class BrowserView : UserControl
    {
        private MainViewModel mainViewModel;

        public BrowserView()
        {
            InitializeComponent();
            mainViewModel = (DataContext as ViewModelLocator).Main;
            webBrowser.LoadCompleted += (s, e) => 
            {
                dynamic doc = webBrowser.Document;
                mainViewModel.Client.Html = doc.documentElement.InnerHtml;
            };
            mainViewModel.Client.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "Url")
                    webBrowser.Navigate(mainViewModel.Client.Url);
            };
            webBrowser.SourceUpdated += (s, e) => 
            {
                mainViewModel.Client.Url = webBrowser.Source.AbsoluteUri;
            };
        }

        private void SetSilent(WebBrowser webBrowser)
        {
            dynamic activeX = webBrowser.GetType().InvokeMember("ActiveXInstance",
                    BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, webBrowser, new object[] { });

            activeX.Silent = true;
        }

        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webBrowser != null && webBrowser.CanGoBack;
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webBrowser != null && webBrowser.CanGoForward;
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.GoForward();
        }

        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = webBrowser != null;
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            webBrowser.Refresh();
        }

        private void webBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            textBoxUrl.Text = e.Uri.AbsoluteUri;
        }

        private void textBoxUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string url = textBoxUrl.Text.Contains("http://") ? textBoxUrl.Text : ("http://" + textBoxUrl.Text);
                webBrowser.Navigate(url);
            }
        }

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            SetSilent(webBrowser);
            webBrowser.Navigate("http://www.whoishostingthis.com/tools/user-agent/");
        }
    }

    public static class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html",
            typeof(string),
            typeof(BrowserBehavior),
            new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser wb = d as WebBrowser;
            if (wb != null)
                wb.NavigateToString(e.NewValue as string);
        }
    }
}
