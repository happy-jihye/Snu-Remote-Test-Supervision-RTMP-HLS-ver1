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
using System.ComponentModel;
using HLSTools.NETFramework;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Net.Http;


namespace FFmePlayer_snu.Controls
{
    /// <summary>
    /// live_viewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class search_viewer
    {

        // Insert .m3u8 URL here
        public string hls_uri = null;

        // Insert your Bing Speech to Text API subscription key here
        private const string BingSpeechToTextApiSubscriptionKey = "";


        private const int MinMediaSegmentBufferSize = 1;
        private const int MaxSingleSubtitleLengthInCharacters = 50;

        private HLSProcessor _hlsProcessor;
        public MediaSegmentBuffer _mediaSegmentBuffer;
        private Timer _subtitleTimer;
        private IList<string> _subtitles;
        private IList<int> _subtitleDurationsInMilliseconds;
        private int _subtitleIndex;
        private bool _mediaStarted;

        public string token;
        
        // Show another page on login button click
        MainWindow mainWindow { get => Application.Current.MainWindow as MainWindow; }

        public search_viewer(string parm_token)
        {
            InitializeComponent();
            token = parm_token;
            initial_info();
            mainWindow.Closing += page_closed;
        }

        #region Select Video

        // 1) Combobox에 감독관이 담당한 학생들이 자동으로 나열됩니다.
        private void initial_info()
        {
            // curl -X POST http://3.35.240.138:3333/get_test_pre -d token=
            string list;

            string URI = "http://3.35.240.138:3333/get_test_pre";
            string myParameters = "token=" + token;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                list = webClient.UploadString(URI, myParameters);
            }

            string[] list_split = list.Split('^');

            for (int i = 0; i < list_split.Length; i++)
            {
                test_combo.Items.Add(list_split[i]);
            }
            test_combo.SelectedIndex = 0;
        }

        // 2) 시험을 선택하면 두번째 combobox에 그 시험을 치룬 학생들 목록이 나열됩니다.

        private void test_combo_click(object sender, RoutedEventArgs e)
        {
            stu_combo.Items.Clear();

            string test_info = test_combo.SelectedItem.ToString();
            string[] info = test_info.Split('.');

            // curl -X POST http://3.35.240.138:3333/previousvideo_student_list -d lec=logicdesign -d testdate=20210111 -d test=final
            string list;

            string URI = "http://3.35.240.138:3333/previousvideo_student_list";
            string myParameters = "lec=" + info[0] + "&testdate=" + info[2] + "&test=" + info[1] + "&token=" + token;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                list = webClient.UploadString(URI, myParameters);
            }

            string[] list_split = list.Split('^');

