using FileManagerSystem.ViewModel;
using System;
using System.IO;
using System.Linq;

namespace FileManagerSystem.Model
{
    public class FileStorageModel
    {
        LogViewModel logViewModel = new LogViewModel();
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

        // 2. 가장 오래된 파일 삭제 및 삭제된 크기 반환
        public bool DeleteOldestDateFolder(string targetRootPath)
        {
            try
            {
                if (!Directory.Exists(targetRootPath)) return false;

                DirectoryInfo rootDir = new DirectoryInfo(targetRootPath);

                // 1. 가장 오래된 연도 폴더 찾기
                var oldestYearDir = rootDir.GetDirectories().OrderBy(d => d.Name).FirstOrDefault();
                if (oldestYearDir != null)
                {
                    // 2. 그 안에서 가장 오래된 날짜 폴더 찾기
                    var oldestDateDir = oldestYearDir.GetDirectories().OrderBy(d => d.Name).FirstOrDefault();
                    if (oldestDateDir != null)
                    {
                        oldestDateDir.Delete(true); // 폴더 통째로 삭제

                        // 3. 연도 폴더가 비었으면 연도 폴더도 삭제
                        if (oldestYearDir.GetDirectories().Length == 0)
                        {
                            oldestYearDir.Delete();
                        }
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        // 3. 날짜별 폴더 생성 및 파일 이동 (아카이빙)
        public void ArchiveFiles(string sourcePath, string targetRootPath)
        {
            if (!Directory.Exists(sourcePath) || !Directory.Exists(targetRootPath)) return;

            try
            {
                DirectoryInfo sourceInfo = new DirectoryInfo(sourcePath);
                // 오늘 생성된 파일을 제외한 모든 파일 대상
                var files = sourceInfo.GetFiles("*", SearchOption.AllDirectories)
                                      .Where(f => f.LastWriteTime.Date < DateTime.Now.Date);

                foreach (var file in files)
                {
                    string yearFolder = file.LastWriteTime.ToString("yyyy");
                    string dateFolder = file.LastWriteTime.ToString("MM-dd");

                    // 원본의 상대 경로 추출 (예: front\good)
                    string relativePath = file.DirectoryName.Replace(sourceInfo.FullName, "")
                                             .TrimStart(Path.DirectorySeparatorChar);

                    // 타겟 경로 조립
                    string finalDestDir = Path.Combine(targetRootPath, yearFolder, dateFolder, relativePath);

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
                }
            }
            catch (Exception ex) { logViewModel.AddLog("아카이브 상세 분류 에러: " + ex.Message); }
        }


    }
}