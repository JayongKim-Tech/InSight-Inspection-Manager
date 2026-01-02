using Cognex.InSight;
using InSight_Inspection_Manager;
using InSight_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
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
        BrushConverter converter = new BrushConverter();
        CvsInSight _isInSightSensor = new CvsInSight();

        private string _ipAddress = "127.0.0.1";
        private int port = 23;
        private string _status = "Null";
        private string _selectedFolderPath = null;
        private string _fileName = null;

        private bool _isSpreadsheetVisible = false;

        private bool _isLocalMode = true;
        private bool _isLiveMode = false;

        private List<string> _files;

        private BitmapImage _image = null;

        private string _connectStatus = "Offline";
        private Brush _connectStatusColor;

        public ICommand ConnectCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }

        public ICommand ToggleSpreadsheetCommand { get; }


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

        public bool IsSpreadsheetVisible
        {
            get => _isSpreadsheetVisible;
            set
            {
                _isSpreadsheetVisible = value;
                OnPropertyChanged();
            }
        }
        public Brush ConnectionStatusColor
        {
            get => _connectStatusColor;
            set
            {
                _connectStatusColor = value;
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
                ConnectionStatusText = "Online";
                ConnectionStatusColor = (Brush)converter.ConvertFromString("#00FF00");
            }
            else
            {
                ConnectionStatusText = "Offline";
                ConnectionStatusColor = (Brush)converter.ConvertFromString("#FF0000");
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
            IsSpreadsheetVisible = !IsSpreadsheetVisible;

            if (IsSpreadsheetVisible)
            {
                // [ON 상태] 스프레드시트를 보고 싶다!
                // -> Cognex 화면(WindowsFormsHost)을 켜고, 로컬 이미지는 숨깁니다.
                IsLocalMode = true;

                // (중요) Cognex 화면에 "격자 보여줘" 신호 보내기 (ShowGrid = true)
                // Behavior가 이걸 감지하고 display.ShowGrid = true를 실행함
            }
            else
            {
                // [OFF 상태] 다시 이미지를 보고 싶다!
                // -> Cognex 화면을 끄고, 로컬 이미지를 다시 보여줍니다.
                IsLocalMode = true;
            }

        }


        public MainViewModel()
        {
            // ConnectCommand를 누르면 OnConnect 함수를 실행해라!
            SelectFolderCommand = new RelayCommand(SelectFolder);
            NextImageCommand = new RelayCommand(NextImage);
            PrevImageCommand = new RelayCommand(PrevImage);
            ToggleSpreadsheetCommand = new RelayCommand(ToggleSpreadsheet);
            OnConnect();

        }




    }
}
