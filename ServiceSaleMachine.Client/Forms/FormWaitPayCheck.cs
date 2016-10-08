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
            pBxReturnBack.Enabled = false;
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
                        return;
                    }

                    if(info.active == true)
                    {
                        // чек погашен - отклоним его
                        SecondMessageText.Text = "Чек уже был использован ранее.";
                        return;
                    }

                    int count = info.Amount;

                    amount += count;

                    // сдача
                    int diff = 0;
                    bool res = true;    // чек забираем всегда - предлагаем вернуть - только если перебор

                    if (Globals.ClientConfiguration.Settings.changeOn == 0)
                    {
                        // работа без сдачи
                        if (amount > data.serv.price)
                        {
                            // сумма на чеке великовата - вернем ее
                            amount -= count;

                            if (amount == 0)
                            {
                                pBxReturnBack.Enabled = true;
                            }

                            // сообщим о том что купюра великовата
                            SecondMessageText.Text = "Внесите чек меньшего номинала.";
                            return;
                        }
                    }
                    else
                    {
                        // со сдачей
                        if (amount > data.serv.price)
                        {
                            // чек великоват - спросим может не стоит его гасить
                            res = (bool)FormManager.OpenForm<FormInsertBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, data, info.Amount, true);
                        }
                    }

                    if (res)
                    {
                        // гасим чек
                        GlobalDb.GlobalBase.FixedCheck(info.Id);
                    }
                    else
                    {
                        // не гасим чек - сумму откатываем
                        amount -= count;

                        if(amount == 0)
                        {
                            pBxReturnBack.Enabled = true;
                        }

                        return;
                    }

                    // внесли достаточную для услуги сумму

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {
                        // посчитаем размер сдачи
                        diff = amount - data.serv.price;

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

                                // внесем на счет
                                GlobalDb.GlobalBase.AddToAmount(data.CurrentUserId, diff);
                            }
                            else
                            {
                                // выдаем чек

                                // запомним сколько выдали на чеке
                                data.statistic.BarCodeMoneySumm += diff;

                                // запомним такой чек
                                string check = CheckHelper.GetUniqueNumberCheck(12);
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                // и напечатем его
                                data.drivers.printer.PrintBarCode(check,diff);
                            }
                        }
                    }

                    // напишем на экране
                    AmountServiceText.Text = "Внесено: " + amount + " руб.";

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
                // Распечатать чек за услугу
                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                if (data.drivers.printer.prn.PrinterIsOpen)
                {
                    data.drivers.printer.PrintHeader();
                    data.drivers.printer.PrintBody(data.serv);
                    data.drivers.printer.PrintFooter();
                    data.drivers.printer.EndPrint();

                    // увеличим номер чека
                    GlobalDb.GlobalBase.SetNumberCheck(GlobalDb.GlobalBase.GetCurrentNumberCheck() + 1);
                }
            }

            // запомним принятую сумму
            data.statistic.AllMoneySumm += amount;
            // запомним на сколько оказали услуг
            data.statistic.ServiceMoneySumm += data.serv.price;

            // Запомним в базе
            GlobalDb.GlobalBase.SetMoneyStatistic(data.statistic);
            // заносим в базу платеж
            GlobalDb.GlobalBase.InsertMoney(data.CurrentUserId, amount);

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
