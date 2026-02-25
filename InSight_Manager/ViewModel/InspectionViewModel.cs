using Cognex.InSight;
using Cognex.InSight.Extensions;
using InSight_Inspection_Manager;
using InSight_Manager.Model;
using InSight_Manager.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.ViewModel
{
    public class InspectionViewModel : ViewModelBase
    {

        private IDisplayController _displayController;


        public IDisplayController DisplayController
        {
            get => _displayController;
            set
            {
                if (_displayController != null) _displayController.InspectionFinished -= OnInspectionFinished;

                _displayController = value;
                OnPropertyChanged();

                if (_displayController != null) _displayController.InspectionFinished += OnInspectionFinished;

            }
        }

        private async void OnInspectionFinished(object sender, EventArgs e)
        {
            var sensor = DisplayController?.InSightSensor;
            var display = DisplayController?.InSightDisplay;
            if (sensor == null || sensor.Results == null) return;

            try
            {
                DateTime now  = DateTime.Now;
                string resultCell = display.Results.Cells["A10"].ToString();
                string modelName = display.Results.Cells["B3"].ToString();
                string position = display.Results.Cells["D9"].ToString();
                bool isPass = (resultCell == "1");
                CvsImage img = display.Results.Image;


                await ModelHub.Instance.inspectionMangerModel.ExecuteSaveResult(modelName, isPass, now);
                await ModelHub.Instance.inspectionMangerModel.ExecuteSaveImage(img, position, isPass, now);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"데이터 추출 실패: {ex.Message}");
            }
        }

    }
}
