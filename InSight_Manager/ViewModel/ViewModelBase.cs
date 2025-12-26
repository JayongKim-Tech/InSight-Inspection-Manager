using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InSight_Inspection_Manager
{
    // 이 클래스는 '화면에 값을 알리는 기능'만 전문적으로 담당합니다.
    public class ViewModelBase : INotifyPropertyChanged
    {
        // 화면과 연결된 '종' (이벤트)
        public event PropertyChangedEventHandler PropertyChanged;

        // 종을 울리는 '행위' (함수)
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // 나를 지켜보고 있는 화면이 있다면, "값이 바뀌었어!"라고 알려줌
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}