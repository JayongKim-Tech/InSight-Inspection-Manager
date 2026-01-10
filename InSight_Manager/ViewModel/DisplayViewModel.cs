using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System.Windows.Input;

namespace InSight_Manager.ViewModel
{
    public class DisplayViewModel : ViewModelBase
    {

        // 리모컨 (메인에서 받아올 것임)
        private IDisplayController _displayController;

        public IDisplayController DisplayController
        {
            get { return _displayController; }
            set { _displayController = value; OnPropertyChanged(); }
        }



        // 상태 변수 (기존 이름 그대로)
        public bool _gridStatus = false;
        public bool _customView = false;
        public bool _graphicView = true;

        // 커맨드
        public ICommand ToggleSpreadsheetCommand { get; set; }
        public ICommand ZoomInCommand { get; set; }
        public ICommand ZoomOutCommand { get; set; }
        public ICommand ZoomFitCommand { get; set; }
        public ICommand ToggleCustomViewCommand { get; set; }
        public ICommand ToggleGraphicsCommand { get; set; }

        // 생성자
        public DisplayViewModel()
        {
            ToggleSpreadsheetCommand = new RelayCommand(ToggleSpreadsheet);
            ZoomFitCommand = new RelayCommand(ZoomFit);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ToggleCustomViewCommand = new RelayCommand(ShowCustomView);
            ToggleGraphicsCommand = new RelayCommand(ShowGraphicView);
        }


        private void ToggleSpreadsheet(object obj)
        {
            _gridStatus = !_gridStatus;
            DisplayController?.SetGrid(_gridStatus);
        }

        private void ZoomFit(object obj)
        {
            DisplayController?.FitImage();
        }

        private void ZoomIn(object obj)
        {
            DisplayController?.SetZoomIn(0.05); // 기존 값 0.05 유지
        }

        private void ZoomOut(object obj)
        {
            DisplayController?.SetZoomOut(0.05); // 기존 값 0.05 유지
        }

        private void ShowCustomView(object obj)
        {
            _customView = !_customView;
            DisplayController?.IsCustomView(_customView); // 리모컨 함수명 유지
        }

        private void ShowGraphicView(object obj)
        {
            _graphicView = !_graphicView;
            DisplayController?.IsGraphicView(_graphicView); // 리모컨 함수명 유지
        }



    }
}