using System;
using System.Windows.Forms;

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
        }

        /// <summary>
        /// Основной обработчик
        /// </summary>
        private void MainWorker()
        {
            //if (Globals.admin)
            {
                result.drivers.InitAllDevice();
                result = (FormResultData)FormManager.OpenForm<FormSettings>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
              //  drivers.StopAllDevice();
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
                        result = (FormResultData)FormManager.OpenForm<FormWelcomeUser>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        break;
                    case WorkerStateStage.NeedSettingProgram:
                        {
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

            while (true)
            {
                try
                {
                    // забудем пользователя
                    result.CurrentUserId = 0;
                    result.stage = WorkerStateStage.None;

                    // Проверка статистических данных - может пора заканчивать работать
                    result = CheckStatistic(result);

                    if (result.stage == WorkerStateStage.NeedService)
                    {
                        // Необходимо обслуживание аппарата
                        result = (FormResultData)FormManager.OpenForm<FormNeedService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.EndNeedService)
                        {
                            continue;
                        }
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
                        // аппарат временно не работает
                        result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
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
                            continue;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.ChooseService)
                        {
                            // уходим на выбор услуг
                        }
                    }
                    else if (result.stage == WorkerStateStage.DropCassettteBill)
                    {
                        // выемка денег
                        result = (FormResultData)FormManager.OpenForm<FormMoneyRecess>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.EndDropCassette)
                        {
                            continue;
                        }
                    }
                    else if (result.stage == WorkerStateStage.NeedService)
                    {
                        // необходимо обслуживание
                        result = (FormResultData)FormManager.OpenForm<FormNeedService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.EndNeedService)
                        {
                            continue;
                        }
                    }
                    else if (result.stage == WorkerStateStage.TimeOut)
                    {
                        // по тайм ауту вышли в рекламу
                        result = (FormResultData)FormManager.OpenForm<FormWaitStage>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.DropCassettteBill)
                        {
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
                        else if (result.stage == WorkerStateStage.NeedService)
                        {
                            // необходимо обслуживание
                            result = (FormResultData)FormManager.OpenForm<FormNeedService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                            if (result.stage == WorkerStateStage.EndNeedService)
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
                        }
                        else
                        {
                            // вышли из рекламы
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

                    if (result.stage == WorkerStateStage.PayBillService)
                    {
                        // ожидание внесение денег
                        result = (FormResultData)FormManager.OpenForm<FormWaitPayBill1>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail || result.stage == WorkerStateStage.EndDropCassette)
                        {
                            // отказ - выход в выбор услуг
                            goto ChooseService;
                        }
                        else if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
                        }
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            continue;
                        }
                    }
                    else if (result.stage == WorkerStateStage.PayCheckService)
                    {
                        // ожидание считывания чека
                        result = (FormResultData)FormManager.OpenForm<FormWaitPayCheck>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail || result.stage == WorkerStateStage.EndDropCassette)
                        {
                            // отказ - выход в начало
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
                        continue;
                    }

                    // оказание услуги
                    Device dev = serv.GetActualDevice();

                    if (dev != null)
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

                        // сначала включим подсветку и разрешим забрать аксессуары
                        result = (FormResultData)FormManager.OpenForm<FormProgress>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.ExitProgram)
                        {
                            // выход
                            Close();
                            return;
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
                        else if (result.stage == WorkerStateStage.TimeOut)
                        {
                            continue;
                        }

                        // Будем оказывать услугу
                        result = (FormResultData)FormManager.OpenForm<FormProvideService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        if (result.stage == WorkerStateStage.Fail)
                        {
                            // отказались от услуги
                            
                        }

                        // Услугу оказали - выбросим расходники
                        result = (FormResultData)FormManager.OpenForm<FormMessageEndService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        // пишем в базу строку с временем работы
                        GlobalDb.GlobalBase.WriteWorkTime(serv.id, dev.id, result.timework);
                    }
                    else
                    {
                        MessageBox.Show("Услуга " + serv.caption + " не может быть предоставлена");
                    }
                }
                catch (Exception exp)
                {
                    // вернемся к исходной позиции - какая то ошибка
                }
            }
        }

        /// <summary>
        /// Проверка статистических данных
        /// </summary>
        private FormResultData CheckStatistic(FormResultData result)
        {
            if (result.statistic.CountBankNote >= Globals.ClientConfiguration.Settings.MaxCountBankNote)
            {
                // сообщим о необходимоcти изъятия денег
                result.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageNeedCollect);

                // Пора слать смс с необходимостью обслуживания
                result.stage = WorkerStateStage.NeedService;
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
                result.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd);

                // аппарат не работает
                result.stage = WorkerStateStage.NeedService;
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
