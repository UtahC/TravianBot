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
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
