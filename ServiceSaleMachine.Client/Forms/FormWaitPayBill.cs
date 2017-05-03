using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using System.Drawing;

namespace AirVitamin.Client
{
    public partial class FormWaitPayBill : MyForm
    {
        FormResultData data;

        // количество внесенных денег
        int amount;

        // количество денег на купюрах
        int amountMoney;
        // количество денег на чеках
        int amountCheck;

        // положили на чек
        int difftoCheck = 0;
        // положили на аккаунт
        int difftoAccount = 0;

        // зафиксировали купюру
        bool moneyFixed = false;

        // остановка оборудования - больше денег не надо
        bool StopHardware = false;

        public FormWaitPayBill()
        {
            InitializeComponent();

            TimeOutTimer.Enabled = true;
            Timeout = 0;
            amount = 0;
            amountMoney = 0;
            amountCheck = 0;

            if (Globals.ClientConfiguration.Settings.offHardware == 0 && Globals.ClientConfiguration.Settings.offBill == 0)
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

            data.log.Write(LogMessageType.Information, "========================НАЧАЛО ОБСЛУЖИВАНИЯ==========================");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxGiveOxigen, Globals.DesignConfiguration.Settings.ButtonGetOxigen);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMainMenu, "Menu_big.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle1, "Smes_txt.png");

            if (data.numberService == 0)
            {
                // до тренировки
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Do_tren_ver.png");
            }
            else
            {
                // после тренировки
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "");
            }

            AmountServiceText.Font = new Font(CustomFont.GetCustomFont(Properties.Resources.CeraRoundPro_Bold).Families[0], 72);
            AmountServiceText.Text = "Внесено: 0 руб.";
            AmountServiceText.ForeColor = Color.FromArgb(0,158,227);

            SecondMessageText.Font = new Font(CustomFont.GetCustomFont(Properties.Resources.CeraRoundPro_Medium).Families[0], 72);
            SecondMessageText.ForeColor = Color.Gray;
            SecondMessageText.Text = "";

            TextPayBill.ForeColor = Color.Gray;
            TextPayBill.Font = new Font(CustomFont.GetCustomFont(Properties.Resources.CeraRoundPro_Thin).Families[0],14);
            TextPayBill.LoadFile(Globals.GetPath(PathEnum.Text) + "\\WaitPayBill.rtf");

            // сразу проверим - если авторизовались и достаточно денег на счете - сразу списываем деньги со счета
            if (data.retLogin != "")
            {
                int sum = GlobalDb.GlobalBase.GetUserMoney(data.CurrentUserId);

                data.log.Write(LogMessageType.Information, "ACCOUNT: На счету у пользователя " + data.retLogin + " " + sum + " руб.");

                if (sum >= data.serv.price)
                {
                    // денег на счете достаточно
                    data.log.Write(LogMessageType.Information, "ACCOUNT: Оказываем услугу с денег со счета...");

                    amount += data.serv.price;

                    AmountServiceText.Text = "Внесено: " + data.serv.price + " руб.";
                    AmountServiceText.ForeColor = Color.Green;
                    SecondMessageText.Text = "                Остаток на счете: " + (sum - data.serv.price) + " руб.";

                    data.log.Write(LogMessageType.Information, "ACCOUNT: Внесли достаточную для оказания услуги сумму со счета.");

                    // все можно уже пользоваться
                    pBxGiveOxigen.Enabled = true;

                    moneyFixed = false;
                    StopHardware = true;

                    // обновим счет
                    GlobalDb.GlobalBase.AddToAmount(data.CurrentUserId, 0 - data.serv.price);

                    pBxMainMenu.Enabled = false;

                    return;
                }
                else if (sum > 0)
                {
                    // денег не достаточно - все равно списываем все подчистую
                    data.log.Write(LogMessageType.Information, "ACCOUNT: Оказываем услугу с денег со счета...");

                    amount += sum;

                    AmountServiceText.Text = "Внесено: " + sum + " руб.";
                    SecondMessageText.Text = "                Недостаточно денег для оказания услуги";

                    // обновим счет
                    GlobalDb.GlobalBase.AddToAmount(data.CurrentUserId, 0 - sum);

                    pBxMainMenu.Enabled = false;
                }
            }
            //

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
                if (data.drivers.CCNETDriver.NoConnectBill == false)
                {
                    data.drivers.CCNETDriver.WaitBillEscrow();
                    data.log.Write(LogMessageType.Information, "WAIT BILL: запускаем режим ожидания купюр.");
                }

