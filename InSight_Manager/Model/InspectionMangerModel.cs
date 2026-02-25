using Cognex.InSight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.Model
{
    public class InspectionMangerModel
    {
        private readonly string _rootPath = @"C:\VisionLog";

        private string GetBasePath(DateTime now)
        {
            return Path.Combine(_rootPath, now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
        }

        public async Task ExecuteSaveResult(string modelName, bool isPass, DateTime now)
        {
            await Task.Run(() =>
            {
                try
                {
                    string basePath = GetBasePath(now);
                    Directory.CreateDirectory(basePath);

                    string csvPath = Path.Combine(basePath, "Inspection_Result.csv");
                    if (!File.Exists(csvPath))
                        File.WriteAllText(csvPath, "Time,Model,Result\n", Encoding.UTF8);

                    File.AppendAllText(csvPath, $"{now:HH:mm:ss},{modelName},{(isPass ? "OK" : "NG")}\n");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"저장 중 오류 발생: {ex.Message}");
                }
            });
        }

        public async Task<string> ExecuteSaveImage(CvsImage image, string pos, bool isPass,DateTime now)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string basePath = GetBasePath(now);
                    string reuslt = isPass ? "OK" : "NG";
                    string imgFolder = Path.Combine(basePath, reuslt);
                    Directory.CreateDirectory(imgFolder);

                    string fileName = $"{pos}_{reuslt}_{now:HH시mm분ss초}.bmp";

                    image.Save(Path.Combine(imgFolder, fileName));
                    return Path.Combine(imgFolder, fileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"저장 중 오류 발생: {ex.Message}");
                }


            });
        }

        public async Task<string> PrepareImageSavePath(string modelName, bool isPass, DateTime now)
        {
            return await Task.Run(() =>
            {
                // 1. 날짜/결과별 폴더 경로 계산 및 생성
                string basePath = Path.Combine(_rootPath, now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
                string imgFolder = Path.Combine(basePath, isPass ? "OK" : "NG");
                Directory.CreateDirectory(imgFolder); // 폴더가 없으면 생성

                // 2. 최종 파일명 조합 및 반환
                string fileName = $"{now:HHmmss_fff}_{modelName}.bmp";
                return Path.Combine(imgFolder, fileName);
            });
        }

    }
}
