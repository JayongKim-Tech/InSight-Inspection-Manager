using Cognex.InSight;
using Cognex.InSight.Controls.Display;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;

namespace InSight_Manager.ViewModel // 네임스페이스는 프로젝트에 맞게 수정
{
    public static class InSightBehavior
    {

        #region Controller

        public static IDisplayController GetController(DependencyObject obj) => (IDisplayController)obj.GetValue(ControllerProperty);
        public static void SetController(DependencyObject obj, IDisplayController value) => obj.SetValue(ControllerProperty, value);

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.RegisterAttached("Controller", typeof(IDisplayController), typeof(InSightBehavior),
                new PropertyMetadata(null));

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

                display.InSight = e.NewValue as CvsInSight;

                var viewModelController = GetController(host);
                if (viewModelController == null)
                {
                    // 즉석에서 구현체(Wrapper)를 만들어서 보냄
                    SetController(host, new DisplayControllerWrapper(display));
                }



            }
        }
        #endregion


        private class DisplayControllerWrapper : IDisplayController
        {
            private readonly CvsInSightDisplay _display;
            public DisplayControllerWrapper(CvsInSightDisplay display) => _display = display;

            public void SetZoomIn(double scale) => _display.ImageScale = scale + 0.1;
            public void SetZoomOut(double scale) => _display.ImageScale = scale - 0.1;
            public void SetGrid(bool show) { _display.ShowGrid = show; _display.Invalidate(); }
            public void SetGraphics(bool show) => _display.ShowGraphics = show;
            public void FitImage() => _display.ImageScale = 0.75;
        }


    }


}