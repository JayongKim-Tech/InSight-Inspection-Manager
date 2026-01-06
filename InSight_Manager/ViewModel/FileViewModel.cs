using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System.Windows; // MessageBox용
using System.Windows.Input;

namespace InSight_Manager.ViewModel
{
    public class FileViewModel : ViewModelBase
    {
        // 모델
        JobFileManagerModel jobFileManagerModel = new JobFileManagerModel();

        // 리모컨
        private IDisplayController _displayController;
        public IDisplayController DisplayController
        {
            get { return _displayController; }
            set { _displayController = value; OnPropertyChanged(); }
        }

        // 커맨드
        public ICommand SaveJobCommand { get; set; }
        public ICommand OpenJobCommand { get; set; }
        public ICommand NewJobCommand { get; set; }

        // 생성자
        public FileViewModel()
        {
            SaveJobCommand = new RelayCommand(SaveJob);
            OpenJobCommand = new RelayCommand(OpenJob);
            NewJobCommand = new RelayCommand(NewJob);
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