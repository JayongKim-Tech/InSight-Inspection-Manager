using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System.Windows;
using System.Windows.Input;

namespace InSight_Manager.ViewModel
{
    public class DisplayViewModel : ViewModelBase
    {

        private IDisplayController _displayController;

        public IDisplayController DisplayController
        {
            get => _displayController;
            set
            {
                if (_displayController != null)
                    _displayController.CellChanged -= OnCellChanged;

                _displayController = value;
                OnPropertyChanged();

                if (_displayController != null)
                    _displayController.CellChanged += OnCellChanged;
            }
        }

        private string _currentCellAddress = "A0 =";
        public string CurrentCellAddress
        {
            get => _currentCellAddress;
            set { _currentCellAddress = value; OnPropertyChanged(); }
        }


        private string _currentCellFormula = " ";
        public string CurrentCellFormula
        {
            get => _currentCellFormula;
            set { _currentCellFormula = value; OnPropertyChanged(); }
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

        public ICommand ShowDependencyIncrease { get; set; }
        public ICommand ShowDependencyDecrease { get; set; }



        // 생성자
        public DisplayViewModel()
        {
            ToggleSpreadsheetCommand = new RelayCommand(ToggleSpreadsheet);
            ZoomFitCommand = new RelayCommand(ZoomFit);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            ToggleCustomViewCommand = new RelayCommand(ShowCustomView);
            ToggleGraphicsCommand = new RelayCommand(ShowGraphicView);
            ShowDependencyIncrease = new RelayCommand(LevelsIncrease);
            ShowDependencyDecrease = new RelayCommand(LevelsDecrease);
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
            DisplayController?.SetZoomIn(0.05);
        }

        private void ZoomOut(object obj)
        {
            DisplayController?.SetZoomOut(0.05);
        }

        private void ShowCustomView(object obj)
        {
            _customView = !_customView;
            DisplayController?.IsCustomView(_customView);
        }

        private void ShowGraphicView(object obj)
        {
            _graphicView = !_graphicView;
            DisplayController?.IsGraphicView(_graphicView);
        }

        private void LevelsIncrease(object obj)
        {
            DisplayController?.ShowDepedencyIncrease();
        }
        private void LevelsDecrease(object obj)
        {
            DisplayController?.ShowDepedencyDecrease();
        }

        private void OnCellChanged(object sender, CellInfoEventArgs e)
        {
            if (DisplayController.InSightDisplay.ShowGrid)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentCellAddress = e.Address + " ="; // "A0 ="
                    CurrentCellFormula = e.Formula;        // 수식

                });
            }

        }

        private void OnDisplayChanged()
        {
            if(_displayController != null)
            {
                //_displayController.InspectionFinished +=
            }
        }

    }
}