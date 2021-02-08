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
using System.Windows.Threading;

namespace FFmePlayer_snu.Controls
{
    /// <summary>
    /// live_viewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class live_viewer
    {

        /* 
        * Live Viewer는 여러개의 RTMP 주소를 받으면,
        * Live player를 통해 여러개의 영상을 띄우는 부분입니다.
        * 
        * 주소의 끝부분에는 초기접속 / 재접속 여부를 뜻하는 정보가(0, 1) 함께 들어오는데, 
        * 이를 통하여 처음 접속하면 영상을 viewer에 추가하고,
        * 재접속을 하면 영상이 새로고침되도록 구현하였습니다.
        */


        List<string> uri_list = new List<string>();
        List<live_player> streaming_player_list = new List<live_player>();
        List<Button> Buttons = new List<Button>();

        int num;

        public live_viewer()
        {
            InitializeComponent();
        }

        public void live_viewer_loaded(string m_uri_string)
        {
            // 여러개의 주소를 배열에 저장
            string[] uri_split = m_uri_string.Split(' ');
            num = uri_split.Length;

            for (int i = 0; i < uri_split.Length; i++)
            {
                string[] uri = uri_split[i].Split('_');
                // 초기접속 or 재접속
                if (uri[1] == "1")
                {
                    bool exist = false;
                    for (int k = 0; k < uri_list.Count; k++)
                    {
                        // 재접속 경우 : 새로고침
                        if (uri[0] == uri_list[k])
                        {
                            exist = true;
                            live_player streaming_player = new live_player();

                            streaming_player.m_uri = uri[0];
                            streaming_player.media_play();

                            myLivePlayers.Items.Remove(streaming_player_list[k]);
                            myLivePlayers.Items.Add(streaming_player);

                            break;
                        }
                    }
                    // 초기접속인 경우 : viewer에 player를 추가
                    if(exist == false)
                    {
                        live_player streaming_player = new live_player();

                        streaming_player.m_uri = uri[0];
                        streaming_player.media_play();

                        uri_list.Add(uri[0]);
                        myLivePlayers.Items.Add(streaming_player);
                        streaming_player_list.Add(streaming_player);

                    }

                }

                double_click();
                myLivePlayers_btn.ItemsSource = Buttons;
            }
        }

        // double click을 하면 화면이 확대되도록 button을 구현
        private void double_click()
        {
            for (int i = 0; i < num; i++)
            {
                Button btn = new Button();
                btn.BorderBrush = Brushes.Transparent;
                btn.Background = Brushes.Transparent;
                btn.Tag = i;
                btn.MouseDoubleClick += btn_click;
                Buttons.Add(btn);
            }
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (zoom_live_player.Visibility == Visibility.Hidden)
            {
                zoom_live_player.Visibility = Visibility.Visible;
                myLivePlayers.Visibility = Visibility.Hidden;
                int k = (int)btn.Tag;
                zoom_live_player.m_uri = streaming_player_list[k].m_uri;
                zoom_live_player.media_play();
            }
            else
            {
                zoom_live_player.Visibility = Visibility.Hidden;
                myLivePlayers.Visibility = Visibility.Visible;
            }
        }

    }
}