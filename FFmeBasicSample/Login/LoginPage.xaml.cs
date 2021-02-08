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
using FFmePlayer_snu.Controls;

namespace FFmePlayer_snu
{

    /// <summary>
    /// LoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginPage : Page
    {

        /*
        *************** Login page ***************
        ** 회원가입 승인이 나면, 임시 비밀번호가 주어집니다.
        ** 임시번호로 로그인 한 경우, 바로 비밀번호를 변경하도록 Change password 창이 띄워집니다. 
        *  비밀번호를 변경하고 나면, 로그인이 완료됩니다.
        ** 최초 로그인이 아니라면, 자신의 이메일과 비밀번호를 통해 로그인 할 수 있습니다.
        */

        public LoginPage()
        {
            InitializeComponent();

            img.Source = new BitmapImage(new Uri(@"\Resources\snu_symbol.PNG", UriKind.RelativeOrAbsolute));
        }

        // Show another page on login button click
        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }

        private void btn_login(object sender, RoutedEventArgs e)
        {

            //curl -X POST http://3.35.240.138:3333/login -d mail_address=John@snu.ac.kr -d PW=temp_password

            string URI = "http://3.35.240.138:3333/login";
            string myParameters = "mail_address=" + txtID.Text + "@snu.ac.kr" + "&PW=" + txtPW.Password.ToString();

            string HtmlResult;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                HtmlResult = webClient.UploadString(URI, myParameters);
            }
            
            // 로그인 실패
            if (HtmlResult == "error")
            {
                MessageBox.Show("Failed");
                txtPW.Password = null;
            }
            // 최초 로그인
            else if (HtmlResult == "Change Password!")
            {
                MessageBox.Show("Change Password!");

                Change_password pw_page = new Change_password();

                // 새로운 창을 가운데에 위치할 수 있도록 위치 지정
                pw_page.WindowStartupLocation = WindowStartupLocation.CenterScreen; 
                pw_page.ShowDialog();

                // token을 main page에 전달
                Main_Page main_Page = new Main_Page(pw_page.token);
                mainWindow.mainFrame.Navigate(main_Page);
            }
            // 로그인 성공 (token 반환)
            else
            {
                Main_Page main_Page = new Main_Page(HtmlResult);
                mainWindow.mainFrame.Navigate(main_Page);
            }
        }



        private void btn_create_account(object sender, RoutedEventArgs e)
        {
            Create_account create_accout_page = new Create_account();
            create_accout_page.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            create_accout_page.Show();
        }

    }
}
