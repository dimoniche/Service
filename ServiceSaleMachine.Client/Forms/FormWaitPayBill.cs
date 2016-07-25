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

        // зафиксировали купюру
        bool moneyFixed = false;

        public FormWaitPayBill()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pbxFail, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            if (Globals.ClientConfiguration.Settings.offHardware == 0 && Globals.ClientConfiguration.Settings.offBill == 0)
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(pbxForward, Globals.DesignConfiguration.Settings.ButtonNoForward);
                // пока не внесли нужную сумму - не жамкаем кнопку
                pbxForward.Enabled = false;
            }
            else
            {
                Globals.DesignConfiguration.Settings.LoadPictureBox(pbxForward, Globals.DesignConfiguration.Settings.ButtonForward);
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
            AmountServiceText.ForeColor = System.Drawing.Color.Red;

            // заменим обработчик событий
            data.drivers.ReceivedResponse += reciveResponse;

            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                // перейдем в режим ожидания купюр
                if (Globals.ClientConfiguration.Settings.changeOn > 0)
                {
                    // со сдачей - жрем деньги сразу
                    data.drivers.WaitBill();
                }
                else
                {
                    // без сдачи
                    data.drivers.WaitBillEscrow();
                }
            }
        }

        /// <summary>
        /// Внесли деньги
        /// </summary>
        /// <param name="e"></param>
        void CreditMoney(ServiceClientResponseEventArgs e)
        {
            lock (Params)
            {
                try
                {
                    if (moneyFixed) return;
                    moneyFixed = true;

                    // сбросим таймаут
                    Timeout = 0;

                    int numberNominal = ((BillNominal)e.Message.Content).nominalNumber;

                    if (Globals.ClientConfiguration.Settings.nominals[numberNominal] > 0)
                    {
                        // такую купюру мы обрабатываем

                    }
                    else
                    {
                        // такая купюра не пойдет - вернем ее
                        moneyFixed = false;
                        data.drivers.ReturnBill();
                        return;
                    }

                    //  прошли проверку

                    // внесли деньги
                    int count = 0;
                    int.TryParse(((BillNominal)e.Message.Content).Denomination, out count);

                    amount += count;

                    // сдача
                    int diff = 0;

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {
                        // сдача на чек
                        if (amount > data.serv.price)
                        {
                            // посчитаем размер сдачи
                            diff = amount - data.serv.price;

                            // тут надо решить как выдать сдачу
                        }
                    }
                    else
                    {
                        // без сдачи
                        if (count > data.serv.price)
                        {
                            // купюра великовата - вернем ее
                            moneyFixed = false;
                            data.drivers.ReturnBill();

                            // сообщим о том что купюра великовата
                            FormManager.OpenForm<FormBigBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify);
                            return;
                        }

                        if (amount <= data.serv.price)
                        {
                            // еще не все внесли - напишем номинал купюры с предложением съесть ее
                            bool res = (bool)FormManager.OpenForm<FormInsertBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, ((BillNominal)e.Message.Content).Denomination);

                            if (res)
                            {
                                // забираем купюру
                                moneyFixed = false;
                                data.drivers.StackBill();
                            }
                            else
                            {
                                // возвращаем ее обратно
                                amount -= count;

                                moneyFixed = false;
                                data.drivers.ReturnBill();
                                return;
                            }
                        }

                        // внесли достаточную для услуги сумму
                    }

                    // напишем на экране
                    AmountServiceText.Text = amount + " руб";

                    // деньги внесли - нет пути назад
                    TimeOutTimer.Enabled = false;

                    if (amount >= data.serv.price)
                    {
                        AmountServiceText.ForeColor = System.Drawing.Color.Green;
                        // внесли нужную сумму - можно идти вперед
                        Globals.DesignConfiguration.Settings.LoadPictureBox(pbxForward, Globals.DesignConfiguration.Settings.ButtonForward);
                        pbxForward.Enabled = true;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            if (Globals.ClientConfiguration.Settings.changeOn > 0)
                            {
                                data.drivers.StopWaitBill();
                                moneyFixed = false;
                            }
                            else
                            {
                                data.drivers.StopWaitBill();
                                moneyFixed = false;
                            }
                        }
                    }
                }
                catch(Exception exp)
                {
                    moneyFixed = false;
                }
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
                case DeviceEvent.BillAcceptorEscrow:
                    CreditMoney(e);
                    break;
                case DeviceEvent.BillAcceptorCredit:
                    CreditMoney(e);
                    break;
                default:
                    // Остальные события нас не интересуют
                    break;
            }
        }

        private void FormWaitPayBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;

            data.drivers.ReceivedResponse -= reciveResponse;

            if (amount > 0)
            {
                // что то уже внесли в аппарат - надо эти деньги записать на счет
            }

            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                // вернем деньгу
                moneyFixed = false;

                data.drivers.ReturnBill();
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
                    data.drivers.printer.PrintBody(data.serv);
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

            if(Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }
    }
}
