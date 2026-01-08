using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System.Collections.ObjectModel;
using System.IO;
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


        // 생성자
        public FileViewModel()
        {
            SaveJobCommand = new RelayCommand(SaveJob);
            OpenJobCommand = new RelayCommand(OpenJob);
            NewJobCommand = new RelayCommand(NewJob);
            SelectFolderCommand = new RelayCommand(SeletedFolder);

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


    }
}