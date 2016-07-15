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

            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonNoForward);
                // пока не внесли нужную сумму - не жамкаем кнопку
                pbxForward.Enabled = false;
            }
            else
            {
                pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonForward);
            }
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

            // перейдем в режим ожидания купюр
            data.drivers.WaitBill();
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

                            data.drivers.StopWaitBill();
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
            data.drivers.StopWaitBill();
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

            // Распечатать чек за услугу


            this.Close();
        }

        private void FormWaitPayBill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
