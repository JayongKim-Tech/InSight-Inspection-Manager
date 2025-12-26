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


namespace InSight_Manager.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        ConnectModel model = new ConnectModel();

        private string _ipAddress = "127.0.0.1";
        private int port = 23;
        private string _status = "Null";
        private string _selectedFolderPath = "Default";

        public ICommand ConnectCommand { get; }
        public ICommand SelectFolderCommand { get; }

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

        private void SelectFolder(object obj)
        {
            // 1. 다이얼로그 인스턴스 생성 (using을 써서 사용 후 바로 메모리 해제)
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                // 창 상단에 뜰 설명 텍스트
                dialog.Description = "이미지가 저장된 폴더를 선택하세요";

                // '새 폴더 만들기' 버튼 숨기기 (단순 선택용이면 false 추천)
                dialog.ShowNewFolderButton = false;

                // 2. 창 띄우기 (사용자가 '확인'을 눌렀는지 체크)
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                // 3. 결과 처리
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // [중요] 사용자가 선택한 경로가 이 변수에 담깁니다.
                    SelectedFolderPath = dialog.SelectedPath;
                }
            }
        }

        public MainViewModel()
        {
            // ConnectCommand를 누르면 OnConnect 함수를 실행해라!
            ConnectCommand = new RelayCommand(OnConnect);
            SelectFolderCommand = new RelayCommand(SelectFolder);
        }




    }
}
