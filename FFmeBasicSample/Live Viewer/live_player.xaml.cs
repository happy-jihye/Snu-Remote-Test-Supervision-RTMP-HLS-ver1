using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FFmePlayer_snu.Controls
{
    /// <summary>
    /// Player.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class live_player
    {

        /* 
        * Live player는 rtmp 프로토콜을 통해 받아온 주소를 재생하는 player입니다.
        * 실시간으로 스트리밍 되고 있는 영상을 띄우는 player이므로 play, pause등의 기능은 구현하지 않았고,
        * mute기능만을 추가하였습니다.
        */

        public string m_uri { get; set; }
        bool mute_btn = true;

        public live_player()
        {
            InitializeComponent();
            Media.Volume = 0;
        }

        private void media_loaded(object sender, RoutedEventArgs e)
        {
            if (m_uri != null)
            {
                Media.Source = new Uri(m_uri);
                Media.Open(Media.Source);
            }
        }

        public void media_play()
        {
            Media.Source = new Uri(m_uri);
            if (Media.IsOpen)
                Media.Open(Media.Source);
        }

        private void Volume_Click(object sender, RoutedEventArgs e)
        {

            if (mute_btn == true) // mute->sound
            {
                mute_btn = false;
                Volume.Content = "🔇";
                Media.Volume = 1;
            }
            else
            {
                mute_btn = true;
                Volume.Content = "🔊";
                Media.Volume = 0;
            }
        }


    }
}
