﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;
using System.Drawing.Imaging;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitPayBill1 : MyForm
    {
        FormResultData data;

        // количество внесенных денег
        int amount;

        // зафиксировали купюру
        bool moneyFixed = false;

        public FormWaitPayBill1()
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
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
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

                            // сообщим о том что купюра великовата
                            SecondMessageText.Text = "Внесите купюру меньшего номинала.";
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
                        return;
                    }

                    // внесли достаточную для услуги сумму

                    if (Globals.ClientConfiguration.Settings.changeOn > 0)
                    {
                        // сдача на чек
                        if (amount > data.serv.price)
                        {
                            // посчитаем размер сдачи
                            diff = amount - data.serv.price;

                            // тут надо решить как выдать сдачу - спросим пользователя
                            ChooseChangeEnum ch = (ChooseChangeEnum)FormManager.OpenForm<FormChooseChange>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, diff.ToString());

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
                                string check = CheckHelper.GetUniqueNumberCheck(10);
                                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, diff, check);

                                // и напечатем его
                                data.drivers.printer.PrintBarCode(check);
                            }
                        }
                    }

                    // напишем на экране
                    AmountServiceText.Text = "Внесено: " + amount + " руб.";

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
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pBxGiveOxigen.Image = gifImage.GetNextFrame();
            //timer1.Enabled = false;
        }
    }
}
