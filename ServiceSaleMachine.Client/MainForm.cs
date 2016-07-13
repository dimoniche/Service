using ServiceSaleMachine.Drivers;
using System;
using System.Drawing;
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

        // услуги
        public int numberService;

        // формы
        FormSettings setting;
        FormWaitStage WaitStageForm;
        FormRuleService RuleStageForm;
        FormChooseService ChooseServiceForm;
        FormChoosePay ChoosePayForm;
        FormWaitPayCheck WaitPayCheck;
        FormWaitPayBill WaitPayBill;
        FormProgress fprgs;
        UserRequest userRequest;

        FormWait wait;

        // потоки
        private SaleThread WorkerWait { get; set; }
        private SaleThread MainWorkerTask { get; set; }

        // задача очистки логов
        ClearFilesControlServiceTask ClearFilesTask { get; set; }

        delegate void StartNextForm();

        // количество банкнот
        int CountBankNote;

        // текущий пользователь
        int CurrentUserId;

        // запуск приложения
        public MainForm()
        {
            InitializeComponent();

            // запустим задачу очистки от логов директории
            ClearFilesTask = new ClearFilesControlServiceTask(Program.Log);

            drivers = new MachineDrivers(Program.Log);
            drivers.ReceivedResponse += reciveResponse;

            // основная управляющая задача
            MainWorkerTask = new SaleThread { ThreadName = "MainWorkerTask" };
            MainWorkerTask.Work += MainWorkerTask_Work;
            MainWorkerTask.Complete += MainWorkerTask_Complete;
            MainWorkerTask.ProgressChanged += MainWorkerTask_ProgressChanged;

            // задача отображения долгих операций
            WorkerWait = new SaleThread { ThreadName = "WorkerWait" };
            WorkerWait.Work += WorkerWait_Work;
            WorkerWait.Complete += WorkerWait_Complete;

            Stage = WorkerStateStage.None;

            // база данных
            if (GlobalDb.GlobalBase.Connect())
            {

            }
            else
            {
                
            }

            GlobalDb.GlobalBase.CreateTables();

            CountBankNote = GlobalDb.GlobalBase.GetCountBankNote();

            //FormManager.OpenForm<FormWaitStage>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify);
        }

        private void MainWorkerTask_ProgressChanged(object sender, ThreadProgressChangedEventArgs e)
        {

        }

        private void MainWorkerTask_Complete(object sender, ThreadCompleteEventArgs e)
        {
        }

        // основной рабочий обработчик
        private void MainWorkerTask_Work(object sender, ThreadWorkEventArgs e)
        {
            //while (!e.Cancel)
            //{
            //    try
            //    {
                    
            //    }
            //    catch
            //    {
            //    }
            //    finally
            //    {
            //        if (!e.Cancel)
            //        {
            //            Thread.Sleep(100);
            //        }
            //    }
            //}

            try
            {
                switch (Stage)
                {
                    case WorkerStateStage.Init:
                        WorkerWait.Run();

                        hideMainForm();

                        // инициализируем устройства
                        //if (drivers.InitAllDevice())
                        {
                            Stage = WorkerStateStage.Wait;          // переходим в ожидания
                        }
                        /*else
                        {
                            Stage = WorkerStateStage.Setting;       // переходим в режим настройки
                            BeginInvoke(new StartNextForm(StartNextForm_Func));
                        }*/

                        WorkerWait.Abort();
                        break;
                    case WorkerStateStage.Wait:
                        break;
                    case WorkerStateStage.AgainWait:
                        Stage = WorkerStateStage.Wait;                      // переходим в ожидание
                        break;
                    case WorkerStateStage.Rules:
                        break;
                    case WorkerStateStage.Setting:
                        break;
                     case WorkerStateStage.ChooseService:
                        break;
                    case WorkerStateStage.ChoosePay:
                        break;
                    case WorkerStateStage.PayBillService:
                        break;
                    case WorkerStateStage.PayCheckService:
                        break;
                    case WorkerStateStage.StartService:
                        break;
                    case WorkerStateStage.UserRequestService:
                        break;
                }

                // переход на нужную форму
                StartNextForm_Func();
            }
            catch (Exception err)
            {
            }
        }

        private void StartNextForm_Func()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StartNextForm(StartNextForm_Func));
                return;
            }

            switch (Stage)
            {
                case WorkerStateStage.Setting:
                    this.Hide();
                    setting = new FormSettings(drivers, this);
                    setting.Show();
                    break;
                case WorkerStateStage.Wait:
                    this.Hide();
                    WaitStageForm = new FormWaitStage(drivers, this);
                    WaitStageForm.Show();
                    break;
                case WorkerStateStage.Rules:
                    this.Hide();
                    RuleStageForm = new FormRuleService(drivers, this);
                    RuleStageForm.Show();
                    break;
                case WorkerStateStage.ChooseService:
                    this.Hide();
                    ChooseServiceForm = new FormChooseService(drivers, this);
                    ChooseServiceForm.Show();
                    break;
                case WorkerStateStage.ChoosePay:
                    this.Hide();
                    ChoosePayForm = new FormChoosePay(drivers, this);
                    ChoosePayForm.Show();
                    break;
                case WorkerStateStage.PayCheckService:
                    this.Hide();
                    WaitPayCheck = new FormWaitPayCheck(drivers, this);
                    WaitPayCheck.Show();
                    break;
                case WorkerStateStage.PayBillService:
                    this.Hide();
                    WaitPayBill = new FormWaitPayBill(drivers, this);
                    WaitPayBill.Show();
                    break;
                case WorkerStateStage.StartService:
                    this.Hide();
                    fprgs = new FormProgress(drivers, this);

                    Service serv = Globals.ClientConfiguration.ServiceByIndex(numberService);

                    Device dev = serv.GetActualDevice();

                    if(dev != null)
                    {
                        fprgs.timework = dev.timework;
                        fprgs.ServName = serv.caption;
                        fprgs.Start();

                        // пишем в базу строку с временем работы
                        GlobalDb.GlobalBase.WriteWorkTime(serv.id, dev.id, dev.timework);
                    }
                    else
                    {
                        MessageBox.Show("Услуга " + serv.caption + " не может быть предоставлена");
                    }

                    break;
                case WorkerStateStage.UserRequestService:
                    this.Hide();
                    userRequest = new UserRequest(drivers, this);
                    userRequest.ShowDialog();
                    if (userRequest.retLogin != "")
                    {
                        UserInfo ui = GlobalDb.GlobalBase.GetUserByName(userRequest.retLogin, userRequest.retPassword);
                        if (ui != null)
                        {
                            MessageBox.Show(ui.Role.ToString());
                        }
                    }
                    break;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            switch (Stage)
            {
                case WorkerStateStage.None:
                    return;
                case WorkerStateStage.Setting:
                    Stage = WorkerStateStage.Init;
                    break;
                case WorkerStateStage.Wait:
                    // вышли из режима ожидания - ознокомление с правилами
                    Stage = WorkerStateStage.Rules;
                    break;
                case WorkerStateStage.Rules:
                    // вышли из режима ознокомления с правилами - теперь выберем услугу
                    Stage = WorkerStateStage.ChooseService;
                    break;
                case WorkerStateStage.Fail:
                    // вышли из режима ознокомления с правилами c отказом - опять ждем клиента
                    Stage = WorkerStateStage.Wait;
                    break;
                case WorkerStateStage.ChooseService:
                    // выбрали сервис - выберем способ оплаты
                    Stage = WorkerStateStage.ChoosePay;
                    break;
                case WorkerStateStage.PayBillService:
                    // выбрали сервис - выберем способ оплаты
                    Stage = WorkerStateStage.PayBillService;
                    break;
                case WorkerStateStage.PayCheckService:
                    // выбрали сервис - выберем способ оплаты
                    Stage = WorkerStateStage.PayCheckService;
                    break;
                case WorkerStateStage.StartService:
                    // выбрали сервис - выберем способ оплаты
                    Stage = WorkerStateStage.StartService;
                    break;
                case WorkerStateStage.EndService:
                    // все сделали переходим в режим ожидания
                    Stage = WorkerStateStage.AgainWait;
                    break;
                case WorkerStateStage.ExitProgram:
                    // выход
                    Close();
                    return;
                case WorkerStateStage.ManualSetting:
                    // вход в настройки
                    drivers.ManualInitDevice();

                    Stage = WorkerStateStage.Setting;
                    break;
                case WorkerStateStage.UserRequestService:
                    // идентификация пользователя  из формы сервиса
                    break;
            }

            // запустим обработчик действия пользователя
            MainWorkerTask.Run();
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
                    WorkerWait.Abort();
                    MessageBox.Show("Нет доступных COM портов. Дальнейшая работа бесмысленна.");
                    Close();
                    break;
                case DeviceEvent.InitializationOK:
                    break;
                case DeviceEvent.NeedSettingProgram:
                    break;
                case DeviceEvent.DropCassetteBillAcceptor:
                    // выемка денег
                    DateTime dt = GlobalDb.GlobalBase.GetLastEncashment();

                    int countmoney = 0;
                    if (dt != null)
                    {
                        countmoney = GlobalDb.GlobalBase.GetCountMoney(dt);
                    }
                    else
                    {
                        countmoney = GlobalDb.GlobalBase.GetCountMoney(new DateTime(2000,1,1));
                    }

                    GlobalDb.GlobalBase.Encashment(CurrentUserId, countmoney);
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:
                    // я полный
                    break;
            }
        }

        private void hideMainForm()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StartNextForm(hideMainForm));
                return;
            }

            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setting = new FormSettings(drivers, this);
            setting.Show();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // запустим основной обработчик - инициализация
            Stage = WorkerStateStage.Init;
            MainWorkerTask.Run();
        }

        private void WorkerWait_Complete(object sender, ThreadCompleteEventArgs e)
        {
            wait.Hide();
        }

        private void WorkerWait_Work(object sender, ThreadWorkEventArgs e)
        {
            wait = new FormWait();
            wait.Show();

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
    }
}
