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

        private string _image;
        int currentIndex = 0;

        public string SelectFolder(string SelectedFolderPath)
        {
            // 1. 다이얼로그 인스턴스 생성 (using을 써서 사용 후 바로 메모리 해제)
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                // 창 상단에 뜰 설명 텍스트
                dialog.Description = "이미지가 저장된 폴더를 선택하세요";

                // '새 폴더 만들기' 버튼 숨기기 (단순 선택용이면 false 추천)
                dialog.ShowNewFolderButton = false;

                // 2. 창 띄우기 (사용자가 '확인'을 눌렀는지 체크)
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                // 3. 결과 처리
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // [중요] 사용자가 선택한 경로가 이 변수에 담깁니다.
                    return SelectedFolderPath = dialog.SelectedPath;
                }

                return SelectedFolderPath;
            }
        }



        public List<string> GetImageFiles(string folderPath)
        {

            if(folderPath != null)
            {
                // 1. 지원할 확장자 정의 (필요한 거 더 추가하세요)
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".bmp" };

                // 2. 해당 폴더의 모든 파일을 가져와서 -> 확장자가 맞는 것만 골라냄
                var files = Directory.GetFiles(folderPath)
                                     .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                     .ToList();

                return files; // 이미지 파일 경로들이 담긴 리스트 반환
            }
            return null;

        }

        public String Showiamge(List<string> files)
        {
            if (files.Count > 0)
            {
                _image = files[currentIndex];
            }

            return _image;

        }
        public String NextImage(List<string> files)
        {
            if (files.Count-1 > currentIndex)
            {
                currentIndex += 1;

                _image = files[currentIndex];

                return _image;
            }
            else
            {
                currentIndex = 0;

                _image = files[currentIndex];

                return _image;
            }


            }

        public String PrevImage(List<string> files)
        {
            if(currentIndex > 0)
            {
                currentIndex -= 1;

                _image = files[currentIndex];

                return _image;
            }
            else
            {
                currentIndex = files.Count - 1;

                _image = files[currentIndex];

                return _image;
            }
        }

        public BitmapImage LoadBitmap(string filePath)
        {
            BitmapImage bitmap = new BitmapImage();

            if (filePath != null)
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // 메모리에 올리고 파일 연결 끊기
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;

        }

        public async Task FillFilmstripAsync(string folderPath, ObservableCollection<string> FilmstripImages)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;

            // 1. 기존 리스트 비우기 (UI 스레드)
            FilmstripImages.Clear();

            // 2. 파일 목록을 가져오는 작업은 무거울 수 있으니 비동기로 진행
            await Task.Run(() =>
            {
                try
                {
                    string extensions = ".bmp";

                    // 폴더 내 파일 검색 및 필터링
                    var files = Directory.EnumerateFiles(folderPath)
                                         .Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
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
    }
}
