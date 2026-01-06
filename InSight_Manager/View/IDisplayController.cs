using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.View
{
    public interface IDisplayController
    {
        void SetZoomIn(double scale);
        void SetZoomOut(double scale);
        void SetGrid(bool show);
        void SetGraphics(bool show);
        void FitImage();
        void SaveJob(string filename);
        void OpenJob(string filename);
        void NewJob();

        void IsCustomView(bool show);
        void IsGraphicView(bool show);
        void IsOnline(bool online);

        //void SetFilmstrip(bool show);      // 하단 슬라이드 (필름스트립)
        //void ShowRecordOptions();          // 레코드 옵션 창 띄우기


    }
}
