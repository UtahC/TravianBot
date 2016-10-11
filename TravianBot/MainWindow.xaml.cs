using System;
using System.Windows;
using TravianBot.ViewModel;

namespace TravianBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            //MouseUp += (s, e) => (DataContext as MainViewModel).Client.LastUserClick = DateTime.Now;
            PreviewMouseUp += (s, e) => (DataContext as MainViewModel).Client.SetBotUnavailableSpan(5000);
            //browserView.webControl.WebView.Activate += SetWindowVisible;
            browserView.webControl.WebView.LoadCompleted += SetWindowVisible;
        }

        private void SetWindowVisible(object sender, EO.WebBrowser.LoadCompletedEventArgs e)
        {
            Top = 0;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            browserView.webControl.WebView.LoadCompleted -= SetWindowVisible;
        }
    }
}