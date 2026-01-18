using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FileManagerSystem.ViewModel // 네임스페이스 확인!
{
    // 1. 상태에 따라 색상을 반환 (True: 초록색, False: 빨간색)
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRunning = (bool)value;
            return isRunning ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green
                             : new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Red
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // 2. 상태에 따라 텍스트 반환 (True: Active, False: Stopped)
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isRunning = (bool)value;
            return isRunning ? "SYSTEM ACTIVE" : "SYSTEM STOPPED";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}