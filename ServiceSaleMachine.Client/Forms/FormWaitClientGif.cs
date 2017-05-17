using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;

namespace AirVitamin.Client
{
    public partial class FormWaitClientGif : MyForm
    {
        FormResultData data;
        private GifImage gifImage = null;

        public bool IsSendSMS1 = false;
        public bool IsSendSMS2 = false;
        public bool IsSendSMS3 = false;
        public bool IsSendSMS4 = false;

        public FormWaitClientGif()
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

            if (Globals.ClientConfiguration.Settings.ScreenServerType == 0)
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(ScreenSever, Globals.DesignConfiguration.Settings.ScreenSaver);
            }
            else
            {
                gifImage = new GifImage(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ScreenSaver);
                gifImage.ReverseAtEnd = false; //dont reverse at end
            }

            timer1.Enabled = true;
            timer2.Enabled = true;
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

        private void FormWaitClientGif_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void FormWaitClientGif_FormClosing(object sender, FormClosingEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
            timer1.Enabled = false;
            timer2.Enabled = false;
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
                if (res[0] == 1 && !IsSendSMS1)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 1", data.log);

                    data.stage = WorkerStateStage.Gas1_low;
                    IsSendSMS1 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД1 - HIGH.");
                }
                else if (res[0] == 0) IsSendSMS1 = false;

                if (res[1] == 1 && !IsSendSMS2)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 2", data.log);

                    data.stage = WorkerStateStage.Gas2_low;
                    IsSendSMS2 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД2 - HIGH.");
                }
                else if (res[1] == 0) IsSendSMS2 = false;

                if (res[2] == 1 && !IsSendSMS3)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 3", data.log);

                    data.stage = WorkerStateStage.Gas3_low;
                    IsSendSMS3 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД3 - HIGH.");
                }
                else if (res[2] == 0) IsSendSMS3 = false;

                if (res[3] == 1 && !IsSendSMS4)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 4", data.log);

                    data.stage = WorkerStateStage.Gas4_low;
                    IsSendSMS4 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД4 - HIGH.");
                }
                else if (res[3] == 0) IsSendSMS4 = false;
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

        private void ScreenSever_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.ScreenServerType == 1)
            {
                ScreenSever.Image = gifImage.GetNextFrame();
            }
        }
    }
}