                // при старте сканер разбудим, если не отключена возможность оплаты чеком
                if (Globals.ClientConfiguration.Settings.offCheck != 1)
                {
                    data.drivers.scaner.Request(ZebexCommandEnum.wakeUp);
                    data.log.Write(LogMessageType.Information, "WAIT CHECK: запускаем режим ожидания чеков.");
                }
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
                data.log.Write(LogMessageType.Debug, "WAIT BILL: Событие: " + e.Message.Content + ".");
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.BillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorEscrow:
                    CreditMoney(e);
                    break;
                case DeviceEvent.Scaner:
                    CreditCheck(e);
                    break;
                case DeviceEvent.BillAcceptorCredit:
                    //CreditMoney(e);
                    break;
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        if (data.log != null) data.log.Write(LogMessageType.Debug, "WAIT BILL: Вытащили купюроприемник.");

                        this.Close();
                    }
                    break;
                case DeviceEvent.ConnectBillError:
                    {
                        // нет связи с купюроприемником
                        data.stage = WorkerStateStage.ErrorBill;
                        if (data.log != null) data.log.Write(LogMessageType.Error, "WAIT BILL: Нет связи с купюроприемником.");

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
            if (StopHardware) return;

            pBxMainMenu.Enabled = false;
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

                        data.log.Write(LogMessageType.Information, "WAIT BILL: Купюру c номером номинала равным " + numberNominal + " не принимаем");

                        SecondMessageText.Text = "                Внесите купюру другого номинала.";
                        return;
                    }

                    //  прошли проверку

                    // внесли деньги
                    int count = 0;
                    int.TryParse(((BillNominal)e.Message.Content).Denomination, out count);

                    data.log.Write(LogMessageType.Information, "WAIT BILL: Внесли купюру номиналом " + count + " руб.");

                    amount += count;
                    amountMoney += count;

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
                            amountMoney -= count;

                            moneyFixed = false;

                            if (Globals.ClientConfiguration.Settings.offHardware == 0)
                            {
                                data.drivers.CCNETDriver.ReturnBill();
                            }

                            if (amount == 0)
                            {
                                pBxMainMenu.Enabled = true;
                            }

                            // сообщим о том что купюра великовата
                            SecondMessageText.Text = "                Внесите купюру меньшего номинала.";

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

                            data.log.Write(LogMessageType.Information, "WAIT BILL: Забираем купюру в приемник.");
                        }
                        else
                        {
                            moneyFixed = false;
                        }
                    }
                    else
                    {
                        // возвращаем ее обратно
                        amount -= count;
                        amountMoney -= count;

                        data.log.Write(LogMessageType.Information, "WAIT BILL: Возвращаем купюру обратно покупателю.");

                        moneyFixed = false;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.ReturnBill();
                        }

                        if (amount == 0)
                        {
                            pBxMainMenu.Enabled = true;
                        }

                        return;
                    }

                    ChooseChangeEnum ch = ChooseChangeEnum.None;

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

                            if (data.retLogin != "" && Globals.ClientConfiguration.Settings.changeToAccount == 1)
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

                                // Здесь также наращивается номер чека
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                                // и напечатем его
                                data.drivers.printer.PrintHeader(1);
                                data.drivers.printer.PrintBarCode(check,diff);
                                data.drivers.printer.PrintFooter();
                                data.drivers.printer.EndPrint();
                            }
                        }
                    }

                    // напишем на экране
                    if (amount >= data.serv.price)
                    {
                        if (diff > 0)
                        {
                            AmountServiceText.Text = "ПРИНЯТО: " + data.serv.price + " руб.";

                            if (ch == ChooseChangeEnum.ChangeToAccount)
                            {
                                SecondMessageText.Text = "Сдача в размере " + diff + " руб. будет зачислена на Ваш аккаунт";
                            }
                            else
                            {
                                SecondMessageText.Text = "Остаток на чеке: " + diff + " руб.";
                            }
                        }
                        else
                        {
                            AmountServiceText.Text = "Внесено: " + amount + " руб.";
                            SecondMessageText.Text = "";
                        }
                    }
                    else
                    {
                        AmountServiceText.Text = "Внесено: " + amount + " руб.";
                        SecondMessageText.Text = "                Недостаточно денег для оказания услуги";
                    }

                    data.log.Write(LogMessageType.Information, "WAIT BILL: Внесено " + amount + " руб.");

                    // деньги внесли - нет пути назад
                    TimeOutTimer.Enabled = false;

                    // Количество банкнот
                    data.statistic.CountBankNote++;
                    // заносим банкноту
                    GlobalDb.GlobalBase.InsertBankNote();

                    if (amount >= data.serv.price)
                    {
                        // внесли достаточную для услуги сумму
                        data.log.Write(LogMessageType.Information, "WAIT BILL: Внесли достаточную для оказания услуги сумму.");

                        AmountServiceText.ForeColor = System.Drawing.Color.Green;
                        pBxGiveOxigen.Enabled = true;

                        moneyFixed = false;
                        StopHardware = true;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.ReturnBill();
                            data.drivers.scaner.Request(ZebexCommandEnum.sleep);

                            data.log.Write(LogMessageType.Information, "WAIT BILL: Остановим работу оборудования для приема средств.");
                        }
                    }
                }
                catch (Exception exp)
                {
                    data.log.Write(LogMessageType.Error, "WAIT BILL: ошибка управляющего устройства.");
                    data.log.Write(LogMessageType.Error, "WAIT BILL: " + exp.GetDebugInformation());

                    // если какая либо ошибка - вернем купюру
                    data.drivers.CCNETDriver.ReturnBill();

                    moneyFixed = false;
                }
            }
        }

        private void CreditCheck(ServiceClientResponseEventArgs e)
        {
            if (StopHardware) return;

            pBxMainMenu.Enabled = false;   // блокировать возврат не будем - можем ведь чеком сдачу давать
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
                    CheckInfo info;

                    if (((string)e.Message.Content).Contains("0000000000"))
                    {
                        info = new CheckInfo();
                        info.active = false;
                        info.Amount = 50;
                    }
                    else
                    {
                        info = GlobalDb.GlobalBase.GetCheckByStr((string)e.Message.Content);
                    }

                    data.log.Write(LogMessageType.Information, "WAIT CHECK: Внесли чек номер " + (string)e.Message.Content);

                    if (info == null)
                    {
                        // нет такого чека
                        SecondMessageText.Text = "              Чек не существует.         ";

                        if (amount == 0)
                        {
                            pBxMainMenu.Enabled = true;
                        }

                        data.log.Write(LogMessageType.Information, "WAIT CHECK: Чек не существует.");

                        return;
                    }

                    if (info.active == true)
                    {
                        // чек погашен - отклоним его
                        SecondMessageText.Text = "Чек уже был использован ранее";

                        if (amount == 0)
                        {
                            pBxMainMenu.Enabled = true;
                        }

                        data.log.Write(LogMessageType.Information, "WAIT CHECK: Чек уже был использован ранее.");

                        return;
                    }

                    int count = info.Amount;

                    amount += count;
                    amountCheck += count;

                    // всегда со сдачей - сразу забираем деньгу c чека - никого не спрашиваем - гасим текущий чек - распечатаем новый
                    GlobalDb.GlobalBase.FixedCheck(info.Id);

                    // сдача
                    int diff = 0;
                    ChooseChangeEnum ch = ChooseChangeEnum.None;

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {

                        // посчитаем размер сдачи
                        diff = amount - data.serv.price;

                        // денег не достаточно - сдачи нет
                        if (diff < 0) diff = 0;

                        // сдача на чек
                        if (amount > data.serv.price)
                        {
                            if (data.retLogin != "" && Globals.ClientConfiguration.Settings.changeToAccount == 1)
                            {
                                ch = ChooseChangeEnum.ChangeToAccount;
                            }
                            else if (Globals.ClientConfiguration.Settings.changeToCheck == 1)
                            {
                                ch = ChooseChangeEnum.ChangeToCheck;
                            }

                            if (ch == ChooseChangeEnum.ChangeToAccount)
                            {
                                data.log.Write(LogMessageType.Information, "WAIT CHECK: сдача на аккаунт. Сумма сдачи " + diff);

                                // запомним сколько внесли на аккаунт
                                data.statistic.AccountMoneySumm += diff;

                                difftoAccount += diff;

                                // внесем на счет
                                GlobalDb.GlobalBase.AddToAmount(data.CurrentUserId, diff);
                            }
                            else
                            {
                                // выдаем чек
                                data.log.Write(LogMessageType.Information, "WAIT CHECK: сдача на чек. Сумма сдачи " + diff);

                                // запомним сколько выдали на чеке - печатаем новый чек - часть денег отоварили
                                data.statistic.BarCodeMoneySumm -= data.serv.price;

                                difftoCheck += diff;

                                // запомним такой чек
                                string check = CheckHelper.GetUniqueNumberCheck(12);

                                // Здесь также наращивается номер чека
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                                int numberCheck = GlobalDb.GlobalBase.GetCurrentNumberDeliveryCheck();

                                // и напечатем его
                                data.drivers.printer.PrintHeader(1);
                                data.drivers.printer.PrintBarCode(check, diff);
                                data.drivers.printer.PrintFooter();
                                data.drivers.printer.EndPrint();

                                data.log.Write(LogMessageType.Information, "WAIT CHECK: Печатаем чек со сдачей под номером " + numberCheck + ". На сумму " + diff + " руб.");
                                data.log.Write(LogMessageType.Information, "WAIT CHECK: BarCode " + check);
                                data.log.Write(LogMessageType.Information, "WAIT CHECK: Выход на оказание услуги.");
                            }
                        }
                    }

                    // напишем на экране
                    if (amount >= data.serv.price)
                    {
                        if (diff > 0)
                        {
                            AmountServiceText.Text = "ПРИНЯТО: " + data.serv.price + " руб.";

                            if (ch == ChooseChangeEnum.ChangeToAccount)
                            {
                                SecondMessageText.Text = "Сдача в размере " + diff + " руб. будет зачислена на Ваш аккаунт";
                            }
                            else
                            {
                                SecondMessageText.Text = "Остаток на чеке: " + diff + " руб.";
                            }
                        }
                        else
                        {
                            AmountServiceText.Text = "Внесено: " + amount + " руб.";
                            SecondMessageText.Text = "";
                        }
                    }
                    else
                    {
                        AmountServiceText.Text = "Внесено: " + amount + " руб.";
                        SecondMessageText.Text = "               Недостаточно денег для оказания услуги";
                    }

                    data.log.Write(LogMessageType.Information, "WAIT CHECK: Внесено " + amount + " руб.");

                    // деньги внесли - нет пути назад
                    TimeOutTimer.Enabled = false;

                    if (amount >= data.serv.price)
                    {
                        // внесли достаточную для услуги сумму
                        data.log.Write(LogMessageType.Information, "WAIT CHECK: Внесли достаточную для оказания услуги сумму.");

                        AmountServiceText.ForeColor = System.Drawing.Color.Green;
                        pBxGiveOxigen.Enabled = true;

                        StopHardware = true;

                        if (Globals.ClientConfiguration.Settings.offHardware == 0)
                        {
                            data.drivers.CCNETDriver.ReturnBill();
                            data.drivers.scaner.Request(ZebexCommandEnum.sleep);

                            data.log.Write(LogMessageType.Information, "WAIT BILL: Остановим работу оборудования для приема средств.");
                        }
                    }
                }
                catch (Exception exp)
                {
                    data.log.Write(LogMessageType.Error, "WAIT CHECK: ошибка управляющего устройства.");
                    data.log.Write(LogMessageType.Error, "WAIT CHECK: " + exp.GetDebugInformation());

                    // при ошибке сканер усыпим
                    data.drivers.scaner.Request(ZebexCommandEnum.sleep);
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

                if (data.drivers.CCNETDriver.NoConnectBill == false)
                {
                    data.drivers.CCNETDriver.ReturnBill();
                    data.drivers.CCNETDriver.StopWaitBill();
                }

                // при завершении сканер усыпим
                data.drivers.scaner.Request(ZebexCommandEnum.sleep);
            }

            Params.Result = data;

            if (amount >= data.serv.price)
            {
                data.log.Write(LogMessageType.Information, "WAIT BILL: Выход на оказание услуги.");
            }
            else
            {
                data.log.Write(LogMessageType.Information, "WAIT BILL: Услугу не оказываем.");
            }
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
                if (amountMoney > 0)
                {
                    // Распечатать чек за услугу - если были потрачены деньги
                    data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                    if (data.drivers.printer.prn.PrinterIsOpen)
                    {
                        data.drivers.printer.PrintHeader();
                        data.drivers.printer.PrintBody(data.serv, amountMoney);
                        data.drivers.printer.PrintFooter();
                        data.drivers.printer.EndPrint();

                        int numbercheck = GlobalDb.GlobalBase.GetCurrentNumberCheck();

                        // увеличим номер фискального чека
                        GlobalDb.GlobalBase.SetNumberCheck(numbercheck + 1);

                        data.log.Write(LogMessageType.Information, "WAIT BILL: Печатаем фискальный чек с номером " + numbercheck + ". На сумму " + amountMoney + " руб.");
                    }
                }

                int sum = GlobalDb.GlobalBase.GetUserMoney(data.CurrentUserId);

                // если зарегистрированы - чек с остатком на аккаунте тоже даем, если есть остаток
                if (data.retLogin != "" && sum > 0)
                {
                    // Распечатать чек c информацией по аккаунту - платили с аккаунта
                    data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

                    if (data.drivers.printer.prn.PrinterIsOpen)
                    {
                        data.drivers.printer.PrintHeader();
                        data.drivers.printer.PrintAccountBody(data.serv, sum, data.retLogin);
                        data.drivers.printer.PrintFooter();
                        data.drivers.printer.EndPrint();

                        int numbercheck = GlobalDb.GlobalBase.GetCurrentAccountNumberCheck();

                        // увеличим номер отчетного
                        GlobalDb.GlobalBase.SetNumberAccountCheck(numbercheck + 1);
                    }
                }
            }

            // запомним принятую купюру
            data.statistic.AllMoneySumm += (amountMoney);
            // запомним на сколько оказали услуг
            data.statistic.ServiceMoneySumm += data.serv.price;

            // Запомним в базе принятую купюру
            GlobalDb.GlobalBase.SetMoneyStatistic(data.statistic);

            if (amountMoney > 0)
            {
                // заносим в базу платеж - если платили купюрами
                GlobalDb.GlobalBase.InsertMoney(data.CurrentUserId, (amountMoney));
            }

            // нарастим глобальные счетчики
            int count = GlobalDb.GlobalBase.GetAmountMoney();
            GlobalDb.GlobalBase.SetAmountMoney(count + amountMoney);

            count = GlobalDb.GlobalBase.GetAmountService();
            GlobalDb.GlobalBase.SetAmountService(count + data.serv.price);

            // запоминаем оказанную услугу этому пользователю
            GlobalDb.GlobalBase.InsertService(data.CurrentUserId, (data.serv.price));

            data.log.Write(LogMessageType.Information, "==============================================================================");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Приняли " + amount + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Купюрами " + amountMoney + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Чеками " + amountCheck + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Дали сдачу чеком на сумму " + difftoCheck + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Окажем услугу на сумму " + data.serv.price + " руб.");
            data.log.Write(LogMessageType.Information, "WAIT BILL: Оказываем услугу.");
            data.log.Write(LogMessageType.Information, "==============================================================================");

            // обновим из базы статистические данные
            data.statistic = GlobalDb.GlobalBase.GetMoneyStatistic();

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
            else if (e.Alt & e.KeyCode == Keys.F6)
            {
                if (Globals.IsDebug)
                {
                    // в дебаге - вносим деньги руками
                    Drivers.Message message = new Drivers.Message();

                    message.Content = new BillNominal();

                    ((BillNominal)message.Content).nominalNumber = 3;
                    ((BillNominal)message.Content).Denomination = "50";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    CreditMoney(e1);
                }
            }
            else if (e.Alt & e.KeyCode == Keys.F7)
            {
                if (Globals.IsDebug)
                {
                    // в дебаге - вносим деньги руками
                    Drivers.Message message = new Drivers.Message();

                    message.Content = "0000000000";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    CreditCheck(e1);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //pBxGiveOxigen.Image = gifImage.GetNextFrame();
            //timer1.Enabled = false;
        }
    }
}
