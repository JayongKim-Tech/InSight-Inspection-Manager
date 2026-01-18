using FileManagerSystem.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace FileManagerSystem.ViewModel
{
    public class StorageViewModel : ViewModelBase
    {
        private readonly FileStorageModel _fileModel = new FileStorageModel();
        private DateTime _lastArchiveDate = DateTime.Now.Date;


        public LogViewModel logVm { get; } = new LogViewModel();

        // 1. 필드 (내부 데이터)
        private string _sourcePath;
        private string _targetPath;
        private double _currentCapacityGB;
        private double _maxCapacityGB = 100; // 기본 제한 100GB
        private double _capacityPercent;
        private DispatcherTimer _timer;


        // 2. 프로퍼티 (UI 바인딩용)

        private double _cleanupBufferGB = 10; // 기본값 10GB
        public double CleanupBufferGB
        {
            get => _cleanupBufferGB;
            set { _cleanupBufferGB = value; OnPropertyChanged(); }
        }
        public string SourcePath
        {
            get => _sourcePath;
            set { _sourcePath = value; OnPropertyChanged(); }
        }

        public string TargetPath
        {
            get => _targetPath;
            set { _targetPath = value; OnPropertyChanged(); }
        }

        public double CurrentCapacityGB
        {
            get => _currentCapacityGB;
            set { _currentCapacityGB = value; OnPropertyChanged(); UpdatePercent(); }
        }

        public double MaxCapacityGB
        {
            get => _maxCapacityGB;
            set { _maxCapacityGB = value; OnPropertyChanged(); UpdatePercent(); }
        }

        public double CapacityPercent
        {
            get => _capacityPercent;
            set { _capacityPercent = value; OnPropertyChanged(); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                _isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EngineText));
                OnPropertyChanged(nameof(EngineColor));
            }
        }

        // 버튼에 표시될 텍스트
        public string EngineText => IsRunning ? "STOP ENGINE" : "START ENGINE";

        // 버튼 색상 (달리기 중이면 빨간색/주황색, 멈췄으면 흰색/파란색)
        public string EngineColor => IsRunning ? "#FF3B30" : "White"; // iOS 스타일 Red vs White

        // 3. 커맨드 (버튼 연결용)
        public ICommand SelectSourceCommand { get; }
        public ICommand SelectTargetCommand { get; }
        public ICommand ManualArchiveCommand { get; }
        public ICommand ToggleEngineCommand { get; }



        // 4. 생성자
        public StorageViewModel()
        {
            ToggleEngineCommand = new RelayCommand(o =>
            {
                // 1. 상태 반전
                this.IsRunning = !this.IsRunning;

                // 2. 로그 남기기
                string status = IsRunning ? "시작" : "중지";
                logVm.AddLog($"[시스템] 모니터링 엔진이 {status}되었습니다.");

                // 3. (디버깅용) 만약 로그도 안 찍히면 이 메시지 박스가 뜨는지 보세요.
                // System.Windows.MessageBox.Show($"엔진 상태: {status}");
            });


            IsRunning = false;
            // 커맨드 초기화
            SelectSourceCommand = new RelayCommand(o => BrowseFolder("Source"));
            SelectTargetCommand = new RelayCommand(o => BrowseFolder("Target"));

            ManualArchiveCommand = new RelayCommand(async o =>
            {
                logVm.AddLog("[수동 실행] 사용자가 백업을 직접 시작했습니다.");
                await RunArchiving();
            });

            // 2초마다 용량을 체크하는 타이머 설정
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += async (s, e) =>
            {
                if (IsRunning)
                {
                    await RefreshCapacityAsync(); // 1. 용량 체크 및 삭제
                    await CheckDailyArchive();    // 2. 날짜 변경 체크 및 이동
                }
            };
            _timer.Start();
            logVm.AddLog("시스템이 시작되었습니다. 모니터링 중...");
        }

        private async Task CheckDailyArchive()
        {
            // 현재 날짜가 마지막 작업 날짜보다 뒤라면 (즉, 자정이 지났다면)
            if (DateTime.Now.Date > _lastArchiveDate)
            {

                logVm.AddLog($"[날짜 변경] {DateTime.Now:yyyy-MM-dd} 자정 백업을 시작합니다.");
                // 로그를 남기고 싶다면 여기서 호출 (예: "일일 백업을 시작합니다.")
                await RunArchiving();

                // 작업 완료 후 오늘 날짜로 갱신
                _lastArchiveDate = DateTime.Now.Date;
            }
        }

        // 5. 폴더 브라우저 실행 (WinForms 라이브러리 활용)
        private void BrowseFolder(string type)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = type == "Source" ? "감시할 폴더를 선택하세요." : "백업될 폴더를 선택하세요.";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (type == "Source")
                    {
                        SourcePath = dialog.SelectedPath;
                        logVm.AddLog($"감시 경로 설정: {SourcePath}");
                        _ = RefreshCapacityAsync(); // 경로 변경 시 즉시 계산
                    }
                    else
                    {
                        TargetPath = dialog.SelectedPath;
                        logVm.AddLog($"백업 경로 설정: {TargetPath}");
                    }
                }
            }
        }

        // 비동기 로직 계산
        public async Task RefreshCapacityAsync()
        {
            // 1. 소스 폴더는 경로가 있는지 체크만 (이동을 위해)
            if (string.IsNullOrEmpty(SourcePath)) return;

            // 2. ★ 핵심: 타겟 폴더의 용량을 계산해서 화면에 표시
            if (!string.IsNullOrEmpty(TargetPath) && Directory.Exists(TargetPath))
            {
                long targetBytes = await Task.Run(() => _fileModel.GetDirectorySize(TargetPath));

                // 현재 타겟 용량을 GB로 변환하여 프로퍼티에 저장 (게이지 바와 연결됨)
                CurrentCapacityGB = targetBytes / (1024.0 * 1024.0 * 1024.0);

                // 3. 설정한 제한 용량을 넘으면 가장 오래된 날짜 폴더 삭제
                if (CurrentCapacityGB > MaxCapacityGB)
                {
                    logVm.AddLog($"[경고] 타겟 용량 초과! ({CurrentCapacityGB:N2}GB / {MaxCapacityGB}GB)");
                    await RunTargetCleanup();
                }
            }
        }

        private async Task RunTargetCleanup()
        {
            await Task.Run(() =>
            {
                double currentGB = CurrentCapacityGB;

                // 용량이 제한치 아래로 내려올 때까지 가장 오래된 날짜 폴더를 하나씩 삭제
                while (currentGB > MaxCapacityGB- _cleanupBufferGB)
                {
                    bool success = _fileModel.DeleteOldestDateFolder(TargetPath);
                    if (!success) break;

                    // 삭제 후 용량 다시 계산
                    long newBytes = _fileModel.GetDirectorySize(TargetPath);
                    currentGB = newBytes / (1024.0 * 1024.0 * 1024.0);
                }

                // 최종 결과 업데이트
                CurrentCapacityGB = currentGB;
                logVm.AddLog($"[정리 완료] 오래된 날짜 폴더를 삭제했습니다.");

            });
        }
        public async Task RunArchiving()
        {
            if (string.IsNullOrEmpty(SourcePath) || string.IsNullOrEmpty(TargetPath)) return;

            logVm.AddLog("데이터 이동을 시작합니다...");
            await Task.Run(() =>
            {
                _fileModel.ArchiveFiles(SourcePath, TargetPath);
            });

            logVm.AddLog("데이터 이동이 완료되었습니다.");
            // 이동 후 용량 다시 계산
            await RefreshCapacityAsync();
        }


        // 8. 프로그레스바 퍼센트 업데이트
        private void UpdatePercent()
        {
            if (MaxCapacityGB > 0)
            {
                double percent = (CurrentCapacityGB / MaxCapacityGB) * 100;
                // 100%를 넘지 않도록 제한
                CapacityPercent = percent > 100 ? 100 : percent;
            }
        }



    }
}