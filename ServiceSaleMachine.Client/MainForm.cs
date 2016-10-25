using System;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : MyForm
    {
        /// <summary>
        /// задача очистки логов
        /// </summary>
        ClearFilesControlServiceTask ClearFilesTask { get; set; }

        /// <summary>
        /// Данные передаваемые между окнами
        /// </summary>
        FormResultData result;

        // запуск приложения
        public MainForm()
        {
            InitializeComponent();

            // сначала база данных
            if (GlobalDb.GlobalBase.CreateDB())
            {

            }
            else
            {

            }

            if (GlobalDb.GlobalBase.Connect())
            {

            }
            else
            {
                
            }

            GlobalDb.GlobalBase.CreateTables();
            GlobalDb.GlobalBase.FillSystemValues();

            // запустим задачу очистки от логов директории
            ClearFilesTask = new ClearFilesControlServiceTask(Program.Log);

            result = new FormResultData(Program.Log);
            // инициализируем задачи
            result.drivers.InitAllTask();
        }

        /// <summary>
        /// Основной обработчик
        /// </summary>
        private void MainWorker()
        {
            if (Globals.admin)
            {
                Program.Log.Write(LogMessageType.Information, "MAIN WORK: Входим в режим настройки приложения.");

                result.drivers.InitAllDevice();
                result = (FormResultData)FormManager.OpenForm<FormSettings>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выходим из режима настройки приложения.");

                // на выход сразу - не надо в настроечном режиме продолжать работать
                Close();
                return;
            }

            initDevice:

            if(Globals.ClientConfiguration.Settings.offHardware == 0)   // если не отключено
            {
                // инициализация оборудования
                switch (result.drivers.InitAllDevice())
                {
                    case WorkerStateStage.None:
                        break;
                    case WorkerStateStage.NoCOMPort:
                        //result = (FormResultData)FormManager.OpenForm<FormWelcomeUser>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Нет оборудования.");
                        break;
                    case WorkerStateStage.NeedSettingProgram:
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Не все оборудование настроено верно.");

                            result = (FormResultData)FormManager.OpenForm<FormSettings>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                            if (Globals.ClientConfiguration.Settings.offHardware == 0)
                            {
                                goto initDevice;
                            }
                        }
                        break;
                }
            }
            else
            {
                
            }

            bool check = true;

            while (true)
            {
                try
                {
                    // местный обработчик
                    result.drivers.ReceivedResponse += reciveResponse;

                    // забудем пользователя
                    result.CurrentUserId = 0;
                    result.stage = WorkerStateStage.None;

                    // Проверка статистических данных - может пора заканчивать работать
                    if (check)
                    {
                        result = CheckStatistic(result);
                    }

                    check = true;

                    if (result.stage != WorkerStateStage.None)
                    {
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Аппарат не работает.");

                        // аппарат не работает
                        result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }

                        continue;
                    }

                    // ожидание клиента
                    result = (FormResultData)FormManager.OpenForm<FormMainMenu>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.ErrorControl)
                    {
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Аппарат не работает.");

                        // аппарат временно не работает
                        result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }

                        continue;
                    }
                    else if (result.stage == WorkerStateStage.ChooseService)
                    {
                        // уходим на выбор услуг
                    }
                    else if (result.stage == WorkerStateStage.Rules || result.stage == WorkerStateStage.Philosof)
                    {
                        // ознакомление с правилами
                        result = (FormResultData)FormManager.OpenForm<FormRules>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.MainScreen)
                        {
                            // ознакомились - возвращаемся обратно
                            check = false;
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            // выемка денег
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.ChooseService)
                        {
                            // уходим на выбор услуг
                            check = false;
                        }
                    }
                    else if (result.stage == WorkerStateStage.DropCassettteBill)
                    {
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                        // выемка денег
                        result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.EndDropCassette)
                        {
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        // по тайм ауту вышли в рекламу
                        //result = (FormResultData)FormManager.OpenForm<FormWaitStage>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        //result = (FormResultData)FormManager.OpenForm<FormWaitClientVideo>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        result = (FormResultData)FormManager.OpenForm<FormWaitClientGif>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.ErrorControl)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Аппарат временно не работает.");

                            // аппарат временно не работает
                            result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                            if (result.stage == WorkerStateStage.ErrorEndControl)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }

                            continue;
                        }
                        else
                        {
                            // вышли из рекламы
                            check = false;
                            continue;
                        }
                    }

                    ChooseService:

                    // выбор услуг
                    result = (FormResultData)FormManager.OpenForm<FormChooseService1>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.DropCassettteBill)
                    {
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                        // выемка денег
                        result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.EndDropCassette)
                        {
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                    }
                    else if (result.stage == WorkerStateStage.UserRequestService)
                    {
                        // авторизация пользователя
                        result = (FormResultData)FormManager.OpenForm<UserRequest>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        // проверим результат
                        if (result.retLogin == "admin")
                        {
                            // вход админа
                            result = (FormResultData)FormManager.OpenForm<FormSettings>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            // проинициализируем железо после настроек
                            if (Globals.ClientConfiguration.Settings.offHardware == 0)
                            {
                                result.drivers.InitAllDevice();
                            }

                            continue;
                        }
                        else if (result.retLogin != "")
                        {
                            UserInfo ui = GlobalDb.GlobalBase.GetUserByName(result.retLogin, result.retPassword);
                            if (ui != null)
                            {
                                MessageBox.Show(ui.Role.ToString());
                            }
                        }

                        // вернемся в выбор услуги (уж не думал что goto буду использовать)
                        goto ChooseService;
                    }
                    else if (result.stage == WorkerStateStage.WhatsDiff)
                    {
                        result = (FormResultData)FormManager.OpenForm<FormWhatsDiff>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }

                        // вернемся в выбор услуги (уж не думал что goto буду использовать)
                        goto ChooseService;
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        check = false;
                        continue;
                    }

                    if (Globals.ClientConfiguration.Settings.offCheck != 1)
                    {
                        // выбор формы оплаты - если есть оплата чеком
                        result = (FormResultData)FormManager.OpenForm<FormChoosePay>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                    }
                    else
                    {
                        // платим только деньгами
                        result.stage = WorkerStateStage.PayBillService;
                    }

                    // загрузим выбранную услугу
                    Service serv = Globals.ClientConfiguration.ServiceByIndex(result.numberService);
                    result.serv = serv;

                    Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выбрали услугу: " + serv.caption);

                    if (result.stage == WorkerStateStage.PayBillService)
                    {
                        // ожидание внесение денег
                        result = (FormResultData)FormManager.OpenForm<FormWaitPayBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail || result.stage == WorkerStateStage.EndDropCassette)
                        {
                            // отказ - выход в выбор услуг
                            goto ChooseService;
                        }
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            check = false;
                            continue;
                        }
                    }
                    else if (result.stage == WorkerStateStage.PayCheckService)
                    {
                        // ожидание считывания чека
                        result = (FormResultData)FormManager.OpenForm<FormWaitPayCheck>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail || result.stage == WorkerStateStage.EndDropCassette)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Отказались от оплаты.");

                            // отказ - выход в начало
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выход из оплаты по тайм ауту.");

                            check = false;
                            continue;
                        }
                    }
                    else if (result.stage == WorkerStateStage.Fail)
                    {
                        // в начало
                        continue;
                    }
                    else if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        check = false;
                        continue;
                    }

                    // оказание услуги
                    //Device dev = serv.GetActualDevice();  // устройств пока всего 2 - поэтому решаем в лоб

                    //if (dev != null)
                    {
                        result.timework = serv.timework;

                        // пока так
                        if(result.numberService == 0)
                        {
                            // первая услуга - устройство 3
                            result.numberCurrentDevice = (int)ControlDeviceEnum.dev3;
                        }
                        else
                        {
                            // вторая услуга - устройство 4
                            result.numberCurrentDevice = (int)ControlDeviceEnum.dev4;
                        }

                        result.timeRecognize = serv.timeRecognize;
                        result.ServName = serv.caption;

                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Начали оказывать услугу: " + serv.caption + " Забор аксессуаров.");

                        // сначала включим подсветку и разрешим забрать аксессуары
                        result = (FormResultData)FormManager.OpenForm<FormProgress>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if(result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }

                        // теперь собственно окажем услугу - сначала спросим надо ли 
                        result = (FormResultData)FormManager.OpenForm<FormProvideServiceStart>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.Fail)
                        {
                            // услуга не нужна
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            continue;
                        }

                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Начали оказывать услугу: " + serv.caption);

                        // Будем оказывать услугу
                        result = (FormResultData)FormManager.OpenForm<FormProvideService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail)
                        {
                            // отказались от услуги
                            
                        }

                        // Услугу оказали - выбросим расходники
                        result = (FormResultData)FormManager.OpenForm<FormMessageEndService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выемка денег.");

                            // выемка денег
                            result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndDropCassette)
                            {
                                continue;
                            }
                            else if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }

                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Закончили оказывать услугу: " + serv.caption);
                        Program.Log.Write(LogMessageType.Information, "MAIN WORK: Проработали: " + result.timework);

                        // пишем в базу строку с временем работы
                        GlobalDb.GlobalBase.WriteWorkTime(serv.id, result.numberCurrentDevice, result.timework);
                    }
                    //else
                    //{
                    //    MessageBox.Show("Услуга " + serv.caption + " не может быть предоставлена");
                    //}
                }
                catch (Exception exp)
                {
                    // вернемся к исходной позиции - какая то ошибка
                    Program.Log.Write(LogMessageType.Error, "MAIN WORK: Ошибка.");
                }
            }
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }
        }

        /// <summary>
        /// Проверка статистических данных
        /// </summary>
        private FormResultData CheckStatistic(FormResultData result)
        {
            result.stage = WorkerStateStage.None;

            if (result.statistic.CountBankNote >= Globals.ClientConfiguration.Settings.MaxCountBankNote)
            {
                // сообщим о необходимоcти изъятия денег
                result.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageNeedCollect, result.log);

                // Пора слать смс с необходимостью обслуживания
                result.stage = WorkerStateStage.BillFull;

                Program.Log.Write(LogMessageType.Error, "CHECK_STAT: необходимо изъять купюры. Предел " + Globals.ClientConfiguration.Settings.MaxCountBankNote + " сейчас " + result.statistic.CountBankNote);
            }

            // сообщение о ресурсе устройств
            DateTime dt = GlobalDb.GlobalBase.GetLastRefreshTime();

            int count = 0;
            if (dt != null)
            {
                count = GlobalDb.GlobalBase.GetWorkTime(dt);
            }
            else
            {
                count = GlobalDb.GlobalBase.GetWorkTime(new DateTime(2000, 1, 1));
            }

            if (count >= Globals.ClientConfiguration.Settings.limitServiceTime)
            {
                // ресурс выработали - сообщим об этом
                result.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd, result.log);

                // аппарат не работает
                result.stage = WorkerStateStage.ResursEnd;

                Program.Log.Write(LogMessageType.Error, "CHECK_STAT: выработали ресурс. Было установлено:" + Globals.ClientConfiguration.Settings.limitServiceTime + " проработали " + count);
            }

            // читаем состояние устройства
            byte[] res;
            res = result.drivers.control.GetStatusControl(Program.Log);

            if (res != null)
            {
                if (res[0] == 0)
                {
                    result.drivers.modem.SendSMS("Отказ управляющего устройства", result.log);

                    result.stage = WorkerStateStage.ErrorControl;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: ошибка управляющего устройства.");
                }
            }

            // проверяем наличие бумаги в принтере
            //if(result.drivers.printer.status.CheckPaper(Program.Log) == PaperEnableEnum.PaperEnd)
            //{
            //    // бумага кончилась

            //    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
            //    {
            //        // с такой ошибкой не работаем
            //        result.stage = WorkerStateStage.PaperEnd;
            //    }

            //    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: кончилась бумага.");
            //}

            PrinterStatus status = result.drivers.printer.GetStatus();

            if ((status & (PrinterStatus.PRINTER_STATUS_PAPER_OUT 
                         | PrinterStatus.PRINTER_STATUS_PAPER_JAM 
                         | PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM 
                         | PrinterStatus.PRINTER_STATUS_ERROR)) > 0)
            {
                result.drivers.modem.SendSMS("Кончилась бумага", result.log);

                // что то с бумагой
                if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                {
                    // с такой ошибкой не работаем
                    result.stage = WorkerStateStage.PaperEnd;
                }

                Program.Log.Write(LogMessageType.Error, "CHECK_STAT: кончилась бумага.");
            }

            return result;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Запустим основной разработчик
            MainWorker();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //drivers.StopAllDevice();
            }
            catch
            {

            }
        }
    }
}
