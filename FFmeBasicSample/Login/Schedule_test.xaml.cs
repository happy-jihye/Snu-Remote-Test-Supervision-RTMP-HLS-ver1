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
    /// add_stu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Schedule_test : Page
    {
        /*
        * Scheduling test 에서는 
        * 시험정보(강의명, 시험명 ex. midterm, 시험 날짜, 시험 시작시간과 끝나는시간)와 
        * 학생 정보(이름, 학번, 감독관 번호)를 입력받습니다.
        */

        string token;
        public Schedule_test(string parm_token)
        {
            InitializeComponent();
            token = parm_token;

            img.Source = new BitmapImage(new Uri(@"\Resources\snu_symbol.PNG", UriKind.RelativeOrAbsolute));
        }

        private void btn_test_click(object sender, RoutedEventArgs e)
        {
            // lecture
            txt_stu_Lec.Text = txt_test_Lec.Text;
            txt_stu_Lec.IsReadOnly = true;

            // test
            txt_stu_test.Text = txt_test_test.Text;
            txt_stu_test.IsReadOnly = true;

            // testdate
            string test_testdate = txt_test_testdate.SelectedDate.Value.ToString("yyyyMMdd");
            txt_stu_testdate.Text = test_testdate;
            txt_stu_testdate.IsReadOnly = true;
            Console.WriteLine(txt_stu_testdate.Text);

            // start time
            
            string test_start_time = txt_test_start_time.SelectedTime.Value.ToString("HHmm");
            txt_stu_start_time.Text = test_start_time;
            txt_stu_start_time.IsReadOnly = true;

            // end time
            
            string test_end_time = txt_test_end_time.SelectedTime.Value.ToString("HHmm");
            txt_stu_end_time.Text = test_end_time;
            txt_stu_end_time.IsReadOnly = true;

            if((txt_test_Lec.Text == null | txt_test_test.Text == null | test_testdate == null | test_start_time == null | test_end_time == null))
                MessageBox.Show("Failed");
            else
            {
                string test_info_result = test_info(lec: txt_test_Lec.Text, test: txt_test_test.Text, testdate: test_testdate, starttime: test_start_time, endtime: test_end_time);
                Console.WriteLine(test_info_result);

                if (test_info_result == "error")
                    MessageBox.Show("Failed");
                else
                {
                    MessageBox.Show("Success");
                }
            }
        }

        private void btn_stu_click(object sender, RoutedEventArgs e)
        {

            string stu_info_result = stu_info(num: txt_stu_num.Text, name: txt_stu_name.Text, superv: txt_stu_superv.Text , lec: txt_stu_Lec.Text, test: txt_stu_test.Text, testdate: txt_stu_testdate.Text, starttime: txt_stu_start_time.Text, endtime: txt_stu_end_time.Text);
            Console.WriteLine(stu_info_result);

            if (stu_info_result == "error")
                txt_success.Text = "Failed";
            else
            {
                txt_success.Text = "Success!";
            }
        }

        public string test_info(string lec, string test, string testdate, string starttime, string endtime)
        {
            //curl -X POST http://3.35.240.138:3333/add_exam_data -d lec=logicdesign -d test=midterm -d testdate=20210108 -d starttime=1400 -d endtime=1530 -d token=

            string URI = "http://3.35.240.138:3333/add_exam_data";
            Console.WriteLine(URI);
            string myParameters = "lec=" + lec + "&test=" + test + "&testdate=" + testdate + "&starttime=" + starttime + "&endtime=" + endtime + "&token=" + token;
            Console.WriteLine(myParameters);
            //"ID=2020-13579&PW=qwerty1234&lec_id=logic_design";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = webClient.UploadString(URI, myParameters);
                return HtmlResult;
            }
        }

        public string stu_info(string num, string name, string superv, string lec, string test, string testdate, string starttime, string endtime)
        {
            //curl -X POST http://XXX/add_student_data -d num=2020-12345 -d name=원준 -d supervNum=1 -d lec=logicdesign -d test=midterm -d testdate=20210108 -d starttime=1400 -d endtime=1530 -d token=

            string URI = "http://XXX/add_student_data";
            string myParameters = "num=" + num + "&name=" + name + "&supervNum=" + superv + "&lec=" + lec + "&test=" + test + "&testdate=" + testdate + "&starttime=" + starttime + "&endtime=" + endtime + "&token=" + token;
            //"ID=2020-13579&PW=qwerty1234&lec_id=logic_design";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = webClient.UploadString(URI, myParameters);
                Console.WriteLine(HtmlResult);
                return HtmlResult;
            }
        }

        private void txt_stu_superv_TextChanged(object sender, TextChangedEventArgs e)
        {
            txt_success.Text = " ";
        }

        private void txt_stu_num_TextChanged(object sender, TextChangedEventArgs e)
        {
            txt_success.Text = " ";
        }

        private void txt_stu_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            txt_success.Text = " ";
        }

        private void date_double_click(object sender, RoutedEventArgs e)
        {
            txt_test_testdate.IsDropDownOpen = true;
        }
        private void start_double_click(object sender, RoutedEventArgs e)
        {
            txt_test_start_time.IsDropDownOpen = true;
        }
        private void end_double_click(object sender, RoutedEventArgs e)
        {
            txt_test_end_time.IsDropDownOpen = true;
        }

        private void txt_success_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }
        private void home_Click(object sender, RoutedEventArgs e)
        {
            Main_Page main_page = new Main_Page(token);
            mainWindow.mainFrame.Navigate(main_page);
        }
    }
}
