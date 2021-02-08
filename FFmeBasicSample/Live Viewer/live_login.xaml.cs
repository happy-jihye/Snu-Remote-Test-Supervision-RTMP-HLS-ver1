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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Http;

namespace FFmePlayer_snu.Controls
{
    /// <summary>
    /// live_login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class live_login
    {
        public string token { get; set; }

        public live_login()
        {
            InitializeComponent();

        }


        public string rtmp_endpoint_stu(string num, string lec, string token)
        {
            //"curl -X POST http://3.35.240.138:3333/superv_endpoint -d lec_id=logicdesign.midterm_20210107 -d supervNum=1 -d token="
            // 강좌이름 / 감독관 번호 / 날짜

            string URI = "http://3.35.240.138:3333/superv_endpoint";
            string myParameters = "lec_id=" + lec + "&supervNum=" + num + "&token=" + token;
            //"ID=2020-13579&PW=qwerty1234&lec_id=logic_design";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = webClient.UploadString(URI, myParameters);
                Console.WriteLine(HtmlResult);
                return HtmlResult;
            }
        }

        private void btn_login(object sender, RoutedEventArgs e)
        {

            //live_tab live_tab = new live_tab(token);

            //NavigationService.Navigate(live_tab);
        }
    }
}
