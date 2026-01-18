namespace FileManagerSystem.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public StorageViewModel StorageVM { get; set; }
        public LogViewModel LogVM { get; set; }

        public MainViewModel()
        {
            // 2. 여기서 실제 객체를 생성합니다.
            StorageVM = new StorageViewModel();
            LogVM = new LogViewModel();

            // (선택사항) StorageVM에서 로그를 남기고 싶을 때 LogVM을 연결해줄 수도 있습니다.
            // StorageVM.SetLogger(LogVM);
        }
    }
}