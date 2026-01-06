using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.ViewModel
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




    }
}
