using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitPayBill : MyForm
    {
        FormResultData data;

        // количество внесенных денег
        int amount;

        // положили на чек
        int difftoCheck = 0;
        // положили на аккаунт
        int difftoAccount = 0;

        // зафиксировали купюру
        bool moneyFixed = false;

        public FormWaitPayBill()
        {
            InitializeComponent();

            TimeOutTimer.Enabled = true;
            Timeout = 0;
            amount = 0;

            if (Globals.ClientConfiguration.Settings.offHardware == 0 && Globals.ClientConfiguration.Settings.offBill == 0)
            {
                // пока не внесли нужную сумму - не жамкаем кнопку
                pBxGiveOxigen.Enabled = false;
            }
        }

        private GifImage gifImage = null;
        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            gifImage = new GifImage(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonGetOxigen);
            gifImage.ReverseAtEnd = false; //dont reverse at end

            //Globals.DesignConfiguration.Settings.LoadPictureBox(pBxGiveOxigen, Globals.DesignConfiguration.Settings.ButtonGetOxigen);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxReturnBack, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            AmountServiceText.Text = "Внесено: 0 руб.";
            AmountServiceText.ForeColor = System.Drawing.Color.Red;
            SecondMessageText.Text = "";

            LabelNameService2.Text = Globals.ClientConfiguration.Settings.services[data.numberService].caption;

            TextPayBill.LoadFile(Globals.GetPath(PathEnum.Text) + "\\WaitPayBill.rtf");

            // заменим обработчик событий
            data.drivers.ReceivedResponse += reciveResponse;

            if (data.serv.price == 0)
            {
                // может быть цена нулевая - и это демо режим - можно сразу без денег работать
                AmountServiceText.ForeColor = System.Drawing.Color.Green;
                pBxGiveOxigen.Enabled = true;

                data.log.Write(LogMessageType.Information, "WAIT BILL: работаем в демо режиме.");
            }
            else if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                // перейдем в режим ожидания купюр
                data.drivers.CCNETDriver.WaitBillEscrow();
                data.log.Write(LogMessageType.Information, "WAIT BILL: запускаем режим ожидания купюр.");
            }
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MachineDrivers.ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            if (data.log != null)
            {
                data.log.Write(LogMessageType.Information, "WAIT BILL: Событие: " + e.Message.Content + ".");
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.BillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorEscrow:
                    CreditMoney(e);
                    break;
                case DeviceEvent.BillAcceptorCredit:
                    //CreditMoney(e);
                    break;
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        data.log.Write(LogMessageType.Information, "WAIT BILL: Вытащили купюроприемник.");

                        this.Close();
                    }
                    break;
                default:
                    // Остальные события нас не интересуют
                    break;
            }
        }

        private void CreditMoney(ServiceClientResponseEventArgs e)
        {
            pBxReturnBack.Enabled = false;
            SecondMessageText.Text = "";

            lock (Params)
            {
                try
                {
                    if (moneyFixed) return;
                    moneyFixed = true;

                    data.log.Write(LogMessageType.Information, "BILL: Обработаем получение денег");

                    // сбросим таймаут
                    Timeout = 0;

                    int numberNominal = ((BillNominal)e.Message.Content).nominalNumber;

                    data.log.Write(LogMessageType.Information, "WAIT BILL: Внесли купюру номиналом " + numberNominal + " руб.");

                    if (Globals.ClientConfiguration.Settings.nominals[numberNominal] > 0)
                    {
                        // такую купюру мы обрабатываем

                    }
                    else
                    {
                        // такая купюра не пойдет - вернем ее
                        moneyFixed = false;
                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.ReturnBill();
                        }

                        data.log.Write(LogMessageType.Information, "WAIT BILL: Купюру " + numberNominal + " не принимаем");

                        SecondMessageText.Text = "Внесите купюру другого номинала.";
                        return;
                    }

                    //  прошли проверку

                    // внесли деньги
                    int count = 0;
                    int.TryParse(((BillNominal)e.Message.Content).Denomination, out count);

                    amount += count;

                    // сдача
                    int diff = 0;
                    bool res = true;    // купюру забираем всегда - предлагаем вернуть - только если перебор

                    if (Globals.ClientConfiguration.Settings.changeOn == 0)
                    {
                        // без сдачи
                        if (amount > data.serv.price)
                        {
                            // купюра великовата - вернем ее
                            amount -= count;

                            moneyFixed = false;

                            if (Globals.ClientConfiguration.Settings.offHardware == 0)
                            {
                                data.drivers.CCNETDriver.ReturnBill();
                            }

                            if (amount == 0)
                            {
                                pBxReturnBack.Enabled = true;
                            }

                            // сообщим о том что купюра великовата
                            SecondMessageText.Text = "Внесите купюру меньшего номинала.";

                            data.log.Write(LogMessageType.Information, "WAIT BILL: Внесите купюру меньшего номинала. Нет сдачи.");

                            return;
                        }
                    }
                    else
                    {
                        // со сдачей
                        if (amount > data.serv.price)
                        {
                            // купюра великовата - спросим может вернуть ее
                            res = (bool)FormManager.OpenForm<FormInsertBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, data, ((BillNominal)e.Message.Content).Denomination);
                        }
                    }


                    if (res)
                    {
                        // забираем купюру
                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            moneyFixed = false;
                            data.drivers.CCNETDriver.StackBill();
                        }
                    }
                    else
                    {
                        // возвращаем ее обратно
                        amount -= count;

                        moneyFixed = false;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.ReturnBill();
                        }

                        if (amount == 0)
                        {
                            pBxReturnBack.Enabled = true;
                        }

                        return;
                    }

                    // внесли достаточную для услуги сумму
                    data.log.Write(LogMessageType.Information, "WAIT BILL: Внесли достаточную для оказания услуги сумму.");

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {
                        // посчитаем размер сдачи
                        diff = amount - data.serv.price;

                        // денег не достаточно - сдачи нет
                        if (diff < 0) diff = 0;

                        // сдача на чек
                        if (amount > data.serv.price)
                        {
                            data.log.Write(LogMessageType.Information, "WAIT BILL: Сумма сдачи " + diff + " руб.");

                            ChooseChangeEnum ch = ChooseChangeEnum.None;

                            if (Globals.ClientConfiguration.Settings.changeToAccount == 1 && Globals.ClientConfiguration.Settings.changeToCheck == 1)
                            {
                                // тут надо решить как выдать сдачу - спросим пользователя
                                ch = (ChooseChangeEnum)FormManager.OpenForm<FormChooseChange>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, diff.ToString());
                            }
                            else if (Globals.ClientConfiguration.Settings.changeToAccount == 1)
                            {
                                ch = ChooseChangeEnum.ChangeToAccount;
                            }
                            else if (Globals.ClientConfiguration.Settings.changeToCheck == 1)
                            {
                                ch = ChooseChangeEnum.ChangeToCheck;
                            }

                            if (ch == ChooseChangeEnum.ChangeToAccount)
                            {
                                data.log.Write(LogMessageType.Information, "WAIT BILL: сдача на аккаунт. Сумма сдачи " + diff);

                                // заносим в аккаунт - если не авторизовались - нужна авторизация в аккаунт
                                if (data.CurrentUserId == 0)
                                {
                                    // форма регистрации
                                    data = (FormResultData)FormManager.OpenForm<UserRequest>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, data);
                                }

                                // запомним сколько внесли на аккаунт
                                data.statistic.AccountMoneySumm += diff;

                                difftoAccount += diff;

                                // внесем на счет
                                GlobalDb.GlobalBase.AddToAmount(data.CurrentUserId, diff);
                            }
                            else
                            {
                                // выдаем чек
                                data.log.Write(LogMessageType.Information, "WAIT BILL: сдача на чек. Сумма сдачи " + diff);

                                // запомним сколько выдали на чеке
                                data.statistic.BarCodeMoneySumm += diff;

                                difftoCheck += diff;

                                // запомним такой чек
                                string check = CheckHelper.GetUniqueNumberCheck(12);
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                                // и напечатем его
                                data.drivers.printer.PrintHeader(true);
                                data.drivers.printer.PrintBarCode(check,diff);
                                data.drivers.printer.PrintFooter();
                                data.drivers.printer.EndPrint();
                            }
                        }
                    }

                    // напишем на экране
                    AmountServiceText.Text = "Внесено: " + amount + " руб.";

                    data.log.Write(LogMessageType.Information, "WAIT BILL: Внесено " + amount + " руб.");

                    // деньги внесли - нет пути назад
                    TimeOutTimer.Enabled = false;

                    // Количество банкнот
                    data.statistic.CountBankNote++;
                    // заносим банкноту
                    GlobalDb.GlobalBase.InsertBankNote();

                    if (amount >= data.serv.price)
                    {
                        AmountServiceText.ForeColor = System.Drawing.Color.Green;
                        pBxGiveOxigen.Enabled = true;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.StopWaitBill();
                            moneyFixed = false;
                        }
                    }
                }
                catch (Exception exp)
                {
                    data.log.Write(LogMessageType.Error, "WAIT BILL: ошибка управляющего устройства.");
                    data.log.Write(LogMessageType.Error, "WAIT BILL: " + exp.ToString());

                    // если какая либо ошибка - вернем купюру
                    data.drivers.CCNETDriver.ReturnBill();

                    moneyFixed = false;
                }
            }
        }

        private void FormWaitPayBill1_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;

            data.drivers.ReceivedResponse -= reciveResponse;

            if (Globals.ClientConfiguration.Settings.offHardware == 0)
            {
                // вернем деньгу
                moneyFixed = false;

                data.drivers.CCNETDriver.ReturnBill();
                data.drivers.CCNETDriver.StopWaitBill();
            }

            Params.Result = data;

            data.log.Write(LogMessageType.Information, "WAIT BILL: Выход на оказание услуги.");
        }

        int Timeout = 0;

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            Timeout++;

            if (Globals.ClientConfiguration.Settings.timeout == 0)
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

        private void pBxGiveOxigen_Click(object sender, EventArgs e)
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
                    data.drivers.printer.PrintBody(data.serv, amount);
                    data.drivers.printer.PrintFooter();
                    data.drivers.printer.EndPrint();

                    int numbercheck = GlobalDb.GlobalBase.GetCurrentNumberCheck();

                    // увеличим номер фискального чека
                    GlobalDb.GlobalBase.SetNumberCheck(numbercheck + 1);

                    data.log.Write(LogMessageType.Information, "WAIT BILL: Печатаем чек с номером " + numbercheck + ". На сумму " + amount + " руб.");
                }
            }

            // запомним принятую купюру
            data.statistic.AllMoneySumm += (amount);
            // запомним на сколько оказали услуг
            data.statistic.ServiceMoneySumm += data.serv.price;

            // Запомним в базе принятую купюру
            GlobalDb.GlobalBase.SetMoneyStatistic(data.statistic);
            // заносим в базу платеж
            GlobalDb.GlobalBase.InsertMoney(data.CurrentUserId, (amount));
            // запоминаем оказанную услугу этому пользователю
            GlobalDb.GlobalBase.InsertService(data.CurrentUserId, (data.serv.price));

            data.log.Write(LogMessageType.Information, "WAIT BILL: Приняли " + amount + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Окажем услугу на сумму " + data.serv.price + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Оказываем услугу.");

            this.Close();
        }

        private void pBxReturnBack_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormWaitPayBill1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void LabelNameService1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
            else if (e.Alt & e.KeyCode == Keys.F5)
            {
                if(Globals.IsDebug)
                {
                    // в дебаге - вносим деньги руками
                    Drivers.Message message = new Drivers.Message();

                    message.Content = new BillNominal();

                    ((BillNominal)message.Content).nominalNumber = 3;
                    ((BillNominal)message.Content).Denomination = "500";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    CreditMoney(e1);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pBxGiveOxigen.Image = gifImage.GetNextFrame();
            //timer1.Enabled = false;
        }
    }
}
