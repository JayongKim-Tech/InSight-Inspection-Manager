using Cognex.InSight;
using Cognex.InSight.Controls.Display.EZBuilder;
using InSight_Inspection_Manager;
using InSight_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Resources.ResXFileRef;

namespace InSight_Manager.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        ConnectModel model = new ConnectModel();
        ImageManagerModel imageManagerModel = new ImageManagerModel();
        JobFileManagerModel jobFileManagerModel = new JobFileManagerModel();
        BrushConverter converter = new BrushConverter();
        CvsInSight _isInSightSensor = new CvsInSight();

        public bool _gridStatus, _customView, _graphicView, _onlineStatus = false;


        private string jobFilepath;

        private string _ipAddress = "127.0.0.1";
        private string _status = "Null";
        private string _selectedFolderPath = null;
        private string _fileName;
        private string _isOnlineText = "Offline";

        private bool _isLocalMode = true;
        private bool _isLiveMode = false;

        private List<string> _files;

        private BitmapImage _image;

        private string _connectStatus = "Offline";
        private Brush _isOnline = Brushes.Red;



        public ICommand ConnectCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }

        public ICommand ToggleSpreadsheetCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ZoomFitCommand { get; }

        public ICommand SaveJobCommand { get; }
        public ICommand OpenJobCommand { get; }
        public ICommand NewJobCommand { get; }

        public ICommand ToggleCustomViewCommand { get; }
        public ICommand ToggleGraphicsCommand { get; }
        public ICommand ToggleOnlineCommand { get; }



        private IDisplayController _displayController;

        public IDisplayController DisplayController
        {
            get => _displayController;
            set { _displayController = value; OnPropertyChanged(); }
        }

        public bool IsLiveMode
        {
            get => _isLiveMode;
            set
            {
                _isLiveMode = value;
                OnPropertyChanged();
            }
        }
        public bool IsLocalMode
        {
            get => _isLocalMode;
            set
            {
                _isLocalMode = value;
                OnPropertyChanged();
            }
        }

        public string CurrentImageName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set
            {
                _selectedFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionStatusText
        {
            get => _connectStatus;
            set
            {
                _connectStatus = value;
                OnPropertyChanged();
            }
        }

        public string IsOnlineText
        {
            get => _isOnlineText;
            set
            {
                _isOnlineText = value;
                OnPropertyChanged();
            }
        }

        public Brush IsOnline
        {
            get => _isOnline;
            set
            {
                _isOnline = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage DisplayedImage
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public CvsInSight IsInSightSensor
        {
            get=> _isInSightSensor;
            set
            {
                _isInSightSensor = value;
                OnPropertyChanged();
            }
        }



        private void OnConnect()
        {
            if(model.ConnectToEmulator(IsInSightSensor,IpAddress))
            {
                MessageBox.Show("Insight Connect Complete!");
                ConnectionStatusText = "Connected";

            }
            else
            {
                ConnectionStatusText = "Disconnected";
            }

        }

        private void SelectFolder(object obj)
        {
            SelectedFolderPath =  imageManagerModel.SelectFolder(SelectedFolderPath);


            _files = imageManagerModel.GetImageFiles(SelectedFolderPath);

            if(_files != null)
            {
                CurrentImageName = imageManagerModel.Showiamge(_files);
                DisplayedImage = imageManagerModel.LoadBitmap(CurrentImageName);
            }
        }

        private void NextImage(object obj)
        {
            if(_files != null)
            {
                CurrentImageName = imageManagerModel.NextImage(_files);
                DisplayedImage = imageManagerModel.LoadBitmap(CurrentImageName);
            }

        }

        private void PrevImage(object obj)
        {
            if (_files != null)
            {
                CurrentImageName = imageManagerModel.PrevImage(_files);
                DisplayedImage = imageManagerModel.LoadBitmap(CurrentImageName);
            }
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


        private void SaveJob(object obj)
        {

            DisplayController?.SaveJob(jobFileManagerModel.SaveJobDlg());
        }

        private void OpenJob(object obj)
        {

            DisplayController?.OpenJob(jobFileManagerModel.OpenJobDlg());
        }

        private void NewJob(object obj)
        {
            var result = System.Windows.MessageBox.Show(
                "Do you want to save the changes?",   // 메시지 내용
                "Check Please",                     // 창 제목
                System.Windows.MessageBoxButton.YesNo,  // 버튼 종류 (Yes/No)
                System.Windows.MessageBoxImage.Question // 아이콘 (물음표)
            );

            // 2. 선택 결과 bool로 변환 (Yes를 누르면 true, 아니면 false)
            bool isConfirmed = (result == System.Windows.MessageBoxResult.Yes);

            if (isConfirmed) jobFileManagerModel.SaveJobDlg();
            else DisplayController?.NewJob();

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

        private void SetOnline(object obj)
        {
            _onlineStatus = !_onlineStatus;

            if (_onlineStatus)
            {
                IsOnlineText = "Online";
                IsOnline = Brushes.Green;
            }


            else
            {
                IsOnlineText = "Offline";
                IsOnline = Brushes.Red;
            }


            DisplayController?.IsOnline(_onlineStatus);


        }

        public MainViewModel()
        {
            // ConnectCommand를 누르면 OnConnect 함수를 실행해라!
            SelectFolderCommand = new RelayCommand(SelectFolder);
            NextImageCommand = new RelayCommand(NextImage);
            PrevImageCommand = new RelayCommand(PrevImage);
            ToggleSpreadsheetCommand = new RelayCommand(ToggleSpreadsheet);
            ZoomFitCommand = new RelayCommand(ZoomFit);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            SaveJobCommand = new RelayCommand(SaveJob);
            OpenJobCommand = new RelayCommand(OpenJob);
            NewJobCommand = new RelayCommand(NewJob);
            ToggleCustomViewCommand = new RelayCommand(ShowCustomView);
            ToggleGraphicsCommand = new RelayCommand(ShowGraphicView);
            ToggleOnlineCommand = new RelayCommand(SetOnline);
            OnConnect();
        }




    }
}
