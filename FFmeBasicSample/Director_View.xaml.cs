using FFmePlayer_snu.Controls;
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
using Unosquare.FFME.Common;
using System.Net;
using System.Net.Http;

namespace FFmePlayer_snu
{
    /// <summary>
    /// Director_View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Director_View : Page
    {
        public string token;

        public Director_View(string parm_token)
        {
            InitializeComponent();

            token = parm_token;
            Console.WriteLine(token);

        }

        // Show another page on login button click
        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }

        private void btn_live_click(object sender, RoutedEventArgs e)
        {
            string live_date = txt_live_Date.SelectedDate.Value.ToString("yyyyMMdd");
            
            string rtmp_info = "lec_id=" + txt_live_Lec.Text + "." + txt_live_test.Text + "_" + live_date + "&supervNum=" + txt_live_Super.Text + "&token=" + token;
            string rtmp_uri = rtmp_endpoint_stu(rtmp_info);
            //rtmp_endpoint_stu(lec: "logicdesign", test: "midterm", date: "20210108", superv:"1");

            if (rtmp_uri == "error ")
                MessageBox.Show("Failed");
            else
            {
                MessageBox.Show("Success");
                live_tab live_view = new live_tab(rtmp_info, token);
                mainWindow.mainFrame.Navigate(live_view);
            }
        }

        private void btn_search_click(object sender, RoutedEventArgs e)
        {

            //string search_date = txt_search_Date.SelectedDate.Value.ToString("yyyyMMdd");
            string hls_uri = "a";
            // hls_endpoint_stu(num: txt_search_num.Text, lec: txt_search_lec.Text, date: search_date);


            // string hls_uri = "a";
            // hls_endpoint_stu(num: "2020-12345", lec: "logicdesign", token);

            //
            // hls_endpoint_stu( num: "2020-12345", lec : "logicdesign", token );

            if (hls_uri == "error ")
                MessageBox.Show("Failed");
            else
            {
                MessageBox.Show("Success");
                search_viewer search_view = new search_viewer(hls_uri, token);
                mainWindow.mainFrame.Navigate(search_view);
            }
        }

        public string rtmp_endpoint_stu(string info)
        {
            //"curl -X POST http://3.35.240.138:3333/superv_endpoint -d lec_id=logicdesign.midterm_20210108 -d supervNum=1 -d token="
            // 강좌이름 / 감독관 번호 / 날짜

            string URI = "http://3.35.240.138:3333/superv_endpoint";
            string myParameters = info;
            Console.WriteLine(myParameters);

            //"ID=2020-13579&PW=qwerty1234&lec_id=logic_design";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = webClient.UploadString(URI, myParameters);
                Console.WriteLine("======");
                Console.WriteLine(HtmlResult);
                Console.WriteLine("======");
                return HtmlResult;
            }
        }


        public string hls_endpoint_stu(string num, string lec, string date)
        {
            //"curl -X POST http://3.35.240.138:3333/get_test -d num=2020-12345 -d lec=logicdesign =d token="
            // 학번 강좌이름 날짜

            string URI = "http://3.35.240.138:3333/get_test";
            string myParameters = "num=" + num + "&lec=" + lec + "_" + date + "&token=" + token;
            //"ID=2020-13579&PW=qwerty1234&lec_id=logic_design";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = webClient.UploadString(URI, myParameters);
                Console.WriteLine(HtmlResult);
                return HtmlResult;
            }
        }

        private void btn_add_stu(object sender, RoutedEventArgs e)
        {
            Add_stu add_student_page = new Add_stu(token);
            add_student_page.Show();

        }


        private void live_date_double_click(object sender, RoutedEventArgs e)
        {
            txt_live_Date.IsDropDownOpen = true;
        }

        private void search_date_double_click(object sender, RoutedEventArgs e)
        {
            txt_search_Date.IsDropDownOpen = true;
        }
    }
}