            for (int i = 0; i < list_split.Length; i++)
            {
                stu_combo.Items.Add(list_split[i]);
            }
            stu_combo.Tag = info[0];

        }

        // 3) 최종적으로 학생을 선택하면, 해당 학생의 영상을 볼 수 있는 hls 주소가 반환됩니다.
        private void stu_combo_click(object sender, RoutedEventArgs e)
        {

            string stu_info = stu_combo.SelectedItem.ToString();
            string[] test_info = test_combo.Text.Split('.');

            //curl -X POST http://3.35.240.138:3333/get_test -d num=2020-12345 -d lec=logicdesign =d token=

            string URI = "http://3.35.240.138:3333/get_test";
            string myParameters = "num=" + stu_info + "&lec=" + test_info[0] + "_" + test_info[2] + "&token=" + token;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                // 최종 HLS URI
                hls_uri = webClient.UploadString(URI, myParameters);
            }
        }

        #endregion


        bool first_click = false;
        int media_num = 0;
        bool terminate = false;

        #region Play, Pause, Stop

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if ( !first_click )
            {
                // 처음으로 media가 재생되는 경우에는 mediabuffer를 통하여 ts파일을 local에 재생합니다.                
                if (!terminate)
                {
                    _subtitles = new List<string>();
                    _subtitleDurationsInMilliseconds = new List<int>();

                    _hlsProcessor = new HLSProcessor(BingSpeechToTextApiSubscriptionKey);
                    _hlsProcessor.MediaSegmentProcessed += OnHlsMediaSegmentProcessed;
                    media_num = _hlsProcessor.media_count(new Uri(hls_uri));


                    _mediaSegmentBuffer = new MediaSegmentBuffer();
                    _mediaSegmentBuffer.BufferChanged += OnMediaSegmentBufferChanged;


                    Search_viewer_Loaded();

                    mediaElement.LoadedBehavior = MediaState.Manual;
                    mediaElement.MediaEnded += OnMediaElementMediaEnded;
                    mediaElement.Volume = 0;

                    ////Time Slider
                    mediaelement_MediaOpened();
                }

                first_click = true;
            }
            else
            {
                mediaElement.Play();
            }
           
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();

        }
        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            // stop 버튼을 누른 경우, 첫 ts파일이 재생됩니다.
            if (_mediaSegmentBuffer.PlayFirst(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri))
            {
                System.Diagnostics.Debug.WriteLine($"Playing {localTsFileUri}...");
               mediaElement.Source = localTsFileUri;
                mediaElement.Stop();
                ConfigureSubtitlesForNextSegment(mediaSegmentContent);

                // Time Slider
                current_index = 0;
                Time_slider.Value = current_index * 30;

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Failed to get the next media segment");
            }
        }
        #endregion

        #region Volume / Speed
        // Slider를 통해 volume과 영상의 speed를 지원합니다. (배속재생 가능)

        private void Slider_volume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                mediaElement.Volume = slider.Value;
            }
            // 플레이시간이 변경되면, 표시영역을 label에 업데이트합니다.
            lblVolume.Content = slider.Value.ToString();
        }
        

        private void Slider_speed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                mediaElement.SpeedRatio = slider.Value;
            }
            // 플레이시간이 변경되면, 표시영역을 label에 업데이트합니다.
            lblSpeed.Content = slider.Value.ToString();
        }
        #endregion

        // 30초 건너뛰기
        private void btn_skip(object sender, RoutedEventArgs e)
        {
            PlayNext();
        }

        /// <summary>
        /// Starts processing the playlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_viewer_Loaded()
        {
            if (string.IsNullOrWhiteSpace(hls_uri))
            {
                throw new ArgumentNullException("No playlist defined");
            }

            progressBar.IsEnabled = true;
            new Thread(async () =>
            {
                Thread.CurrentThread.IsBackground = true;

                try
                {
                    await _hlsProcessor.StartProcessingAsync(hls_uri);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to start processing playlist: {ex.Message}");
                }
            }).Start();


            //// Time Slider
            // 동영상 파일의 Timespan 제어를 위해 초기화와 이벤트처리기를 추가합니다.
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += TimerTickHandler;
            timer.Start();

        }

        #region Time Slider
        // time slider를 통해 원하는 부분을 바로 재생할 수 있도록 구현하였습니다.

        bool sldrDragStart = false;

        // 미디어파일 타임 핸들러 // 미디어파일의 실행시간이 변경되면 호출된다. 
        void TimerTickHandler(object sender, EventArgs e)
        {
            // 미디어파일 실행시간이 변경되었을 때 사용자가 임의로 변경하는 중인지를 체크한다. 
            if (sldrDragStart)
                return;
            if (mediaElement.Source == null || !mediaElement.NaturalDuration.HasTimeSpan)
            {
                lblPlayTime.Content = "No file selected...";
                //return; 
            } 
            
            // 미디어 파일 총 시간을 슬라이더와 동기화한다. 
            if(current_index >= 0)
            {
                // Ts 파일의 길이가 총 30초 (영호님과 협의함)
                Time_slider.Value = mediaElement.Position.TotalSeconds + 30 * current_index;
            }
        }

        private void mediaelement_MediaOpened()
        {
            Time_slider.Minimum = 0;
            if (mediaElement.NaturalDuration.HasTimeSpan)
                Time_slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            else
                Time_slider.Maximum = (media_num - 1) * 30 ;
        }

        private void SldrPlayTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            // 사용자가 시간대를 변경하면, 잠시 미디어 재생을 멈춘다. 
            sldrDragStart = true;
            mediaElement.Pause();
        }

        private void SldrPlayTime_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            // 사용자가 지정한 시간대로 이동하면, 이동한 시간대로 값을 지정한다. 
            
            // 1) 현재 재생하는 부분보다 뒤로 이동한 경우
            if(Time_slider.Value > 30 * current_index)
            {
                if((Time_slider.Value - 30 * current_index) < 30)
                {
                    mediaElement.Position = TimeSpan.FromSeconds(Time_slider.Value - 30 * current_index);
                }
                else
                {
                    int index = (int)(Time_slider.Value - 30 *current_index) / 30;
                    for (int i = 0; i < index; i++)
                        PlayNext();
                    mediaElement.Position = TimeSpan.FromSeconds(Time_slider.Value - 30 * current_index);
                }
            }
            // 2) 현재 재생하는 부분보다 앞으로 이동한 경우
            else
            {
                if ((-Time_slider.Value + 30 * current_index) < 30)
                {
                    int time = (int)(-Time_slider.Value + 30 * current_index);
                    PlayPrevious();
                    mediaElement.Position = TimeSpan.FromSeconds(30 - time);
                }
                else
                {
                    int index = (int)(- Time_slider.Value + 30 * current_index) / 30;
                    for (int i = 0; i < index; i++)
                        PlayPrevious();
                    //mediaElement.Position = TimeSpan.FromSeconds(Time_slider.Value - 30 * current_index);
                }
            }

            //mediaElement.Position = TimeSpan.FromSeconds(Time_slider.Value);
            // 멈췄던 미디어를 재실행한다. 
            mediaElement.Play();
            sldrDragStart = false;
        }

        private void SldrPlayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement.Source == null)
                return;

            // 플레이시간이 변경되면, 표시영역을 업데이트한다.
            lblPlayTime.Content = String.Format("{0} / {1}",
                (mediaElement.Position + TimeSpan.FromSeconds(30*current_index)).ToString(@"mm\:ss"),
                TimeSpan.FromSeconds(30 * media_num).ToString(@"mm\:ss"));
        }
        #endregion

        int current_index = -1;

        #region 뒤로 이동 / 앞으로 이동 (30초 단위)
        /// <summary>
        /// Plays the next media in the buffer.
        /// </summary>
        private void PlayNext()
        {
            if (_mediaSegmentBuffer.TryGetNext(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri))
            {
                System.Diagnostics.Debug.WriteLine($"Playing {localTsFileUri}...");
                mediaElement.Source = localTsFileUri;             
                mediaElement.Play();
                ConfigureSubtitlesForNextSegment(mediaSegmentContent);

                // Time Slider
                current_index++;
                Time_slider.Value = current_index * 30;
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Failed to get the next media segment");
            }
        }

        private void PlayPrevious()
        {
            if (_mediaSegmentBuffer.TryGetPrevious(out MediaSegmentContent mediaSegmentContent, out Uri localTsFileUri))
            {
                System.Diagnostics.Debug.WriteLine($"Playing {localTsFileUri}...");
                mediaElement.Source = localTsFileUri;
                mediaElement.Play();
                ConfigureSubtitlesForNextSegment(mediaSegmentContent);

                // Time Slider
                current_index--;
                Time_slider.Value = current_index * 30;

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Failed to get the next media segment");
            }
        }
        #endregion

        #region ETC ..

        /// <summary>
        /// Adds the processed media segment into the buffer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mediaSegmentContent">The processed media segment.</param>
        private void OnHlsMediaSegmentProcessed(object sender, MediaSegmentContent mediaSegmentContent)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                _mediaSegmentBuffer.Add(mediaSegmentContent);
            }));
        }


        // media 재생 시작하는 부분의 코드

        /// <summary>
        /// Starts playing the media in the buffer given that we have enough buffered content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="numberOfMediaSegmentsInBuffer"></param>
        private void OnMediaSegmentBufferChanged(object sender, int numberOfMediaSegmentsInBuffer)
        {
            if (!_mediaStarted && numberOfMediaSegmentsInBuffer >= MinMediaSegmentBufferSize)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    progressBar.IsEnabled = false;
                    progressBar.Visibility = Visibility.Collapsed;
                }));

                _mediaStarted = true;
                PlayNext();
            }

        }

        private void OnMediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Media ended - trying to play the next one");
            PlayNext();
        }

        /// <summary>
        /// Configures the subtitles for the given media segment.
        /// </summary>
        /// <param name="mediaSegmentContent"></param>
        private void ConfigureSubtitlesForNextSegment(MediaSegmentContent mediaSegmentContent)
        {
            if (_subtitleTimer != null)
            {
                _subtitleTimer.Dispose();
                _subtitleTimer = null;
            }

            _subtitles.Clear();
            _subtitleDurationsInMilliseconds.Clear();
            _subtitleIndex = 0;

            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                subtitleTextBlock.Text = string.Empty;
            }));

            if (mediaSegmentContent.Transcription != null
                && mediaSegmentContent.Transcription.NBest.Count > 0
                && !string.IsNullOrWhiteSpace(mediaSegmentContent.Transcription.NBest[0].Display))
            {
                if (mediaSegmentContent.Transcription.NBest[0].Confidence > 0.5f)
                {
                    string transcription = mediaSegmentContent.Transcription.NBest[0].Display;
                    string[] words = transcription.Split(' ');
                    string subtitle = string.Empty;

                    for (int i = 0; i < words.Length; ++i)
                    {
                        subtitle += $"{words[i]} ";

                        if (subtitle.Length >= MaxSingleSubtitleLengthInCharacters
                            || i == words.Length - 1)
                        {
                            _subtitles.Add(subtitle.Trim());
                            subtitle = string.Empty;
                        }
                    }

                    // Transcription.Duration is measured in "ticks"
                    int transcriptionDurationInMilliseconds =
                        mediaSegmentContent.Transcription.Duration / 10000;

                    foreach (string subtitleElement in _subtitles)
                    {
                        double ratio = (double)subtitleElement.Length / (double)transcription.Length;
                        _subtitleDurationsInMilliseconds.Add((int)(transcriptionDurationInMilliseconds * ratio));
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No enough confidence in respect to transcription of this segment: {mediaSegmentContent.Transcription.NBest[0].Confidence}");
                }
            }

            if (_subtitleDurationsInMilliseconds.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Resolved {_subtitleDurationsInMilliseconds.Count} subtitle segment(s) for the current media segment");
                SubtitleTimerCallback(null);
            }
        }

        /// <summary>
        /// Updates the subtitle text.
        /// </summary>
        /// <param name="state"></param>
        private void SubtitleTimerCallback(object state)
        {
            if (_subtitles.Count > _subtitleIndex)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    subtitleTextBlock.Text = _subtitles[_subtitleIndex];
                }));

                System.Diagnostics.Debug.WriteLine(
                    $"Subtitle segment: {_subtitleDurationsInMilliseconds[_subtitleIndex]} ms -> \"{_subtitles[_subtitleIndex]}\"");
            }

            if (_subtitleDurationsInMilliseconds.Count > _subtitleIndex + 1)
            {
                // We have a next subtitle for this segment
                if (_subtitleTimer != null)
                {
                    _subtitleTimer.Dispose();
                }

                _subtitleTimer = new Timer(
                    SubtitleTimerCallback,
                    null,
                    _subtitleDurationsInMilliseconds[_subtitleIndex],
                    int.MaxValue);
            }

            _subtitleIndex++;
        }
        #endregion

        #region HLS Finish : home으로 이동 / 어플리케이션 종료
        private void btn_home(object sender, RoutedEventArgs e)
        {
            // Dispose (local 에 저장된 ts파일들을 delete하기 위해)

            if (first_click) {
                Console.WriteLine("Dsipose");
                _mediaSegmentBuffer.Dispose();
            }

            //curl -X POST http://3.35.240.138:3333/hlsFinish -d httpUrl=https://node-sdk-sample-976067b2-cb45-4960-844f-000466192d2f.s3.ap-northeast-2.amazonaws.com//media/20201228/young_1228_1/young_1228_1.m3u8

            string URI = "http://3.35.240.138:3333/hlsFinish";
            string myParameters = "httpUrl=" + hls_uri;

            using (WebClient webClient = new WebClient())
            {
                if(hls_uri != null)
                {
                    webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string HtmlResult = webClient.UploadString(URI, myParameters);
                }
            }

            // home으로 이동
            Main_Page main_Page = new Main_Page(token);
            mainWindow.mainFrame.Navigate(main_Page);
        }

        private void page_closed(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("정말 종료하시겠습니까?", "종료", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else { 
                terminate = true;
                if (first_click)
                    _mediaSegmentBuffer.Dispose();

                string URI = "http://3.35.240.138:3333/hlsFinish";
                string myParameters = "httpUrl=" + hls_uri;

                using (WebClient webClient = new WebClient())
                {
                    if (hls_uri != null)
                    {
                        webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = webClient.UploadString(URI, myParameters);
                    }
                }
        
            }
        }
        #endregion
    }
}
