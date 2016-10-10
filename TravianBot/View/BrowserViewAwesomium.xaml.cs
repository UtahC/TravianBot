using System;
using System.Collections.Generic;
using System.Linq;
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

namespace TravianBot.View
{
    /// <summary>
    /// Interaction logic for BrowserViewAwesomium.xaml
    /// </summary>
    public partial class BrowserViewAwesomium : UserControl
    {
        public BrowserViewAwesomium()
        {
            InitializeComponent();
            var mainViewModel = (DataContext as ViewModelLocator).Main;

            mainViewModel.Client.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Url")
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (webControl.Source.AbsoluteUri == mainViewModel.Client.Url)
                        {
                            mainViewModel.Client.Html = webControl.HTML;
                        }
                        else
                        {
                            webControl.Source = new Uri(mainViewModel.Client.Url);
                        }
                    });
                }
                else if (e.PropertyName == "Javascript")
                {
                    Dispatcher.Invoke(() =>
                    {
                        webControl.ExecuteJavascript(mainViewModel.Client.Javascript);
                    });
                }
            };
            webControl.LoadingFrameComplete += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    mainViewModel.Client.Html = webControl.HTML;
                    mainViewModel.Client.Url = webControl.Source.AbsoluteUri;
                });
            };
        }
    }
}
