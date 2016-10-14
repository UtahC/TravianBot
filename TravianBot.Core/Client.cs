﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace TravianBot.Core
{
    using Extensions;
    using HtmlAgilityPack;
    using LinqToDB;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using TravianBot.Core.Log;
    using TravianBot.Core.Models;
    using TravianBot.Core.State;

    public class Client : GalaSoft.MvvmLight.ObservableObject
    {
        private StateMachine stateMachine;
        private static Client client;
        private string url , html, javascript;
        private bool isBotWorking = false;
        private DateTime BotAvailableTime = new DateTime(1970, 1, 1);
        private ObservableCollection<Village> villages = new ObservableCollection<Village>();

        public ManualResetEvent BottingWorkAvailableSignal = new ManualResetEvent(true);
        public ManualResetEvent HtmlAvailableSignal = new ManualResetEvent(false);

        public static Client Default
        {
            get
            {
                if (client == null)
                    client = new Client();
                return client;
            }
        }
        public string Html
        {
            get
            {
                HtmlAvailableSignal.WaitOne();
                return html;
            }
            set
            {
                html = value;
            }
        }
        public HtmlDocument Document
        {
            get
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(Html);
                return doc;
            }
        }
        public bool IsBotWorking
        {
            get { return isBotWorking; }
            private set
            {
                if (value)
                    BottingWorkAvailableSignal.WaitOne();
                Set(() => IsBotWorking, ref isBotWorking, value);
            }
        }
        public ISetting Setting { get { return Core.Setting.Default; } }
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public string BotMessage { get; private set; }
        public string Javascript { get { return javascript; } private set { javascript = value; } }
        public IEventLogger EventLogger { get; set; }
        public ILogger Logger { get; set; }
        public StateMachine StateMachine { get; }
        public ObservableCollection<Village> Villages
        {
            get { return villages; }
        }
        public string BasePath { get; private set; }

        private Client()
        {
            //Get the main exe folder
            string exePath = Assembly.GetExecutingAssembly().GetName().CodeBase;
            exePath = new Uri(exePath).LocalPath;
            BasePath = Path.GetDirectoryName(exePath);

            url = Setting.Server;
            stateMachine = new StateMachine();

            villages.CollectionChanged += Villages_CollectionChanged;
        }

        private void Villages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Run(() => 
            {
                using (var db = new TravianBotDB())
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                var village = item as DB_Village;
                                db.Insert(village);
                            }
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                var village = item as DB_Village;
                                db.DB_Villages.Where(dV => dV.VillageId == village.VillageId).Delete();
                            }
                            break;
                    }
                }
            });
        }

        public void SetBotWorking(bool isBotWorking, string message = "")
        {
            IsBotWorking = isBotWorking;
            BotMessage = message;
        }

        public void AsyncLoadUrl(string url)
        {
            
        }

        public async Task LoadUrl(Uri uri)
        {
            await LoadUrl(uri.AbsoluteUri);
        }

        public async Task LoadUrl(string url)
        {
            Set(() => Url, ref this.url, url);
            await Task.Delay(Setting.DelayAfterLoadUrl);
        }

        public void ExecuteJavascript(string script)
        {
            Set(() => Javascript, ref javascript, script);
        }

        public void SetBotUnavailableSpan(int milliseconds)
        {
            SetBotUnavailableTimeUtil(DateTime.Now.AddMilliseconds(milliseconds));
        }

        public void SetBotUnavailableTimeUtil(DateTime dateTime)
        {
            if (dateTime > BotAvailableTime)
                BotAvailableTime = dateTime;
            BottingWorkAvailableSignal.Reset();
            Task.Run(async () =>
            {
                while (BotAvailableTime > DateTime.Now)
                    await Task.Delay(1000);
                BottingWorkAvailableSignal.Set();
            });
        }

        public void Login()
        {
            BotAvailableTime = new DateTime(1970, 1, 1);
            stateMachine.State = new LoginState() { IsLoginOnly = true };
            stateMachine.Start(new CancellationToken());
        }

        public void StartBot()
        {
            BotAvailableTime = new DateTime(1970, 1, 1);
            stateMachine.State = new LoginState();
            stateMachine.Start(new CancellationToken());
        }
    }

    public class BotWorkingEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public BotWorkingEventArgs()
        {

        }

        public BotWorkingEventArgs(string message)
        {
            Message = message;
        }
    }
}

