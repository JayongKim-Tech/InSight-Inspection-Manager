using Cognex.InSight;
using Cognex.InSight.Controls.Display;
using InSight_Manager.View;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;

namespace InSight_Manager.View // 네임스페이스는 프로젝트에 맞게 수정
{
    public class CellInfoEventArgs : EventArgs
    {
        public string Address { get; set; }
        public string Formula { get; set; }
    }

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
                display.ImageScale = 0.65;

                var viewModelController = GetController(host);
                if (viewModelController == null)
                {
                    SetController(host, new DisplayControllerWrapper(display, display.InSight));
                }

            }
        }
        #endregion


        private class DisplayControllerWrapper : IDisplayController
        {
            private readonly CvsInSightDisplay _display;

            public event EventHandler<CellInfoEventArgs> CellChanged;

            public event EventHandler InspectionFinished;

            public DisplayControllerWrapper(CvsInSightDisplay display, CvsInSight insight)
            {
                if (insight == null || display == null) return;

                _display = display;
                _display.InSight = insight;

                _display.CurrentCellChanged += Edit_FocusedCellChanged;

                _display.CurrentCellExpressionChanged += Edit_FocusedCellChanged;
                _display.CurrentCellExpressionChanged += Palette;

                _display.InSight.ResultsChanged += OnSensorResultsChanged;
            }
            private void Palette(object sender, EventArgs e)
            {
                _display.CurrentCellExpression.GetEnumerator().MoveNext();
            }
            private void Edit_FocusedCellChanged(object sender, EventArgs e)
            {
                try
                {
                    var cell = _display.CurrentCell.ToString();
                    var value = _display.CurrentCellExpression.ToString();

                    if (cell != null)
                    {
                        CellChanged?.Invoke(this, new CellInfoEventArgs
                        {
                            Address = cell,
                            Formula = value
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"셀 정보 추출 실패: {ex.Message}");
                }
            }

            private void OnSensorResultsChanged(object sender, EventArgs e)
            {
                InspectionFinished?.Invoke(this, EventArgs.Empty);
            }

            public CvsInSightDisplay InSightDisplay => _display;
            public CvsInSight InSightSensor => _display.InSight;
            public void SetZoomIn(double scale) => _display.ImageScale += scale;
            public void SetZoomOut(double scale) => _display.ImageScale -= scale;
            public void SetGrid(bool show) { _display.ShowGrid = show; _display.Invalidate(); }
            public void SetGraphics(bool show) => _display.ShowGraphics = show;
            public void FitImage() => _display.ImageScale = 0.65;
            public void OpenJob(string filepath)
            {
                _display.InSight.File.LoadJobFileLocally(filepath);
            }
            public void SaveJob(string filename)
            {
                _display.InSight.File.SaveJobFileLocally(filename);
            }
            public void NewJob() => _display.InSight.File.CreateNewJob();

            public void IsCustomView(bool show) => _display.ShowCustomView = show;
            public void IsGraphicView(bool show) => _display.ShowGraphics = show;

            public void IsOnline(bool online) => _display.SoftOnline = online;

            public void ShowImage(string filePath)
            {
                try
                {

                    if (_display != null && !string.IsNullOrEmpty(filePath))
                    {
                        _display.Edit.OpenImageFromFile(filePath);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"이미지 로드 실패: {ex.Message}");
                }
            }

            public void ShowDepedencyIncrease()
            {
                if(_display.ShowGrid)
                {
                    _display.Edit.ShowDependencyLevelsIncrease.Execute();
                }
            }

            public void ShowDepedencyDecrease()
            {
                if(_display.ShowGrid)
                {
                    _display.Edit.ShowDependencyLevelsDecrease.Execute();
                }
            }




        }


    }
}