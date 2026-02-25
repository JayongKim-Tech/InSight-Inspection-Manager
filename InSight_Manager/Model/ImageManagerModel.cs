using Cognex.InSight;
using Cognex.InSight.Controls.Display;
using Cognex.InSight.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InSight_Manager.Model
{
    public class ImageManagerModel
    {

        public string SelectFolder(string SelectedFolderPath)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "이미지가 저장된 폴더를 선택하세요";

                dialog.ShowNewFolderButton = false;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                // 3. 결과 처리
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return SelectedFolderPath = dialog.SelectedPath;
                }

                return SelectedFolderPath;
            }
        }


        public async Task FillFilmstripAsync(string folderPath, ObservableCollection<string> FilmstripImages)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;

            FilmstripImages.Clear();

            // 2. 파일 목록을 가져오는 작업은 무거울 수 있으니 비동기로 진행
            await Task.Run(() =>
            {
                try
                {
                    string[] allowedExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };

                    // 폴더 내 파일 검색 및 필터링
                    var files = Directory.EnumerateFiles(folderPath)
                                         .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                         .OrderBy(file => File.GetCreationTime(file)) // 생성 순서대로
                                         .ToList();

                    // 3. 찾은 파일들을 UI 리스트에 추가
                    // ObservableCollection은 UI 스레드에서 수정해야 하므로 Dispatcher를 사용합니다.
                    foreach (var file in files)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            FilmstripImages.Add(file);
                        });
                    }
                }
                catch (Exception ex)
                {
                    // [불확실함]:
                    // 폴더 접근 권한 문제나 다른 프로그램이 파일을 사용 중일 때 예외가 발생할 수 있습니다.
                    System.Diagnostics.Debug.WriteLine($"파일 목록 로드 실패: {ex.Message}");
                }
            });
        }


        public async Task RunBatchProcessAsync(List<string> files, CvsInSight insightSensor, string resultFilePath, CvsInSightDisplay display)
        {
            if (files == null || files.Count == 0 || insightSensor == null) return;

            List<string> ngList = new List<string>();


            await Task.Run(() =>
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string currentFile = files[i];
                    try
                    {
                        display.Edit.OpenImageFromFile(currentFile);

                        insightSensor.Results.GetImage(0);


                        System.Threading.Thread.Sleep(50);

                        var resultCell = insightSensor.Results.Cells["G67"];

                        // 5. 조건 확인 (0이면 NG/False로 간주)
                        if (resultCell != null && resultCell.ToString() == "0")
                        {
                            ngList.Add($"{Path.GetFileName(currentFile)} : NG (Value: {resultCell})");

                        }
                    }
                    catch (Exception ex)
                    {
                        ngList.Add($"{Path.GetFileName(currentFile)} : Error ({ex.Message})");
                    }

                    // 진행률을 로그나 UI에 출력하고 싶다면 여기서 Dispatcher.Invoke 사용
                }

                // 6. 결과 파일(텍스트) 생성 및 저장
                SaveResultsToFile(ngList, resultFilePath);
            });
        }

        private void SaveResultsToFile(List<string> ngList, string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"--- NG 이미지 리스트 ({DateTime.Now}) ---");
                sb.AppendLine($"총 검사 수 중 NG 발견 건수: {ngList.Count}");
                sb.AppendLine("-----------------------------------------");

                foreach (var line in ngList)
                {
                    sb.AppendLine(line);
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"결과 저장 실패: {ex.Message}");
            }
        }

        public void GetImagePath(CvsInSightDisplay sensor)
        {

        }

    }
}
