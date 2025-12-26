using InSight_Manager.ViewModel;
using System.Windows;

// [중요] XAML의 x:Class="InSight_Manager.View.MainWindow"와 똑같아야 합니다.
namespace InSight_Manager.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // 이제 에러 안 날 겁니다!

            // ViewModel 연결 (혹시 ViewModel이 없다면 이 줄은 일단 주석 처리 // 하세요)
            this.DataContext = new MainViewModel();
        }
    }
}