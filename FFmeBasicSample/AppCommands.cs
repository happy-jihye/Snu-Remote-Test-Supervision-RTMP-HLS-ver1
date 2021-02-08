namespace FFmePlayer_snu
{
    using System;
    using System.Windows;
    using Foundation;
    using Unosquare.FFME;

    public class AppCommands
    {
        private MediaElement m_MediaElement;

        private DelegateCommand m_PlayCommand;

        public DelegateCommand PlayCommand
        {
            get
            {
                if (m_PlayCommand == null)
                    m_PlayCommand = new DelegateCommand(async o =>
                    {
                        await m_MediaElement.Play();
                    });

                return m_PlayCommand;
            }
        }
    }
}
