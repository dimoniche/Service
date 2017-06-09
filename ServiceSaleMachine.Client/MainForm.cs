using System;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;
using System.Drawing;
using System.IO;

namespace AirVitamin.Client
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

        FormWaitVideoSecondScreen fr2;

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

            result.FontCollection = new System.Drawing.Text.PrivateFontCollection();

            result.FontCollection.AddFontFile(Globals.GetPath(PathEnum.Fonts) + "\\" + "CeraRoundPro_Bold.ttf");
            result.FontCollection.AddFontFile(Globals.GetPath(PathEnum.Fonts) + "\\" + "CeraRoundPro_Medium.ttf");

            // инициализируем задачи
            result.drivers.InitAllTask();

            //result = (FormResultData)FormManager.OpenForm<FormProvideService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
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

            ShowVideo();

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
                    result.retLogin = "";
                    result.retPassword = "";
                    result.realtimework = 0;

                    Globals.UserConfiguration.UserLogin = "";
                    Globals.UserConfiguration.UserPassword = "";

                    // Проверка статистических данных - может пора заканчивать работать
                    if (check)
                    {
                        result = CheckStatistic(result);
                    }

                    check = true;

NoCheckStatistic:
                    if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        if (result.BillError == false)
                        {
                            result.drivers.modem.SendSMS("Bill acceptor неисправен.", result.log);
                            Program.Log.Write(LogMessageType.Error, "MAIN WORK: Bill acceptor неисправен.");
                        }

                        if (Globals.ClientConfiguration.Settings.changeOn > 0)
                        {
                            // со сдачей - не будем показывать экран смерти - продолжим работать
                            result.stage = WorkerStateStage.None;
                        }

                        result.BillError = true;
                    }
                    else if (result.stage == WorkerStateStage.BillFull)
                    {
                        if (result.BillError == false)
                        {
                            result.drivers.modem.SendSMS("Bill acceptor полон.", result.log);
                            Program.Log.Write(LogMessageType.Error, "MAIN WORK: Bill acceptor полон.");
                        }

                        if (Globals.ClientConfiguration.Settings.changeOn > 0)
                        {
                            // со сдачей - не будем показывать экран смерти
                            result.stage = WorkerStateStage.None;
                        }

                        result.BillError = true;
                    }
                    else if (result.stage == WorkerStateStage.Gas1_low 
                            || result.stage == WorkerStateStage.Gas2_low 
                            || result.stage == WorkerStateStage.Gas3_low 
                            || result.stage == WorkerStateStage.Gas4_low)
                    {
                        // давление упало - но мы продолжаем работать - просто шлем СМС
                        if (result.stage == WorkerStateStage.Gas1_low)
                        {
                            if (!result.IsSendSMS1)
                            {
                                result.drivers.modem.SendSMS("Упало давление кислорода", result.log);
                                result.IsSendSMS1 = true;
                            }

                            Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД1 - HIGH.");

                            if (Globals.ClientConfiguration.Settings.offReserve != 1) result.stage = WorkerStateStage.None;
                        }
                        if (result.stage == WorkerStateStage.Gas2_low)
                        {
                            if (!result.IsSendSMS2)
                            {
                                result.drivers.modem.SendSMS("Низкое давление Газа 2", result.log);
                                result.IsSendSMS2 = true;
                            }

                            Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД2 - HIGH.");

                            result.stage = WorkerStateStage.None;
                        }
                        if (result.stage == WorkerStateStage.Gas3_low)
                        {
                            if (!result.IsSendSMS3)
                            {
                                result.drivers.modem.SendSMS("Низкое давление Газа 3", result.log);
                                result.IsSendSMS3= true;
                            }

                            Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД3 - HIGH.");

                            result.stage = WorkerStateStage.None;
                        }
                        if (result.stage == WorkerStateStage.Gas4_low)
                        {
                            if (!result.IsSendSMS4)
                            {
                                result.drivers.modem.SendSMS("Низкое давление Газа 4", result.log);
                                result.IsSendSMS4 = true;
                            }

                            Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД4 - HIGH.");
                        }
                    }
                    else
                    {
                        // это не ошибки приемника - с приемником все ок
                        result.BillError = false;
                        // с газом тоже полный порядок
                        result.IsSendSMS1 = result.IsSendSMS2 = result.IsSendSMS3 = result.IsSendSMS4 = false;
                    }

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
                        else if (result.stage == WorkerStateStage.DropCassettteBill)
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
                        else if (result.stage == WorkerStateStage.BillErrorEnd)
                        {

                        }

                        continue;
                    }

                    // -----------------------------------------------------
                    // ожидание клиента
                    // -----------------------------------------------------

                    // ознакомление с правилами
                    result = (FormResultData)FormManager.OpenForm<FormRules>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.MainScreen)
                    {
                        // ознакомились - возвращаемся обратно - идем на выбор что делать
                    }
                    else if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        // ошибки купюроприемника
                        goto NoCheckStatistic;
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
                    else if (result.stage == WorkerStateStage.ErrorControl)
                    {
                        #region Ошибка управляющего устройства
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
                        #endregion
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        #region отображение видео ожидания
                        // по тайм ауту вышли в рекламу
                        if (Globals.ClientConfiguration.Settings.ScreenServerType == 0 || Globals.ClientConfiguration.Settings.ScreenServerType == 1)
                        {
                            result = (FormResultData)FormManager.OpenForm<FormWaitClientGif>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        }
                        else
                        {
                            result = (FormResultData)FormManager.OpenForm<FormWaitClientVideo>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        }

                        if (result.stage == WorkerStateStage.ErrorBill)
                        {
                            // ошибки купюроприемника
                            goto NoCheckStatistic;
                        }
                        else if (result.stage == WorkerStateStage.BillFull)
                        {
                            // купюроприемник полон
                            goto NoCheckStatistic;
                        }
                        else if (result.stage == WorkerStateStage.Gas1_low || result.stage == WorkerStateStage.Gas4_low)
                        {
                            // Давление газа упало - в окно ошибки
                            goto NoCheckStatistic;
                        }
                        else if (result.stage == WorkerStateStage.PaperEnd)
                        {
                            // ошибки принтера есть
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ErrorPrinter)
                        {
                            // ошибки принтера есть
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
                        else if (result.stage == WorkerStateStage.ErrorControl)
                        {
                            #region Ошибка управляющего устройства
                            Program.Log.Write(LogMessageType.Information, "MAIN WORK: Аппарат временно не работает.");

                            // аппарат временно не работает
                            result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

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

                                if (result.stage == WorkerStateStage.ExitProgram)
                                {
                                    // выход
                                    Close();
                                    return;
                                }
                            }
                            else if (result.stage == WorkerStateStage.BillErrorEnd)
                            {

                            }

                            continue;
                            #endregion
                        }
                        else
                        {
                            // вышли из рекламы
                            check = false;
                            continue;
                        }
                        #endregion
                    }
                    else if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        // ошибки купюроприемника
                        goto NoCheckStatistic;
                    }
                    else if (result.stage == WorkerStateStage.BillFull)
                    {
                        // купюроприемник полон
                        goto NoCheckStatistic;
                    }
                    else if (result.stage == WorkerStateStage.Gas1_low || result.stage == WorkerStateStage.Gas4_low)
                    {
                        // Давление газа упало - в окно ошибки
                        goto NoCheckStatistic;
                    }

                    MainForm:
                    // выбор что делать
                    result = (FormResultData)FormManager.OpenForm<FormMainMenu>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.PaperEnd)
                    {
                        // ошибки принтера есть
                        continue;
                    }
                    else if (result.stage == WorkerStateStage.ErrorPrinter)
                    {
                        // ошибки принтера есть
                        continue;
                    }
                    else if (result.stage == WorkerStateStage.Gas1_low || result.stage == WorkerStateStage.Gas4_low)
                    {
                        // Давление газа упало - в окно ошибки
                        goto NoCheckStatistic;
                    }
                    else if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        // ошибки купюроприемника
                        goto NoCheckStatistic;
                    }
                    else if (result.stage == WorkerStateStage.BillFull)
                    {
                        // купюроприемник полон
                        goto NoCheckStatistic;
                    }
                    else if (result.stage == WorkerStateStage.InterUser)
                    {
                        #region  авторизация пользователя

                        result = (FormResultData)FormManager.OpenForm<UserRequest>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        // проверим результат
                        if ((result.stage == WorkerStateStage.AuthorizeUser || result.stage == WorkerStateStage.RegisterNewUser) && result.retLogin != "")
                        {
                            // авторизовались или внесли нового пользователя
                            UserInfo ui = GlobalDb.GlobalBase.GetUserByName(result.retLogin, result.retPassword);

                            if(ui != null && result.stage == WorkerStateStage.RegisterNewUser)
                            {
                                // Новый пользователь - сообщим об этом
                                result = (FormResultData)FormManager.OpenForm<FormRegisterNewUser>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result, "Зарегистрировали нового пользователя: +7" + result.retLogin);

                                if (result.stage == WorkerStateStage.ExitProgram)
                                {
                                    // выход
                                    Close();
                                    return;
                                }
                            }
                            else if (ui != null)
                            {
                                // здесь покажем какого пользователя авторизовали
                            }
                        }
                        else if (result.stage == WorkerStateStage.FindPhone)
                        {
                            result = (FormResultData)FormManager.OpenForm<FormRegisterNewUser>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result, "На Ваш телефон " + result.retLogin + " отправлен пароль");

                            if (result.stage == WorkerStateStage.ExitProgram)
                            {
                                // выход
                                Close();
                                return;
                            }
                        }
                        else if (result.stage == WorkerStateStage.NotFindPhone)
                        {
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ErrorRegisterNewUser)
                        {
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.NotAuthorizeUser)
                        {
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.MainScreen)
                        {
                            // сброс авторизации
                            goto MainForm;
                        }
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            // тайм аут

                            continue;
                        }

                        // нормально зарегистрировались
                        goto MainForm;

                        #endregion
                    }
                    else if (result.stage == WorkerStateStage.ErrorControl)
                    {
                        #region Ошибка управляющего устройства
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
                        #endregion
                    }
                    else if (result.stage == WorkerStateStage.ChooseService)
                    {
                        // уходим на выбор услуг
                    }
                    else if (result.stage == WorkerStateStage.Philosof)
                    {
                        #region ознакомление с философией
                        result = (FormResultData)FormManager.OpenForm<FormPhilosofy>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.MainScreen)
                        {
                            // ознакомились - возвращаемся обратно
                            goto MainForm;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.ErrorBill)
                        {
                            // ошибки купюроприемника
                            goto NoCheckStatistic;
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
                        #endregion
                    }
                    else if (result.stage == WorkerStateStage.Rules)
                    {
                        #region ознакомление с правилами
                        result = (FormResultData)FormManager.OpenForm<FormRules>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.MainScreen)
                        {
                            // ознакомились - возвращаемся обратно
                            goto MainForm;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.ErrorBill)
                        {
                            // ошибки купюроприемника
                            goto NoCheckStatistic;
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
                        #endregion
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
                        // перейдем на инструкцию
                        continue;
                    }

                    ChooseService:

                    // -----------------------------------------------------
                    // выбор услуг
                    // -----------------------------------------------------
                    result = (FormResultData)FormManager.OpenForm<FormChooseService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.ExitProgram)
                    {
                        // выход
                        Close();
                        return;
                    }
                    else if (result.stage == WorkerStateStage.MainScreen)
                    {
                        goto MainForm;
                    }
                    else if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        // ошибки купюроприемника
                        goto NoCheckStatistic;
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
                    else if (result.stage == WorkerStateStage.WhatsDiff)
                    {
                        result = (FormResultData)FormManager.OpenForm<FormWhatsDiff>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.ErrorBill)
                        {
                            // ошибки купюроприемника
                            goto NoCheckStatistic;
                        }

                        // вернемся в выбор услуги (уж не думал что goto буду использовать)
                        goto ChooseService;
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        check = false;
                        continue;
                    }

                    // загрузим выбранную услугу
                    Service serv = Globals.ClientConfiguration.ServiceByIndex(result.numberService);
                    result.serv = serv;

                    Program.Log.Write(LogMessageType.Information, "MAIN WORK: Выбрали услугу: " + serv.caption);

                    // -----------------------------------------------------
                    // ожидание внесение денег
                    // -----------------------------------------------------
                    result = (FormResultData)FormManager.OpenForm<FormWaitPayBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.Fail || result.stage == WorkerStateStage.EndDropCassette)
                    {
                        // отказ - выход в выбор услуг
                        goto ChooseService;
                    }
                    else if (result.stage == WorkerStateStage.ErrorBill)
                    {
                        // ошибки купюроприемника
                        goto NoCheckStatistic;
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

                    // -----------------------------------------------------
                    // оказание услуги
                    // -----------------------------------------------------
                    result.timework = serv.timework;

                    // пока так
                    if(result.numberService == 0)
                    {
                        // первая услуга - До
                        result.numberCurrentDevice = (int)ControlDeviceEnum.MixBefore;
                    }
                    else
                    {
                        // вторая услуга - После
                        result.numberCurrentDevice = (int)ControlDeviceEnum.MixAfter;
                    }

                    result.timeRecognize = serv.timeBefore;
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

                    Program.Log.Write(LogMessageType.Information, "MAIN WORK: Начали оказывать услугу: " + serv.caption);

                    // Будем оказывать услугу
                    result = (FormResultData)FormManager.OpenForm<FormProvideService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                    if (result.stage == WorkerStateStage.Fail)
                    {
                        // отказались от услуги
                    }
                    else if (result.stage == WorkerStateStage.Gas1_low || result.stage == WorkerStateStage.Gas2_low)
                    {
                        goto NoCheckStatistic;
                    }

                    // -----------------------------------------------------
                    // Услугу оказали - выбросим расходники
                    // -----------------------------------------------------
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
                    Program.Log.Write(LogMessageType.Information, "MAIN WORK: Проработали: " + result.realtimework);
                    Program.Log.Write(LogMessageType.Information, "========================КОНЕЦ ОБСЛУЖИВАНИЯ===========================");

                    // пишем в базу строку с временем работы
                    GlobalDb.GlobalBase.WriteWorkTime(serv.id, result.numberCurrentDevice, result.realtimework);
                }
                catch (Exception exp)
                {
                    // вернемся к исходной позиции - какая то ошибка
                    Program.Log.Write(LogMessageType.Error, "MAIN WORK: Ошибка.");
                    Program.Log.Write(LogMessageType.Error, "MAIN WORK: " + exp.GetDebugInformation());

                    // все переинициализируем
                    result = new FormResultData(Program.Log);

                    // инициализируем задачи
                    result.drivers.InitAllTask();
                    // инициализируем устройства
                    result.drivers.InitAllDevice();
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
                if (Globals.ClientConfiguration.Settings.BlockDevice == 1) result.stage = WorkerStateStage.ResursEnd;

                Program.Log.Write(LogMessageType.Error, "CHECK_STAT: выработали ресурс. Было установлено:" + Globals.ClientConfiguration.Settings.limitServiceTime + " проработали " + count);
            }

            // читаем состояние устройства
            byte[] res;
            /*res = result.drivers.control.GetStatusControl(Program.Log);

            if (res != null)
            {
                if (res[0] == 0)
                {
                    result.drivers.modem.SendSMS("Отказ управляющего устройства", result.log);

                    result.stage = WorkerStateStage.ErrorControl;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: ошибка управляющего устройства.");
                }
            }*/

            res = result.drivers.control.GetStatusRelay(Program.Log);

            if (res != null)
            {
                if (res[0] >= 1)
                {
                    result.stage = WorkerStateStage.Gas1_low;
                }
                if (res[1] >= 1)
                {
                    result.stage = WorkerStateStage.Gas2_low;
                }
                if (res[2] >= 1)
                {
                    result.stage = WorkerStateStage.Gas3_low;
                }
                if (res[3] >= 1)
                {
                    result.stage = WorkerStateStage.Gas4_low;
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

            if (Globals.ClientConfiguration.Settings.offPrinter == 0)
            {
                PrinterStatus status = result.drivers.printer.GetStatus();

                if ((status & (PrinterStatus.PRINTER_STATUS_PAPER_OUT
                             | PrinterStatus.PRINTER_STATUS_PAPER_JAM
                             | PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM
                             | PrinterStatus.PRINTER_STATUS_DOOR_OPEN
                             | PrinterStatus.PRINTER_STATUS_ERROR)) > 0)
                {
                    // что то с бумагой
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        // с такой ошибкой не работаем
                        result.stage = WorkerStateStage.PaperEnd;
                    }

                    if (result.PrinterError == false)
                    {
                        result.drivers.modem.SendSMS("Кончилась бумага", result.log);
                        Program.Log.Write(LogMessageType.Error, "CHECK_STAT: кончилась бумага.");
                    }

                    result.PrinterError = true;
                }
                else if ((status & PrinterStatus.PRINTER_STATUS_OFFLINE) > 0)
                {
                    if (Globals.ClientConfiguration.Settings.NoPaperWork == 0)
                    {
                        // нет связи с принтером
                        result.stage = WorkerStateStage.ErrorPrinter;
                    }

                    if (result.PrinterError == false)
                    {
                        result.drivers.modem.SendSMS("Нет связи с принтером.", result.log);
                        Program.Log.Write(LogMessageType.Error, "CHECK_STAT: нет связи с принтером.");
                    }

                    result.PrinterError = true;
                }
                else
                {
                    if (result.PrinterError == true)
                    {
                        Program.Log.Write(LogMessageType.Error, "CHECK_STAT: ошибка принтера снялась.");
                    }

                    result.PrinterError = false;
                }
            }
            else
            {
                result.PrinterError = false;
            }

            return result;
        }

        /// <summary>
        /// Запуск видео на втором экране
        /// </summary>
        void ShowVideo()
        {
            if (Globals.ClientConfiguration.Settings.offVideoSecondScreen == 1) return;

            // Разворачивание 2-й формы на втором мониторе
            Screen[] sc;
            sc = Screen.AllScreens;

            // только один экран
            if (sc.Length == 1) return;

            Point p = new Point();

            foreach (Screen screen in sc)
            {
                if(screen.Bounds.Location.X == 0 && screen.Bounds.Location.Y == 0)
                {
                    // главный экран
                }
                else
                {
                    // второй экран
                    p = new Point(screen.Bounds.Location.X, screen.Bounds.Location.Y);
                    break;
                }
            }

            if (p.X == 0 && p.Y == 0)
            {
                // на главном экране ничего не рисуем
                return;
            }

            fr2 = new FormWaitVideoSecondScreen(p,result);

            fr2.FormBorderStyle = FormBorderStyle.None;
            fr2.StartPosition = FormStartPosition.Manual;

            fr2.Location = sc[0].Bounds.Location;
            fr2.WindowState = FormWindowState.Maximized;

            fr2.Show();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Запустим основной разработчик
            MainWorker();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Log.Write(LogMessageType.Information, "Выход из приложения.");

            GlobalDb.GlobalBase.CloseForm();

            result.drivers.printer.AbortPrint();

            if(fr2 != null) fr2.Close();

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
