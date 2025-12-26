using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InSight_Manager.Model
{
    public class ConnectModel
    {
        private TcpClient _client;
        private NetworkStream _stream;

        // [퀴즈 1] 장치와 물리적으로 연결하는 함수입니다.
        public bool Connect(string ip, int port = 23)
        {

            _client = new TcpClient();

            try
            {
                // 1. 새로운 TCP 통로를 생성합니다.
                // 힌트: ip와 port를 인자로 받는 객체를 생성하세요.
                _client.Connect(ip,port);

                // 2. 데이터를 실어 나를 '스트림'을 가져옵니다.
                // 힌트: _client 객체로부터 스트림을 뽑아냅니다.
                _stream = _client.GetStream();

                return _client.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // [퀴즈 2] 텔넷은 글자를 보내야 합니다. 스트림에 데이터를 쏘는 기본 함수입니다.
        public void SendRawData(string message)
        {
            if (_stream == null) return;

            // 문자열을 바이트 배열로 변환 (텔넷은 보통 ASCII 사용)
            byte[] data = Encoding.ASCII.GetBytes(message + "\r\n");

            // 힌트: _stream에 data를 처음부터 끝까지 쓰세요(Write).
            _stream.Write(data, 0, data.Length);
        }

    }
}
