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
