using ServiceSaleMachine.Drivers;
using System;
using System.Threading;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : MyForm
    {
        // стадии работы
        public WorkerStateStage Stage { get; set; }

        // драйвера
        MachineDrivers drivers;

        FormWait wait;

        // потоки
        private SaleThread WorkerWait { get; set; }

        // задача очистки логов
        ClearFilesControlServiceTask ClearFilesTask { get; set; }

        // текущий пользователь
        int CurrentUserId;

        // выемка денег
        bool MoneyRecess;

        // статистика по деньгам
        MoneyStatistic statistic;
        // статистика использования расходников
        DeviceStatistic dev_statistic;

        // запуск приложения
        public MainForm(WorkerStateStage StateStage)
        {
            InitializeComponent();

            // запустим задачу очистки от логов директории
            ClearFilesTask = new ClearFilesControlServiceTask(Program.Log);

            drivers = new MachineDrivers(Program.Log);
            drivers.ReceivedResponse += reciveResponse;

            // задача отображения долгих операций
            WorkerWait = new SaleThread { ThreadName = "WorkerWait" };
            WorkerWait.Work += WorkerWait_Work;
            WorkerWait.Complete += WorkerWait_Complete;


            Stage = StateStage;// WorkerStateStage.None;

            // база данных
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

            // прочтем из базы статистику
            statistic = GlobalDb.GlobalBase.GetMoneyStatistic();
            dev_statistic = GlobalDb.GlobalBase.GetDevStatistic();

            //statistic.CountBankNote = GlobalDb.GlobalBase.GetCountBankNote();
        }

        /// <summary>
        /// Основной обработчик
        /// </summary>
        private void MainWorker()
        {
            FormResultData result = new FormResultData();
            result.drivers = drivers;
            result.statistic = statistic;
            result.dev_statistic = dev_statistic;

            if (Globals.admin)
            {
                drivers.InitAllDevice();
                result = (FormResultData)FormManager.OpenForm<FormSettings>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
              //  drivers.StopAllDevice();
            }

            initDevice:
            //WorkerWait.Run();

            if(Globals.ClientConfiguration.Settings.offHardware == 0)   // если не отключено
            {
                // инициализация оборудования
                switch (drivers.InitAllDevice())
                {
                    case WorkerStateStage.None:
                        break;
                    case WorkerStateStage.NoCOMPort:
                        if (WorkerWait.IsWork) WorkerWait.Abort();
                        result = (FormResultData)FormManager.OpenForm<FormWelcomeUser>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        break;
                    case WorkerStateStage.NeedSettingProgram:
                        {
                            if (WorkerWait.IsWork) WorkerWait.Abort();

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

            if(WorkerWait.IsWork) WorkerWait.Abort();

            while (true)
            {
                try
                {
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
                    else if (result.stage == WorkerStateStage.Rules)
                    {
                        // ознакомление с правилами
                        result = (FormResultData)FormManager.OpenForm<FormRules>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                        // ознакомились - возвращаемся обратно
                        continue;
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
                        else if (result.stage == WorkerStateStage.ErrorControl)
                        {
                            // аппарат временно не работает
                            result = (FormResultData)FormManager.OpenForm<FormTemporallyNoWork>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);
                            if (result.stage == WorkerStateStage.ErrorEndControl)
                            {
                                continue;
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
                    result = (FormResultData)FormManager.OpenForm<FormChooseService>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

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
                                drivers.InitAllDevice();
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
                        result = (FormResultData)FormManager.OpenForm<FormWaitPayBill>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify, result);

                        // проверим результат
                        drivers.ReceivedResponse += reciveResponse;

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

                        // проверим результат
                        drivers.ReceivedResponse += reciveResponse;

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
                        result.timework = dev.timework;
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
                        GlobalDb.GlobalBase.WriteWorkTime(serv.id, dev.id, dev.timework);
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

            return result;
        }


        /// <summary>
        /// Обработчик событий от устройств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.Scaner:

                    break;
                case DeviceEvent.BillAcceptor:

                    break;
                case DeviceEvent.NoCOMPort:
                    break;
                case DeviceEvent.InitializationOK:
                    break;
                case DeviceEvent.NeedSettingProgram:
                    // необходимо запустить настройку приложения
                    break;
                case DeviceEvent.DropCassetteBillAcceptor:
                    // выемка денег
                    {
                        if (MoneyRecess == false)
                        {
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

                            GlobalDb.GlobalBase.Encashment(CurrentUserId, countmoney);

                            MoneyRecess = true;
                        }
                    } 
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:
                    // я полный
                    break;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MainWorker();
        }

        private void WorkerWait_Complete(object sender, ThreadCompleteEventArgs e)
        {
            wait.Hide();
        }

        private void WorkerWait_Work(object sender, ThreadWorkEventArgs e)
        {
            wait = new FormWait();
            try
            {
                wait.Show();
            }
            catch
            {
                return;
            }

            while (!e.Cancel)
            {
                try
                {
                    wait.Refresh();
                }
                catch
                {
                }
                finally
                {
                    if (!e.Cancel)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WorkerWait.Abort();

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
