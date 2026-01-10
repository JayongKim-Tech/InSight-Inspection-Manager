using InSight_Manager.View; // 네임스페이스 확인 필요
using InSight_Manager.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace InSight_Manager
{
    public partial class App : Application
    {
        // 반드시 'async void' 여야 합니다. (Task 안됨)
        protected override async void OnStartup(StartupEventArgs e)
        {
            // MessageBox.Show("1. 프로그램 시작 진입");
            base.OnStartup(e);

            try
            {
                // 2. 로딩창 띄우기 시도
                SplashWindow splash = new SplashWindow();
                splash.Show();

                // 3. ViewModel 생성 시도
                var mainVM = new MainViewModel();

                // 4. 통신 연결 시도
                bool isConnected = await Task.Run(async () =>
                {
                    // UI 스레드 접근 시 Dispatcher 사용
                    splash.Dispatcher.Invoke(() => splash.StatusText.Text = "Connecting...");

                    // ViewModel 초기화 함수 호출
                    return await mainVM.InitializeAsync();
                });

                // 5. 결과 처리
                if (isConnected)
                {
                    MainWindow main = new MainWindow();
                    main.DataContext = mainVM;
                    main.Show();
                    splash.Close();
                }
                else
                {
                    MessageBox.Show("통신 연결 실패!");
                    splash.Close();
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"치명적 오류 발생:\n{ex.Message}\n\n{ex.StackTrace}");
                Shutdown();
            }
        }
    }
}