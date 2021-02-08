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
    /// Change_password.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Change_password : Window
    {
        /*
        *************** Change password ***************
        * curl commend를 통해 원하는 비밀번호로 비밀번호를 변경합니다.
        */

        public Change_password()
        {
            InitializeComponent();
            // Snu Symbol
            img.Source = new BitmapImage(new Uri(@"\Resources\snu_symbol.PNG", UriKind.RelativeOrAbsolute));
        }

        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }
        public string token; 

        private void btn_enter(object sender, RoutedEventArgs e)
        {
            //curl - X POST http://3.35.240.138:3333/change_password -d mail_address=John@snu.ac.kr -d PW=qwerty1234

            string pw1 = txt_PW1.Password.ToString();
            string pw2 = txt_PW2.Password.ToString();

            if (pw1 != pw2)
                MessageBox.Show("Failed! You entered diffrent passwords");
            else
            {
                string URI = "http://3.35.240.138:3333/change_password";
                string myParameters = "mail_address=" + txt_email.Text + "&PW=" + pw1;

                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    token = webClient.UploadString(URI, myParameters);

                    Window.GetWindow(this).Close();
                }
            }

        }
    }
}
