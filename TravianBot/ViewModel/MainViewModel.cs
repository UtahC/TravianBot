using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TravianBot.Core;
using TravianBot.Core.Models;
using TravianBot.Log;
using TravianBot.Model;

namespace TravianBot.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        public Client Client { get { return Client.Default; } }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Client.Default.Logger = Logger.Default;
        }

        public ICommand AccountSettingSave { get { return new AsyncRelayCommand(() => Client.Setting.Save()); } }
        public ICommand TestCommand { get { return new AsyncRelayCommand(() => Client.Url = Client.Setting.Server); } }
        public ICommand LoginCommand { get { return new AsyncRelayCommand(() => Client.Login()); } }
    }

    public class AsyncRelayCommand : RelayCommand
    {
        public AsyncRelayCommand(Action execute) : base(execute)
        {
        }

        public override void Execute(object parameter)
        {
            Task.Run(() => base.Execute(parameter));
        }
    }
}