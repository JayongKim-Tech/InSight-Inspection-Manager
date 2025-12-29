using System.Windows;
using Cognex.InSight;
using Cognex.InSight.Controls.Display;

namespace InSight_Manager.ViewModel // 네임스페이스는 프로젝트에 맞게 수정
{
    public static class InSightBehavior
    {
        private static readonly CvsInSightDisplay _cvsInSightDisplay;

        // 1. "SensorSource"라는 이름의 첨부 속성(Attached Property)을 등록합니다.
        public static readonly DependencyProperty SensorSourceProperty =
            DependencyProperty.RegisterAttached(
                "SensorSource",
                typeof(CvsInSight),
                typeof(InSightBehavior),
                new PropertyMetadata(null, OnSensorSourceChanged));

        // 2. XAML에서 값을 설정할 때 쓰는 Getter/Setter
        public static CvsInSight GetSensorSource(DependencyObject obj)
        {
            return (CvsInSight)obj.GetValue(SensorSourceProperty);
        }

        public static void SetSensorSource(DependencyObject obj, CvsInSight value)
        {
            obj.SetValue(SensorSourceProperty, value);
        }

        // 3. 값이 바뀌었을 때 (바인딩되었을 때) 실행되는 로직
        private static void OnSensorSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 1. d는 XAML에서 이 속성을 붙인 'WindowsFormsHost'여야 합니다.
            if (d is System.Windows.Forms.Integration.WindowsFormsHost host)
            {
                // 2. Host 안에 들어있는 실제 Cognex 컨트롤(Child)을 꺼냅니다.
                if (host.Child is CvsInSightDisplay displayControl)
                {
                    displayControl.InSight = e.NewValue as CvsInSight;
                }
            }
        }

        public static CvsInSightDisplay cvsInSightDisplay1
        {

        }

    }
}