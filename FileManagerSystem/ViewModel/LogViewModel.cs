using System;
using System.Collections.ObjectModel;

namespace FileManagerSystem.ViewModel
{
    public class LogViewModel : ViewModelBase
    {
        public ObservableCollection<LogEntry> LogMessages { get; set; } = new ObservableCollection<LogEntry>();

        public void AddLog(string message)
        {
            // UI 스레드에서 안전하게 로그 추가
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessages.Insert(0, new LogEntry
                {
                    Timestamp = DateTime.Now.ToString("[HH:mm:ss]"),
                    Message = message
                });
            });
        }
    }

    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string Message { get; set; }
    }
}