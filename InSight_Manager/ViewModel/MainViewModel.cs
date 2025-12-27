using InSight_Inspection_Manager;
using InSight_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace InSight_Manager.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        ConnectModel model = new ConnectModel();
        ImageManagerModel imageManagerModel = new ImageManagerModel();

        private string _ipAddress = "127.0.0.1";
        private int port = 23;
        private string _status = "Null";
        private string _selectedFolderPath = null;
        private string _fileName = null;
        private List<string> _files;

        private BitmapImage _image = null;

        public ICommand ConnectCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand NextImageCommand { get; }
        public ICommand PrevImageCommand { get; }



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

        public BitmapImage DisplayedImage
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        private void OnConnect(object obj)
        {
            if(model.Connect(IpAddress, port))
            {
                Status = "OK";
            }
            else
            {
                Status = "Fail";
            }

        }

        private void SelectFolder(object obj)
        {
            SelectedFolderPath =  imageManagerModel.SelectFolder(SelectedFolderPath);

            _files = imageManagerModel.GetImageFiles(SelectedFolderPath);

            CurrentImageName = imageManagerModel.Showiamge(_files);
            DisplayedImage = imageManagerModel.LoadBitmap(imageManagerModel.Showiamge(_files));
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
            ConnectCommand = new RelayCommand(OnConnect);
            SelectFolderCommand = new RelayCommand(SelectFolder);
            NextImageCommand = new RelayCommand(NextImage);
            PrevImageCommand = new RelayCommand(PrevImage);

        }




    }
}
