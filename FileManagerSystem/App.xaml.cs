using System;
using System.Windows;
using System.Windows.Forms; // 참조에 System.Windows.Forms가 있어야 함
using FileManagerSystem.View;
using FileManagerSystem.ViewModel;

namespace FileManagerSystem
{
    public partial class App : System.Windows.Application
    {
        private NotifyIcon _notifyIcon;
        private FileManagerSystem.View.MainWindow _mainWindow;

        // Application_Startup 대신 OnStartup을 오버라이드합니다.
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. 뷰모델 및 메인 윈도우 초기화
            var mainVM = new MainViewModel();
            _mainWindow = new FileManagerSystem.View.MainWindow();
            _mainWindow.DataContext = mainVM;

            // 2. 트레이 아이콘 설정
            _notifyIcon = new NotifyIcon();
            try
            {
                // 프로그램 실행 파일 아이콘 가져오기
                _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            catch
            {
                // 아이콘 추출 실패 시 기본 아이콘 설정 (필요시)
            }

            _notifyIcon.Text = "File Storage Manager";
            _notifyIcon.Visible = true;

            // 3. 트레이 메뉴 (우클릭)
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("열기 (Open)", null, (s, ev) => ShowMainWindow());
            menu.Items.Add("종료 (Exit)", null, (s, ev) => ExitApp());
            _notifyIcon.ContextMenuStrip = menu;

            // 4. 더블 클릭 시 창 열기
            _notifyIcon.DoubleClick += (s, ev) => ShowMainWindow();

            // 처음 시작할 때 창을 띄움
            _mainWindow.Show();
        }

        private void ShowMainWindow()
        {
            _mainWindow.Show();
            _mainWindow.Activate();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void ExitApp()
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
    }
}