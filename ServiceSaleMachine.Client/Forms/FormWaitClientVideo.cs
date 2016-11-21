using ServiceSaleMachine.Drivers;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitClientVideo : MyForm
    {
        FormResultData data;

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            data.drivers.ReceivedResponse += reciveResponse;
            timer1.Enabled = true;

            VideoPlayer.uiMode = "none";
            VideoPlayer.settings.setMode("loop", true);

            VideoPlayer.URL = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo;
            VideoPlayer.Ctlcontrols.play();
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MachineDrivers.ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
            }
        }

        public FormWaitClientVideo()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // читаем состояние устройства
            byte[] res;
            res = data.drivers.control.GetStatusControl(data.log);

            if (res != null)
            {
                if (res[0] == 0)
                {
                    data.stage = WorkerStateStage.ErrorControl;
                    this.Close();
                }
            }
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

            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
            timer1.Enabled = false;
        }
    }
}
