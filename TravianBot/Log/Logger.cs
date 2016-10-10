using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravianBot.Core.Log;

namespace TravianBot.Log
{
    class Logger : ILogger
    {
        private int index = 0;
        private static Logger logger;

        public static Logger Default
        {
            get
            {
                if (logger == null)
                    logger = new Logger();
                return logger;
            }
        }

        public ObservableCollection<LogEntry> LogEntries { get; set; } = new ObservableCollection<LogEntry>();

        private Logger()
        {

        }

        public void Write(string text)
        {
            index++;
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                LogEntries.Add(new LogEntry() { DateTime = DateTime.Now, Index = index, Message = text });
            }));
        }

        public void Write(string text, Brush color)
        {
            throw new NotImplementedException();
        }
    }

    public class LogEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime DateTime { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class CollapsibleLogEntry : LogEntry
    {
        public List<LogEntry> Contents { get; set; }
    }

    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }
}
