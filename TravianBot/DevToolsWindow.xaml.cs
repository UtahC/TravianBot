using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using EO.WebBrowser;

namespace TravianBot
{
    /// <summary>
    /// Interaction logic for DevToolsWindow.xaml
    /// </summary>
    public partial class DevToolsWindow : Window
    {
        public DevToolsWindow()
        {
            InitializeComponent();
        }

        public void Attach(WebView webView)
        {
            webView.Closed += webView_Closed;
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            webView.ShowDevTools(helper.Handle);
        }

        void webView_Closed(object sender, WebViewClosedEventArgs e)
        {
            Close();
        }
    }
}
