using Cognex.InSight;
using Cognex.InSight.Controls.Display;
using Cognex.InSight.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InSight_Manager.Model
{
    public class ConnectModel
    {
        public bool ConnectToEmulator(CvsInSight IsInSightSensor , string address)
        {
            try
            {
                if (IsInSightSensor == null) IsInSightSensor = new CvsInSight();

                // 1. CvsInSight의 Connect는 매개변수가 3개입니다. (IP, ID, PW)
                // 2. 에뮬레이터는 기본적으로 "admin", ""(공백)을 사용합니다.
                IsInSightSensor.Connect(address, "admin", "", true, false);

                // 연결 상태를 반환합니다.
                return true;
            }
            catch(Exception ex)

            {
                MessageBox.Show($"{ex}");
                return false;
            }

        }

    }
}
