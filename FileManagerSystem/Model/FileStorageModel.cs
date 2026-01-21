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

                // 파일이 사용 중인지 체크 (잠겨있으면 스킵)
                if (IsFileLocked(file)) return false;

                string yearFolder = file.LastWriteTime.ToString("yyyy");
                string dateFolder = file.LastWriteTime.ToString("MM-dd");

                // 상대 경로 유지 (예: front\good)
                string relativePath = file.DirectoryName.Replace(sourceRoot, "")
                                                     .TrimStart(Path.DirectorySeparatorChar);

                string finalDestDir = Path.Combine(targetRoot, yearFolder, dateFolder, relativePath);
                if (!Directory.Exists(finalDestDir)) Directory.CreateDirectory(finalDestDir);

                string destFile = Path.Combine(finalDestDir, file.Name);

                if (!File.Exists(destFile))
                {
                    file.MoveTo(destFile);
                }
                else
                {
                    // 중복 파일명 처리
                    string newName = Path.GetFileNameWithoutExtension(file.Name)
                                     + "_" + DateTime.Now.ToString("HHmmssff")
                                     + Path.GetExtension(file.Name);
                    file.MoveTo(Path.Combine(finalDestDir, newName));
                }
                return true;
            }
            catch
            {
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