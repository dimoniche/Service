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

            ScreenVideo[0] = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo1;
            ScreenVideo[1] = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo2;

            VideoPlayer.playlist.add("file:///" + ScreenVideo[0]);
            VideoPlayer.playlist.play();
        }

        /// <summary>
        /// Новое видео
        /// </summary>
        public void SetPlayNewVideo(CurrentVideoEnum video)
        {
            if (Globals.ClientConfiguration.Settings.offVideoSecondScreen == 1) return;

            // остановим текущее видео
            VideoPlayer.playlist.stop();
            VideoPlayer.playlist.items.clear();

            VideoPlayer.playlist.add("file:///" + ScreenVideo[(int)video]);
            VideoPlayer.playlist.play();
        }

        private void FormWaitClientVideo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void FormWaitClientVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Controls.Remove(VideoPlayer);  // убираем элемент WMP с формы
        }
    }
}
