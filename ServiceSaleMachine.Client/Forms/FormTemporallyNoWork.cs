using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormTemporallyNoWork : MyForm
    {
        FormResultData data;

        private bool fileLoaded = false;

        public FormTemporallyNoWork()
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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureError, Globals.DesignConfiguration.Settings.ButtonFail);

            // список ошибок
            if (data.stage == WorkerStateStage.BillFull)
            {
                error.Text = "Ошибка E030";
            }
            else if (data.stage == WorkerStateStage.ErrorBill)
            {
                error.Text = "Ошибка E020";
            }
            else if (data.stage == WorkerStateStage.ResursEnd)
            {
                error.Text = "Ошибка E040";
            }
            else if (data.stage == WorkerStateStage.ErrorControl)
            {
                error.Text = "Ошибка E010";
            }
            else if (data.stage == WorkerStateStage.PaperEnd)
            {
                error.Text = "Ошибка E050";
            }
            else if (data.stage == WorkerStateStage.ErrorPrinter)
            {
                error.Text = "Ошибка E060";
            }
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
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
                case DeviceEvent.ConnectBillErrorEnd:
                    // связь с приемником возобновилась - ошибки ушли
                    {
                        data.stage = WorkerStateStage.BillErrorEnd;
                        this.Close();
                    }
                    break;
                default:
                    // другие события
                    if (data.stage == WorkerStateStage.BillFull)
                    {
                        //// только если вошли сюда с полным баком денег
                        //if (!((string)e.Message.Content).Contains("Drop Cassette out of position")
                        //|| !((string)e.Message.Content).Contains("Drop Cassette Full"))
                        //{
                        //    // не выемка
                        //    data.stage = WorkerStateStage.EndBillFull;
                        //    this.Close();
                        //}
                    }
                    else if (data.stage == WorkerStateStage.ErrorBill)
                    {

                    }
                    else if (data.stage == WorkerStateStage.ResursEnd)
                    {

                    }
                    else if (data.stage == WorkerStateStage.ErrorControl)
                    {

                    }
                        break;
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (!fileLoaded)
            {
                MessageText.LoadFile(Globals.GetPath(PathEnum.Text) + "\\ErrorText.rtf");
                fileLoaded = true;
                timer1.Interval = Globals.IntervalCheckControl;
            }

            if (data.stage == WorkerStateStage.ErrorControl)
            {
                // читаем состояние устройства
                byte[] res;
                res = data.drivers.control.GetStatusControl(data.log);

                if (res != null)
                {
                    if (res[0] > 0)
                    {
                        data.stage = WorkerStateStage.ErrorEndControl;
                        this.Close();
                    }
                }
            }

            if (data.stage == WorkerStateStage.PaperEnd || data.stage == WorkerStateStage.ErrorPrinter)
            {
                PrinterStatus status = data.drivers.printer.GetStatus();

                if ((status & (PrinterStatus.PRINTER_STATUS_PAPER_OUT
                             | PrinterStatus.PRINTER_STATUS_PAPER_JAM
                             | PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM
                             | PrinterStatus.PRINTER_STATUS_DOOR_OPEN
                             | PrinterStatus.PRINTER_STATUS_OFFLINE
                             | PrinterStatus.PRINTER_STATUS_ERROR)) == 0)
                {
                    if (data.stage == WorkerStateStage.PaperEnd)
                    {
                        // с бумагой стало OK
                        data.stage = WorkerStateStage.ErrorEndControl;
                        this.Close();

                        Program.Log.Write(LogMessageType.Error, "CHECK_STAT: бумага появилась.");
                    }
                    else if (data.stage == WorkerStateStage.ErrorPrinter)
                    {
                        data.stage = WorkerStateStage.ErrorEndControl;
                        this.Close();

                        Program.Log.Write(LogMessageType.Error, "CHECK_STAT: ошибка принтера снялась.");
                    }
                }
            }
        }

        private void FormTemporallyNoWork_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (data.stage == WorkerStateStage.ErrorEndControl)
            {
                data.log.Write(LogMessageType.Error, "CHECK_STAT: управляющее устройство работает нормально");
            }

            timer1.Enabled = false;
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void FormTemporallyNoWork_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
            else if (e.Alt & e.KeyCode == Keys.F5)
            {
                if (Globals.IsDebug)
                {
                    // пошлем событие вынимания приемника
                    Drivers.Message message = new Drivers.Message();

                    message.Event = DeviceEvent.DropCassetteBillAcceptor;
                    message.Content = "Drop bill";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    reciveResponse(null, e1);
                }
            }
            else if (e.Alt & e.KeyCode == Keys.F6)
            {
                if (Globals.IsDebug)
                {
                    // пошлем событие вынимания приемника
                    Drivers.Message message = new Drivers.Message();

                    message.Event = DeviceEvent.ConnectBillErrorEnd;
                    message.Content = "";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    reciveResponse(null, e1);
                }
            }
        }
    }
}
