using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;
using System;

namespace AirVitamin.Client
{
    public partial class FormMoneyRecess : MyForm
    {
        FormResultData data;

        public FormMoneyRecess()
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

            // обновим из базы статистические данные
            data.statistic = GlobalDb.GlobalBase.GetMoneyStatistic();

            // 
            moneySumm.Text = "Сумма денег в кассете: " + data.statistic.AllMoneySumm + " руб.";

            data.log.Write(LogMessageType.Information, "==============================================================================");
            data.log.Write(LogMessageType.Information, "INCASS: Сумма денег в кассете: " + data.statistic.AllMoneySumm + " руб.");
            data.log.Write(LogMessageType.Information, "INCASS: Сумма денег на аккаунтах: " + data.statistic.AccountMoneySumm + " руб.");
            data.log.Write(LogMessageType.Information, "INCASS: Количество банкнот: " + data.statistic.CountBankNote + " руб.");
            data.log.Write(LogMessageType.Information, "INCASS: Оказано услуг: " + data.statistic.ServiceMoneySumm + " руб.");
            data.log.Write(LogMessageType.Information, "==============================================================================");
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

                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
                default:
                    // другие события
                    if(!((string)e.Message.Content).Contains("Drop Cassette out of position"))
                    {
                        // не выемка
                        data.stage = WorkerStateStage.EndDropCassette;
                        this.Close();
                    }
                    break;
            }
        }

        private void FormMoneyRecess_FormClosed(object sender, FormClosedEventArgs e)
        {
            // обнулим статистику
            data.statistic.AllMoneySumm = 0;
            data.statistic.CountBankNote = 0;
            data.statistic.ServiceMoneySumm = 0;

            // запишем инфу о последнем обслуживании и инкасации
            DateTime dt = GlobalDb.GlobalBase.GetLastEncashment();

            int countmoney = 0;
            if (dt != null)
            {
                countmoney = GlobalDb.GlobalBase.GetCountMoney(dt);
            }
            else
            {
                countmoney = GlobalDb.GlobalBase.GetCountMoney(new DateTime(2000, 1, 1));
            }

            // запишем сколько инкассировали и обновим время инкасации
            GlobalDb.GlobalBase.Encashment(data.CurrentUserId, countmoney);

            // очистим накопленные банктноты
            GlobalDb.GlobalBase.ClearBankNotes();

            data.drivers.ReceivedResponse -= reciveResponse;
            Params.Result = data;
        }

        private void FormMoneyRecess_Shown(object sender, EventArgs e)
        {
            // форма инкассации открылась - предложим зарегистрироваться
        }

        private void scalableLabel2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
                Close();
            }
            else if (e.Alt & e.KeyCode == Keys.F5)
            {
                if(Globals.IsDebug)
                {
                    // пошлем событие вставки обратно приемника
                    Drivers.Message message = new Drivers.Message();

                    message.Event = DeviceEvent.InitializationOK;
                    message.Content = "Insert bill";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    reciveResponse(null, e1);
                }
            }
        }
    }
}
