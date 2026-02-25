using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.Model
{
    public class ModelHub
    {

        private static readonly ModelHub _instance = new ModelHub();
        public static ModelHub Instance => _instance;

        public CellResultCheckModel cellResultCheckModel { get; }
        public ConnectModel connectModel { get; }
        public ImageManagerModel imageManagerModel { get; }
        public InspectionMangerModel inspectionMangerModel { get; }
        public JobFileManagerModel jobFileManagerModel { get; }

        private ModelHub()
        {
            cellResultCheckModel = new CellResultCheckModel();
            connectModel = new ConnectModel();
            imageManagerModel = new ImageManagerModel();
            inspectionMangerModel = new InspectionMangerModel();
            jobFileManagerModel = new JobFileManagerModel();
        }



    }
}
