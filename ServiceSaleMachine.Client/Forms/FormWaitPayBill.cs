using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitPayBill : MyForm
    {
        FormResultData data;

        // количество внесенных денег
        int amount;
        // остаток денег - на чек
        int balance;

        public FormWaitPayBill()
        {
            InitializeComponent();

            pbxFail.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonFail);

            if (Globals.ClientConfiguration.Settings.offHardware == 0 && Globals.ClientConfiguration.Settings.offBill == 0)
            {
                pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonNoForward);
                // пока не внесли нужную сумму - не жамкаем кнопку
                pbxForward.Enabled = false;
            }
            else
            {
                pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonForward);
            }

            TimeOutTimer.Enabled = true;
            Timeout = 0;
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

            // стоимость
            price.Text = data.serv.price + " руб";
            AmountServiceText.Text = "0 руб";

            // заменим обработчик событий
            data.drivers.ReceivedResponse += reciveResponse;

            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                // перейдем в режим ожидания купюр
                data.drivers.WaitBill();
            }
        }

        /// <summary>
        /// События от приемника денег
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.BillAcceptor:
                    
                    break;
                case DeviceEvent.BillAcceptorCredit:
                    {
                        // внесли деньги
                        int count = 0;
                        int.TryParse((string)e.Message.Content, out count);
                        amount += count;

                        AmountServiceText.Text = amount + " руб";

                        if (amount >= data.serv.price)
                        {
                            // внесли нужную сумму - можно идти вперед
                            pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonForward);
                            pbxForward.Enabled = true;

                            if (Globals.ClientConfiguration.Settings.offHardware == 0)
                            {
                                data.drivers.StopWaitBill();
                            }
                        }
                    }
                    break;
                default:
                    // Остальные события нас не интересуют
                    break;
            }
        }

        private void FormWaitPayBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;
            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                data.drivers.StopWaitBill();
            }
            Params.Result = data;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // запуск услуги
            data.stage = WorkerStateStage.StartService;

            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                // Распечатать чек за услугу
                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                if (data.drivers.printer.prn.PrinterIsOpen)
                {
                    data.drivers.printer.PrintHeader();
                    data.drivers.printer.PrintBody();
                    data.drivers.printer.PrintFooter();
                    data.drivers.printer.EndPrint();
                }
            }

            this.Close();
        }

        private void FormWaitPayBill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        int Timeout = 0;

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            Timeout++;

            if (Timeout > 30)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }
    }
}
