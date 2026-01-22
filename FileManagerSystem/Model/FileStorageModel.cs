using FileManagerSystem.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerSystem.Model
{
    public class FileStorageModel
    {
        LogViewModel logViewModel = new LogViewModel();
        private FileSystemWatcher _watcher;

        // 파일 이동이 완료되었을 때 ViewModel에 알리기 위한 이벤트
        public event Action<string> OnFileArchived;

        #region 실시간 감시 (Watcher) 제어

        public void StartWatcher(string sourcePath, string targetRootPath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !Directory.Exists(sourcePath)) return;

            // 이미 실행 중이면 중지 후 새로 시작
            StopWatcher();

            _watcher = new FileSystemWatcher(sourcePath)
            {
                IncludeSubdirectories = true,
                Filter = "*.*", // 모든 파일 감시 (필요 시 *.bmp로 제한)
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            // 파일 생성 이벤트 연결
            _watcher.Created += async (s, e) =>
            {
                // [중요] 비전 시스템이 파일 저장을 완료할 시간을 벌어주기 위해 0.5초 대기
                await Task.Delay(500);

                // 파일 이동 실행
                bool success = ProcessSingleFile(e.FullPath, sourcePath, targetRootPath);

                if (success)
                {
                    // 이동 성공 시 이벤트 호출 (ViewModel에서 용량 갱신용)
                    OnFileArchived?.Invoke(e.FullPath);
                }
            };

            _watcher.EnableRaisingEvents = true;
        }

        public void StopWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        #endregion

        #region 파일 처리 핵심 로직

        // 단일 파일 이동 로직 (Watcher와 ArchiveFiles 공용)
        private bool ProcessSingleFile(string filePath, string sourceRoot, string targetRoot)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                if (!file.Exists) return false;

                // 1. 파일명 추출 (확장자 제외): "01-21-2026 15.06.31.808_OK_1_TP1"
                string fileNameOnly = Path.GetFileNameWithoutExtension(file.Name);

                // 2. 구분자('_'와 ' ')를 기준으로 분리
                // parts[0] = "01-21-2026", parts[1] = "15.06.31.808", parts[2] = "OK", parts[3] = "1"
                string[] parts = fileNameOnly.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 4) return false; // 형식이 맞지 않으면 스킵

                // 3. 날짜 분리 ("01-21-2026" -> 월, 일, 연도)
                string[] dateParts = parts[0].Split('-');
                string month = dateParts[0];
                string day = dateParts[1];
                string year = dateParts[2];

                // 4. 나머지 정보 매핑
                string timePart = parts[1]; // "15.06.31.808"
                string resultPart = parts[2]; // "OK" 또는 "NG"
                string indexPart = parts[3];  // "1"
                string camName = parts[4]; // "TP2"

                // 5. 타겟 경로 조립: 2026 \ 01 \ 21 \ TP1 \ OK \ 1
                string finalDestDir = Path.Combine(targetRoot, year, month, day, camName, resultPart, indexPart);

                // 폴더 생성
                if (!Directory.Exists(finalDestDir)) Directory.CreateDirectory(finalDestDir);

                // 6. 최종 파일명 결정: 시간.bmp (예: 15.06.31.808.bmp)
                string newFileName = timePart + file.Extension;
                string destFilePath = Path.Combine(finalDestDir, newFileName);

                // 7. 이동 실행
                if (!File.Exists(destFilePath))
                {
                    file.MoveTo(destFilePath);
                }
                else
                {
                    // 동일 시간 파일 존재 시 중복 방지용 처리
                    string dupName = timePart + "_" + DateTime.Now.ToString("ssff") + file.Extension;
                    file.MoveTo(Path.Combine(finalDestDir, dupName));
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"파일명 분류 에러: {ex.Message}");
                return false;
            }
        }

        // 전체 아카이빙 (기존 기능 유지)
        public void ArchiveFiles(string sourcePath, string targetRootPath)
        {
            if (!Directory.Exists(sourcePath) || !Directory.Exists(targetRootPath)) return;

            try
            {
                DirectoryInfo sourceInfo = new DirectoryInfo(sourcePath);
                var files = sourceInfo.GetFiles("*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    ProcessSingleFile(file.FullName, sourcePath, targetRootPath);
                }
            }
            catch (Exception ex)
            {
                logViewModel.AddLog("전체 아카이브 에러: " + ex.Message);
            }
        }

        // 파일 잠김 확인 (비전 이미지 저장 중인지 체크)
        private bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true; // 파일이 아직 다른 프로세스(비전 센서 등)에 의해 사용 중임
            }
            return false;
        }

        #endregion

        #region 용량 관리 로직 (기존 유지)

        public long GetDirectorySize(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            }
            catch { return 0; }
        }

        public bool DeleteOldestDateFolder(string targetRootPath)
        {
            try
            {
                if (!Directory.Exists(targetRootPath)) return false;
                DirectoryInfo rootDir = new DirectoryInfo(targetRootPath);
                var oldestYearDir = rootDir.GetDirectories().OrderBy(d => d.Name).FirstOrDefault();
                if (oldestYearDir != null)
                {
                    var oldestDateDir = oldestYearDir.GetDirectories().OrderBy(d => d.Name).FirstOrDefault();
                    if (oldestDateDir != null)
                    {
                        oldestDateDir.Delete(true);
                        if (oldestYearDir.GetDirectories().Length == 0) oldestYearDir.Delete();
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        #endregion
    }
}