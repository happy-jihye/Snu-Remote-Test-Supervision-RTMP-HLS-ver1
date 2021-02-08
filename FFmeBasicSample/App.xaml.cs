namespace FFmePlayer_snu
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Unosquare.FFME;
    using FFmpeg.AutoGen;
    using System.Diagnostics;


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Library.FFmpegDirectory = @"c:\ffmpeg" + (Environment.Is64BitProcess ? @"\x64" : string.Empty);
            Library.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
            Library.EnableWpfMultiThreadedVideo = !Debugger.IsAttached;

            MediaElement.FFmpegMessageLogged += (s, ev) =>
            {
                System.Diagnostics.Debug.WriteLine(ev.Message);
            };
            base.OnStartup(e);
        }

    }

}
