using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows; // MessageBox용
using System.Windows.Input;
using System.Windows.Shapes;

namespace InSight_Manager.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        // 모델
        JobFileManagerModel jobFileManagerModel = new JobFileManagerModel();
        ImageManagerModel imageManagerModel = new ImageManagerModel();

        private ObservableCollection<string> _filmstripImages = new ObservableCollection<string>();

        public ObservableCollection<string> FilmstripImages


        {
            get => _filmstripImages;
            set
            {
                _filmstripImages = value;
                OnPropertyChanged();
            }
        }
        private IDisplayController _displayController;
        public IDisplayController DisplayController
        {
            get { return _displayController; }
            set { _displayController = value; OnPropertyChanged(); }
        }

        // 이미지명
        private string _selectedFilmstripImage;
        public string SelectedFilmstripImage
        {
            get => _selectedFilmstripImage;
            set
            {
                _selectedFilmstripImage = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(_selectedFilmstripImage))
                {
                    DisplayController?.ShowImage(_selectedFilmstripImage);
                }
            }
        }


        // 이미지 경로
        private string _selectedFolderPath;
        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set
            {
                _selectedFolderPath = value;
                OnPropertyChanged();
            }
        }

        // 커맨드
        public ICommand SaveJobCommand { get; set; }
        public ICommand OpenJobCommand { get; set; }
        public ICommand NewJobCommand { get; set; }
        public ICommand SelectFolderCommand { get; set; }
        public ICommand BatchTestCommand { get; set; }



        // 생성자
        public FileViewModel()
        {
            SaveJobCommand = new RelayCommand(SaveJob);
            OpenJobCommand = new RelayCommand(OpenJob);
            NewJobCommand = new RelayCommand(NewJob);
            SelectFolderCommand = new RelayCommand(SeletedFolder);

            BatchTestCommand = new RelayCommand(async (o) => await ExecuteBatchTest());

        }

        // --- 함수들 ---
        private void SaveJob(object obj)
        {
            DisplayController?.SaveJob(jobFileManagerModel.SaveJobDlg());
        }

        private void OpenJob(object obj)
        {
            DisplayController?.OpenJob(jobFileManagerModel.OpenJobDlg());
        }

        private async void SeletedFolder(object obj)
        {
            SelectedFolderPath = imageManagerModel.SelectFolder(SelectedFolderPath);

            if (!string.IsNullOrEmpty(SelectedFolderPath))
            {
                await imageManagerModel.FillFilmstripAsync(SelectedFolderPath, FilmstripImages);
            }
        }

        private void NewJob(object obj)
        {
            var result = MessageBox.Show(
                "Do you want to save the changes?",
                "Check Please",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            bool isConfirmed = (result == MessageBoxResult.Yes);

            if (isConfirmed)
                jobFileManagerModel.SaveJobDlg();
            else
                DisplayController?.NewJob();
        }

        private async Task ExecuteBatchTest()
        {
            // 1. 사전 체크
            if (string.IsNullOrEmpty(SelectedFolderPath) || !Directory.Exists(SelectedFolderPath))
            {
                MessageBox.Show("이미지 폴더를 먼저 선택해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. 센서 객체 확보
            var sensor = DisplayController?.InSightSensor;
            var display = DisplayController?.InSightDisplay;

            if (sensor == null)
            {
                MessageBox.Show("연결된 센서가 없습니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. 파일 목록 수집
            // FilmstripImages에 이미 로드된 리스트가 있다면 그걸 써도 되고, 새로 폴더를 긁어도 됩니다.
            var files = Directory.GetFiles(SelectedFolderPath, "*.bmp").ToList();

            // 4. 결과 저장 경로 설정
            string resultFilePath = System.IO.Path.Combine(SelectedFolderPath, $"BatchResult_{DateTime.Now:HHmmss}.txt");

            try
            {
                // 5. 모델 호출 (전수 조사 시작)
                MessageBox.Show($"{files.Count}장의 이미지 전수 조사를 시작합니다", "배치 실행");

                await imageManagerModel.RunBatchProcessAsync(files, sensor, resultFilePath, display);

                // 6. 완료 알림 및 결과 파일 열기
                var openResult = MessageBox.Show(
                    "배치 테스트가 완료되었습니다.\nNG 리스트 파일을 여시겠습니까?",
                    "완료",
                    MessageBoxButton.YesNo);

                if (openResult == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("notepad.exe", resultFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"배치 작업 중 오류 발생: {ex.Message}");
            }
        }

    }
}