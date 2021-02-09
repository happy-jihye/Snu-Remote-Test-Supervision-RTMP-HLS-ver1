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
using System.ComponentModel;
using System.Net;
using System.Net.Http;

namespace FFmePlayer_snu.Controls
{
    /// <summary>
    /// live_viewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class live_tab : Page
    {
        /* 
        * Live Tab
        * 서버와의 통신 프로토콜을 통해 rtmp 주소를 받아오는 부분입니다. refresh button을 누르면 주소를 받을 수 있습니다. 
        * 또한, home으로 가거나 프로그램을 종료하는 경우에는 deactivation 명령어를 보내어 시험을 비활성화 하도록 구현하였습니다.
        */

        string lecture;
        string test;
        string date;
        string token;
        string rtmp_uri;

        public live_tab(string parm_lecture, string parm_test, string parm_date, string parm_token)
        {
            InitializeComponent();

            lecture = parm_lecture;
            test = parm_test;
            date = parm_date;
            token = parm_token;

            mainWindow.Closing += page_closed;
        }

        
        // 1) refresh button을 통해 rtmp 주소를 받아올 수 있습니다.
       
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            //curl -X POST http://XXX/superv_endpoint -d lec_id=logicdesign.midterm_20210108 -d supervNum=1 -d token=

            string URI = "http://XXX/superv_endpoint";
            string myParameters = "lec_id=" + lecture + "." + test + "_" + date + "&supervNum=" + superv_num.Text + "&token=" + token;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                rtmp_uri = webClient.UploadString(URI, myParameters);
            }

            if(rtmp_uri != "null")
            {
                myViewer.live_viewer_loaded(rtmp_uri);
            }
        }


        // 2) 이 페이지에서 나갈 경우 시험이 deactivation됩니다.

        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }

        private void home_Click(object sender, RoutedEventArgs e)
        {
            //curl - X POST http://XXX/exam_deactivation -d lec_id=sf.midterm_20210112 -d token=

            string URI = "http://XXX/exam_deactivation";
            string myParameters = "lec_id=" + lecture + "." + test + "_" + date + "&supervNum=" + superv_num.Text + "&token=" + token;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.UploadString(URI, myParameters);
            }

            Main_Page main_Page = new Main_Page(token);
            mainWindow.mainFrame.Navigate(main_Page);
        }

        private void page_closed(object sender, CancelEventArgs e)
        {
            if(MessageBox.Show("정말 종료하시겠습니까?", "종료", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                string URI = "http://XXX/exam_deactivation";
                string myParameters = "lec_id=" + lecture + "." + test + "_" + date + "&supervNum=" + superv_num.Text + "&token=" + token;

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    webClient.UploadString(URI, myParameters);
                }
            }

        }
    }
}
