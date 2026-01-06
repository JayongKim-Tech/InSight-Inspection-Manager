using InSight_Manager.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.Model
{
    public class JobFileManagerModel
    {
        private OpenFileDialog openDlg = new OpenFileDialog();
        private SaveFileDialog saveDlg = new SaveFileDialog();


        public string OpenJobDlg()
        {

            // 2. 필터 설정 (사용자가 엄한 파일 못 고르게 .job만 보여줌)
            openDlg.Filter = "In-Sight Job Files (*.job)|*.job|All Files (*.*)|*.*";
            openDlg.Title = "In-Sight 잡(Job) 파일 선택"; // 창 제목

            // 3. 창 띄우기 (ShowDialog 호출)
            // true가 리턴되면 사용자가 파일을 선택하고 [열기]를 눌렀다는 뜻입니다.
            if (openDlg.ShowDialog() == false)
            {
                return null; // 선택한 파일의 전체 경로 (C:\...\Test.job)
            }

            return openDlg.FileName;
        }


        public string SaveJobDlg()
        {
            saveDlg.Filter = "In-Sight Job Files (*.job)|*.job";
            saveDlg.Title = "잡(Job) 파일 다른 이름으로 저장";
            saveDlg.FileName = "NewJob.job"; // 기본 파일명

            if (saveDlg.ShowDialog() == false)
            {
                return null;
            }

            return saveDlg.FileName;
        }
    }
}
