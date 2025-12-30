using Cognex.InSight;
using Cognex.InSight.Controls.Display;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;

namespace InSight_Manager.ViewModel // 네임스페이스는 프로젝트에 맞게 수정
{
    public static class InSightBehavior
    {


        #region SpreadSheet Control

        public static bool GetShowSpreadsheet(DependencyObject obj) => (bool)obj.GetValue(ShowSpreadsheetProperty);
        public static void SetShowSpreadsheet(DependencyObject obj, bool value) => obj.SetValue(ShowSpreadsheetProperty, value);

        public static readonly DependencyProperty ShowSpreadsheetProperty =
            DependencyProperty.RegisterAttached(
                "ShowSpreadsheet",
                typeof(bool),
                typeof(InSightBehavior),
                new PropertyMetadata(false, OnShowSpreadsheetChanged));

        private static void OnShowSpreadsheetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(d)) return;

            if (d is WindowsFormsHost host && host.Child is CvsInSightDisplay display)
            {
                bool show = (bool)e.NewValue;

                if (show)
                {
                    // 스프레드시트 모드: 격자 켜고, 영상은 꺼버림 (확실하게 보이게)
                    display.ShowGrid = true;
                    display.ShowCustomView = false;
                }
                else
                {
                    // 라이브 모드: 격자 끄고, 영상 다시 켬
                    display.ShowGrid = false;
                    display.ShowCustomView = true;
                }
            }
        }

        #endregion

        #region Camera 연결
        public static CvsInSight GetSensorSource(DependencyObject obj) => (CvsInSight)obj.GetValue(SensorSourceProperty);
        public static void SetSensorSource(DependencyObject obj, CvsInSight value) => obj.SetValue(SensorSourceProperty, value);

        public static readonly DependencyProperty SensorSourceProperty =
            DependencyProperty.RegisterAttached("SensorSource", typeof(CvsInSight), typeof(InSightBehavior), new PropertyMetadata(null, OnSensorSourceChanged));

        private static void OnSensorSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowsFormsHost host && host.Child is CvsInSightDisplay display)
            {
                // 새 카메라가 연결되면 화면에 꽂아줍니다.
                // (단, 로컬 이미지를 보고 있을 때는 무시하도록 로직을 짤 수도 있습니다)
                display.InSight = e.NewValue as CvsInSight;
            }
        }

        #endregion

    }
}