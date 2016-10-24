using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitPayCheck : MyForm
    {
        FormResultData data;

        // количество внесенных денег
        int amount;

        // положили на чек
        int difftoCheck = 0;
        // положили на аккаунт
        int difftoAccount = 0;

        int Timeout = 0;

        private GifImage gifImage = null;

        public FormWaitPayCheck()
        {
            InitializeComponent();

            TimeOutTimer.Enabled = true;
            Timeout = 0;
            amount = 0;

            if (Globals.ClientConfiguration.Settings.offHardware == 0 && Globals.ClientConfiguration.Settings.offCheck == 0)
            {
                // пока не внесли нужную сумму - не жамкаем кнопку
                pBxGiveOxigen.Enabled = false;
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

            gifImage = new GifImage(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonGetOxigen);
            gifImage.ReverseAtEnd = false; //dont reverse at end

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxReturnBack, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            AmountServiceText.Text = "Внесено: 0 руб.";
            AmountServiceText.ForeColor = System.Drawing.Color.Red;
            SecondMessageText.Text = "";

            LabelNameService2.Text = Globals.ClientConfiguration.Settings.services[data.numberService].caption;

            TextPayBill.LoadFile(Globals.GetPath(PathEnum.Text) + "\\WaitPayCheck.rtf");

            // заменим обработчик событий
            data.drivers.ReceivedResponse += reciveResponse;

            // при старте сканер разбудим
            data.drivers.scaner.Request(ZebexCommandEnum.wakeUp);
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            if (data.log != null)
            {
                data.log.Write(LogMessageType.Information, "WAIT CHECK: Событие: " + e.Message.Content + ".");
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.Scaner:
                    CreditMoney(e);
                    break;
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        data.log.Write(LogMessageType.Information, "WAIT CHECK: Вытащили купюроприемник.");

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
            pBxReturnBack.Enabled = true;   // блокировать возврат не будем - можем ведь чеком сдачу давать
            SecondMessageText.Text = "";

            lock (Params)
            {
                try
                {
                    data.log.Write(LogMessageType.Information, "SCANER: Обработаем получение чека");

                    // сбросим таймаут
                    Timeout = 0;

                    // прочли чек

                    // поищем такой чек
                    CheckInfo info = GlobalDb.GlobalBase.GetCheckByStr((string)e.Message.Content);

                    if (info == null)
                    {
                        // нет такого чека
                        SecondMessageText.Text = "Чек не существует.";

                        if (amount == 0)
                        {
                            pBxReturnBack.Enabled = true;
                        }

                        return;
                    }

                    if(info.active == true)
                    {
                        // чек погашен - отклоним его
                        SecondMessageText.Text = "Чек уже был использован ранее.";

                        if (amount == 0)
                        {
                            pBxReturnBack.Enabled = true;
                        }

                        return;
                    }

                    int count = info.Amount;

                    if (count < data.serv.price)
                    {
                        // на чеке слишком мало денег - пока так - может в будущем и по другому сделаем
                        AmountServiceText.Text = "Недостаточно денег";
                        SecondMessageText.Text = "Остаток на чеке: " + count + " руб.";

                        return;
                    }

                    amount += count;

                    // всегда со сдачей - сразу забираем деньгу c чека - никого не спрашиваем - гасим текущий чек - распечатаем новый
                    GlobalDb.GlobalBase.FixedCheck(info.Id);

                    // внесли достаточную для услуги сумму

                    // сдача
                    int diff = 0;

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {
                        // посчитаем размер сдачи
                        diff = amount - data.serv.price;

                        // денег не достаточно - сдачи нет
                        if (diff < 0) diff = 0;

                        // сдача на чек
                        if (amount > data.serv.price)
                        {
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

                                // запомним сколько выдали на чеке - печатаем новый чек - часть денег отоварили
                                data.statistic.BarCodeMoneySumm -= data.serv.price;

                                difftoCheck += diff;

                                // запомним такой чек
                                string check = CheckHelper.GetUniqueNumberCheck(12);
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                                // и напечатем его
                                data.drivers.printer.PrintHeader(true);
                                data.drivers.printer.PrintBarCode(check, diff);
                                data.drivers.printer.PrintFooter();
                                data.drivers.printer.EndPrint();
                            }
                        }
                    }

                    // напишем на экране
                    if (amount >= data.serv.price)
                    {
                        AmountServiceText.Text = "ПРИНЯТО: " + data.serv.price + " руб.";

                        if (diff > 0)
                        {
                            SecondMessageText.Text = "Остаток на чеке: " + diff + " руб.";
                        }
                        else
                        {
                            SecondMessageText.Text = "";
                        }
                    }
                    else
                    {
                        AmountServiceText.Text = "ПРИНЯТО: " + amount + " руб.";
                        SecondMessageText.Text = "Недостаточно денег для оказания услуги.";
                    }

                    // деньги внесли - нет пути назад
                    TimeOutTimer.Enabled = false;

                    if (amount >= data.serv.price)
                    {
                        AmountServiceText.ForeColor = System.Drawing.Color.Green;
                        pBxGiveOxigen.Enabled = true;
                    }
                }
                catch (Exception exp)
                {
                    data.log.Write(LogMessageType.Error, "WAIT CHECK: ошибка управляющего устройства.");
                    data.log.Write(LogMessageType.Error, "WAIT CHECK: " + exp.ToString());

                    // при ошибке сканер усыпим
                    data.drivers.scaner.Request(ZebexCommandEnum.sleep);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormWaitPayCheck_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;

            data.drivers.ReceivedResponse -= reciveResponse;

            Params.Result = data;

            // при завершении сканер усыпим
            data.drivers.scaner.Request(ZebexCommandEnum.sleep);

            // пока этого нет
            //if (amount < data.serv.price && amount > 0)
            //{
            //    // не внесли необходимую сумму - но хотим уйти - вернем чек на всю введенную ранее сумму

            //    // запомним такой новый чек
            //    string check = CheckHelper.GetUniqueNumberCheck(12);
            //    GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, amount, check);

            //    data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

            //    // и напечатем его
            //    data.drivers.printer.PrintHeader(true);
            //    data.drivers.printer.PrintBarCode(check, amount);
            //    data.drivers.printer.PrintFooter();
            //    data.drivers.printer.EndPrint();
            //}

            data.log.Write(LogMessageType.Information, "WAIT CHECK: Выход на оказание услуги.");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // запуск услуги
            data.stage = WorkerStateStage.StartService;
            this.Close();
        }

        private void FormWaitPayCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
                this.Close();
            }
            else if (e.Alt & e.KeyCode == Keys.F5)
            {
                if (Globals.IsDebug)
                {
                    // в дебаге - вносим деньги руками
                    Drivers.Message message = new Drivers.Message();

                    message.Content = "834141186725";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    CreditMoney(e1);
                }
            }
            else if (e.Alt & e.KeyCode == Keys.F6)
            {
                if (Globals.IsDebug)
                {
                    // в дебаге - вносим деньги руками
                    Drivers.Message message = new Drivers.Message();

                    message.Content = "047759717704";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    CreditMoney(e1);
                }
            }
        }

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
                // Распечатать чек за услугу печатать не надо - деньги то не принимаем же
            }

            // запомним на сколько оказали услуг
            data.statistic.ServiceMoneySumm += data.serv.price;

            // запоминаем оказанную услугу этому пользователю
            GlobalDb.GlobalBase.InsertService(data.CurrentUserId, (data.serv.price));

            // Запомним в базе
            GlobalDb.GlobalBase.SetMoneyStatistic(data.statistic);

            data.log.Write(LogMessageType.Information, "WAIT CHECK: Оказываем услугу.");

            this.Close();
        }

        private void pBxReturnBack_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pBxGiveOxigen.Image = gifImage.GetNextFrame();
        }
    }
}
