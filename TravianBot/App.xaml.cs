using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.IO;
using System.Reflection;
using System;

namespace TravianBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string BasePath { get; private set; }

        static App()
        {
            //Get the main exe folder
            string exePath = Assembly.GetExecutingAssembly().GetName().CodeBase;
            exePath = new Uri(exePath).LocalPath;
            BasePath = Path.GetDirectoryName(exePath);

            DispatcherHelper.Initialize();
        }
    }
}
