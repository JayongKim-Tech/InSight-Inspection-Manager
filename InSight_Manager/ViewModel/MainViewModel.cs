using Cognex.InSight;
using Cognex.InSight.Controls.Display.EZBuilder;
using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InSight_Manager.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public DisplayViewModel DisplayVM { get; set; } = new DisplayViewModel();
        public FileViewModel FileVM { get; set; } = new FileViewModel();


        ConnectModel model = new ConnectModel();
        CvsInSight _isInSightSensor = new CvsInSight();

        // 상태 변수
        public bool _onlineStatus = false;
        private string _ipAddress = "127.0.0.1";
        private string _status = "Null";
        private string _isOnlineText = "Offline";
        private string _connectStatus = "Offline";
        private Brush _isOnline = Brushes.Red;



        private IDisplayController _displayController;
        public IDisplayController DisplayController
        {
            get { return _displayController; }
            set
            {
                _displayController = value;

                DisplayVM.DisplayController = value;
                FileVM.DisplayController = value;

                OnPropertyChanged();
            }
        }


        public ICommand ToggleOnlineCommand { get; set; }

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; OnPropertyChanged(); }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        public string ConnectionStatusText
        {
            get { return _connectStatus; }
            set { _connectStatus = value; OnPropertyChanged(); }
        }
        public string IsOnlineText
        {
            get { return _isOnlineText; }
            set { _isOnlineText = value; OnPropertyChanged(); }
        }
        public Brush IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value; OnPropertyChanged(); }
        }

        public CvsInSight IsInSightSensor
        {
            get { return _isInSightSensor; }
            set { _isInSightSensor = value; OnPropertyChanged(); }
        }

        // =============================================
        // [6] 생성자
        // =============================================
        public MainViewModel()
        {

            ToggleOnlineCommand = new RelayCommand(SetOnline);

        }

        public async Task<bool> InitializeAsync()
        {
            return await Task.Run(() =>
            {
                return model.ConnectToEmulator(IsInSightSensor, IpAddress);
            });
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
    }
}