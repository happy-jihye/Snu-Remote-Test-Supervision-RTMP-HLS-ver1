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
using System.Net;
using System.Net.Http;

namespace FFmePlayer_snu
{
    /// <summary>
    /// Create_account.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Create_account : Window
    {
        /*
        *********** Create Account ***********
        * 회원가입 창에서는 학번, 이름, 이메일을 입력받습니다. 
        * curl 명령어를 통해 회원가입 정보를 데이터베이스에 보냈습니다. 
        * 이후 승인이 이뤄지면 회원가입이 가능해집니다.
        */

        public Create_account()
        {
            InitializeComponent();
            // Snu Symbol
            img.Source = new BitmapImage(new Uri(@"\Resources\snu_symbol.PNG", UriKind.RelativeOrAbsolute));
        }

        private void btn_account_enter(object sender, RoutedEventArgs e)
        {
            //curl -X POST http://XXX/sign_up -d ID=2020-54321 -d name=John -d mail_address=John@snu.ac.kr

            string URI = "http://XXX/sign_up";
            string myParameters = "ID=" + txt_num.Text + "&name=" + txt_name.Text + "@snu.ac.kr" + "&mail_address=" + txt_email.Text;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.UploadString(URI, myParameters);

                MessageBox.Show("Success!");
            }

            // 회원가입이 끝나면 창을 닫습니다.
            Window.GetWindow(this).Close();
        }
        

    }
}
