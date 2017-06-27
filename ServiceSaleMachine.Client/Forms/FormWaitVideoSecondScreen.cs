using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    public partial class FormWaitVideoSecondScreen : Form
    {
        FormResultData data;

        public CurrentVideoEnum Video = CurrentVideoEnum.Video1;

        string[] ScreenVideo = new string[2];

        public FormWaitVideoSecondScreen()
        {
            ScreenVideo[0] = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo;
            ScreenVideo[1] = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo2;

            init();
        }

        public FormWaitVideoSecondScreen(Point position, Object obj)
        {
            this.Location = position;

            data = (FormResultData)obj;

            init();
        }

        void init()
        {
            InitializeComponent();

            VideoPlayer.uiMode = "none";
            VideoPlayer.settings.setMode("loop", true);

            VideoPlayer.URL = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo;
            VideoPlayer.Ctlcontrols.play();
        }

        /// <summary>
        /// Новое видео
        /// </summary>
        public void SetPlayNewVideo(CurrentVideoEnum video)
        {
            if (Globals.ClientConfiguration.Settings.offVideoSecondScreen == 1) return;

            // остановим текущее видео
            this.VideoPlayer.Ctlcontrols.stop();

            VideoPlayer.URL = ScreenVideo[(int)video];
            VideoPlayer.Ctlcontrols.play();
        }

        private void FormWaitClientVideo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void VideoPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 0)
            {
                if (data.log != null)
                {
                    data.log.Write(LogMessageType.Error, "Видео не найдено.");
                }
            }
            else if (e.newState == 1)
            {
                if (data.log != null)
                {
                    data.log.Write(LogMessageType.Error, "Видео остановлено.");
                }
            }
            else if (e.newState == 2)
            {
            }
            else if (e.newState == 8)
            {
                //if (data.log != null)
                //{
                //    data.log.Write(LogMessageType.Error, "Видео закончилось.");
                //}
            }
        }

        private void VideoPlayer_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            Close();
        }

        private void FormWaitClientVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.VideoPlayer.close();           // закрываем сам плеер, чтобы все ресурсы освободились
            this.Controls.Remove(VideoPlayer);  // убираем элемент WMP с формы
        }
    }
}
