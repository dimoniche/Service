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

            res = data.drivers.control.GetStatusRelay(data.log);

            if (res != null)
            {
                // просто шлем смс - не выходим пока с ошибкой
                if (res[0] >= 1 && !data.IsSendSMS1)
                {
                    data.drivers.modem.SendSMS("Упало давление кислорода", data.log);

                    data.stage = WorkerStateStage.Gas1_low;
                    data.IsSendSMS1 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД1 - HIGH.");

                    if (Globals.ClientConfiguration.Settings.offReserve == 1) this.Close();
                }
                else if (res[0] == 0) data.IsSendSMS1 = false;

                if (res[1] >= 1 && !data.IsSendSMS2)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 2", data.log);

                    data.stage = WorkerStateStage.Gas2_low;
                    data.IsSendSMS2 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД2 - HIGH.");
                }
                else if (res[1] == 0) data.IsSendSMS2 = false;

                if (res[2] >= 1 && !data.IsSendSMS3)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 3", data.log);

                    data.stage = WorkerStateStage.Gas3_low;
                    data.IsSendSMS3 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД3 - HIGH.");
                }
                else if (res[2] == 0) data.IsSendSMS3 = false;

                if (res[3] >= 1 && !data.IsSendSMS4)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 4", data.log);

                    data.stage = WorkerStateStage.Gas4_low;
                    data.IsSendSMS4 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД4 - HIGH.");

                    this.Close();
                }
                else if (res[3] == 0) data.IsSendSMS4 = false;
            }

            if (Globals.ClientConfiguration.Settings.offPrinter == 0)
            {
                PrinterStatus status = data.drivers.printer.GetStatus();

                if ((status & (PrinterStatus.PRINTER_STATUS_PAPER_OUT
                             | PrinterStatus.PRINTER_STATUS_PAPER_JAM
                             | PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM
                             | PrinterStatus.PRINTER_STATUS_DOOR_OPEN
                             | PrinterStatus.PRINTER_STATUS_ERROR)) > 0)
                {
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        data.stage = WorkerStateStage.PaperEnd;
                        this.Close();
                    }
                    else
                    {
                        if (data.PrinterError == false)
                        {
                            Program.Log.Write(LogMessageType.Error, "WAIT_MENU: кончилась бумага.");
                        }

                        data.PrinterError = true;
                    }
                }
                else if ((status & PrinterStatus.PRINTER_STATUS_OFFLINE) > 0)
                {
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        data.stage = WorkerStateStage.ErrorPrinter;
                        this.Close();
                    }
                    else
                    {
                        if (data.PrinterError == false)
                        {
                            Program.Log.Write(LogMessageType.Error, "WAIT_MENU: нет связи с принтером.");
                        }

                        data.PrinterError = true;
                    }
                }
                else
                {
                    if (data.PrinterError == true)
                    {
                        Program.Log.Write(LogMessageType.Error, "WAIT_MENU: ошибка принтера снялась.");
                    }

                    data.PrinterError = false;
                }
            }
            else
            {
                data.PrinterError = false;
            }
        }
    }
}
