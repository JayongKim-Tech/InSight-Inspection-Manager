using InSight_Inspection_Manager;
using Insight_Manager.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Insight_Manager.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        ConnectModel model = new ConnectModel();

        private string _ipAddress = "127.0.0.1";
        private int port = 23;
        private string _status = "Null";

        public ICommand ConnectCommand { get; }

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

        public MainViewModel()
        {
            // ConnectCommand를 누르면 OnConnect 함수를 실행해라!
            ConnectCommand = new RelayCommand(OnConnect);
        }




    }
}
