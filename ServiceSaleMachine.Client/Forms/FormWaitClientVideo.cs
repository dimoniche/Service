using AirVitamin.Drivers;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    public partial class FormWaitClientVideo : MyForm
    {
        FormResultData data;

        public FormWaitClientVideo()
        {
            InitializeComponent();
        }

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

            VideoPlayer.uiMode = "none";
            VideoPlayer.settings.setMode("loop", true);

            VideoPlayer.URL = Globals.GetPath(PathEnum.Video) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaverVideo;
            VideoPlayer.Ctlcontrols.play();

            timer1.Enabled = true;
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
                    {
                        data.stage = WorkerStateStage.BillFull;
                        this.Close();
                    }
                    break;
                case DeviceEvent.BillAcceptorError:
                case DeviceEvent.DropCassetteJammed:
                case DeviceEvent.BillCheated:
                    {
                        // ошибка купюроприемника
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.ConnectBillError:
                    {
                        // нет связи с купюроприемником
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
                    break;
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
            data.stage = WorkerStateStage.MainScreen;
            Close();
        }

        private void FormWaitClientVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
            timer1.Enabled = false;

            this.VideoPlayer.close();           // закрываем сам плеер, чтобы все ресурсы освободились
            this.Controls.Remove(VideoPlayer);  // убираем элемент WMP с формы
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReasonEnum reason = CheckError.GetStatus(data);

            if (reason == ReasonEnum.FormClose)
            {
                this.Close();
            }
        }
    }
}
