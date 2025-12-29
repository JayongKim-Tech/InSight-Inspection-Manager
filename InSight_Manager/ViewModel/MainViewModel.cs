using InSight_Inspection_Manager;
using InSight_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        private string _ipAddress = "127.0.0.1";
        private int port = 23;
        private string _status = "Null";
        private string _selectedFolderPath = null;
        private string _fileName = null;
        private List<string> _files;

        private BitmapImage _image = null;

        private string _connectStatus = "Offline";
        private Brush _connectStatusColor;

        public ICommand ConnectCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }


        public bool IsLiveMode { get; set; }
        public bool IsLocalMode => !IsLiveMode;
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
        private void OnConnect()
        {
            if(model.Connect(IpAddress, port))
            {
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
                DisplayedImage = imageManagerModel.LoadBitmap(imageManagerModel.Showiamge(_files));
            }
        }

        private void NextImage(object obj)
        {
            CurrentImageName = imageManagerModel.NextImage(_files);
            DisplayedImage = imageManagerModel.LoadBitmap(imageManagerModel.NextImage(_files));
        }

        private void PrevImage(object obj)
        {
            CurrentImageName = imageManagerModel.PrevImage(_files);
            DisplayedImage = imageManagerModel.LoadBitmap(imageManagerModel.PrevImage(_files));
        }


        public MainViewModel()
        {
            // ConnectCommand를 누르면 OnConnect 함수를 실행해라!
            SelectFolderCommand = new RelayCommand(SelectFolder);
            NextImageCommand = new RelayCommand(NextImage);
            PrevImageCommand = new RelayCommand(PrevImage);
            OnConnect();

        }




    }
}
