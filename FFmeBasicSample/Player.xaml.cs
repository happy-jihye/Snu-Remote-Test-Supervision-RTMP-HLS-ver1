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
    public partial class Player 
    {

        //string Uri { get; set; }
        private string m_uri;
        public Player()
        {
            InitializeComponent();
        }
        public void get_uri(string uri)
        {
            m_uri = uri;
            Media.Source = new Uri(m_uri);
            Media.Open(Media.Source);
        }
        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!Media.IsOpen)
            {
                if (m_uri != null)
                    Media.Source = new Uri(m_uri);
                await Media.Open(Media.Source);
            }
            else
            {
                await Task.Delay(Media.Position);
                await Media.Play();
            }
        }
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source == null)
                return;
            Media.Pause();

        }
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Media.Source == null)
                return;
            Media.Stop();
        }

    }
}
