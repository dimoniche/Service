using ServiceSaleMachine.Drivers;
using ServiceSaleMachine.MainWorker;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : Form
    {
        // стадии работы
        public WorkerStateStage Stage { get; set; }


        MachineDrivers drivers;

        FormSettings setting;
        FormWaitStage WaitStageForm;
        FormRuleService RuleStageForm;
        FormChooseService ChooseServiceForm;

        FormWait wait;
        private SaleThread WorkerWait { get; set; }
        private SaleThread MainWorkerTask { get; set; }


        // задача очистки логов
        ClearFilesControlServiceTask ClearFilesTask { get; set; }

        delegate void StartNextForm();

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
        }

        private void MainWorkerTask_ProgressChanged(object sender, ThreadProgressChangedEventArgs e)
        {

        }

        private void MainWorkerTask_Complete(object sender, ThreadCompleteEventArgs e)
        {
            try
            {
                switch (Stage)
                {
                    case WorkerStateStage.Init:

                        break;
                    case WorkerStateStage.Wait:

                        break;
                    case WorkerStateStage.Rules:

                        break;
                        /*case WorkerStateStage.:

                            break;
                        case WorkerStateStage.:

                            break;
                        case WorkerStateStage.:

                            break;*/
                }
            }
            catch (Exception err)
            {
            }
        }

        // основной рабочий обработчик
        private void MainWorkerTask_Work(object sender, ThreadWorkEventArgs e)
        {
            try
            {
                switch (Stage)
                {
                    case WorkerStateStage.Init:
                        WorkerWait.Run();

                        BeginInvoke(new StartNextForm(hideMainForm));

                        // инициализируем устройства
                        //if (drivers.InitAllDevice())
                        {
                            Stage = WorkerStateStage.Wait;          // переходим в ожидания
                            BeginInvoke(new StartNextForm(StartNextForm_Func));
                        }
                        /*else
                        {
                            Stage = WorkerStateStage.Setting;       // переходим в режим настройки
                            BeginInvoke(new StartNextForm(StartNextForm_Func));
                        }*/

                        WorkerWait.Abort();
                        break;
                    case WorkerStateStage.Wait:
                        BeginInvoke(new StartNextForm(StartNextForm_Func));
                        break;
                    case WorkerStateStage.Rules:
                        BeginInvoke(new StartNextForm(StartNextForm_Func));
                        break;
                    case WorkerStateStage.Setting:
                        break;

                     case WorkerStateStage.ChooseService:
                        BeginInvoke(new StartNextForm(StartNextForm_Func));
                        break;
                }
            }
            catch (Exception err)
            {
            }
        }

        private void hideMainForm()
        {
            this.Hide();
        }

        private void StartNextForm_Func()
        {
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
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            switch (Stage)
            {
                case WorkerStateStage.Setting:
                    Stage = WorkerStateStage.Init;
                    MainWorkerTask.Run();
                    break;
                case WorkerStateStage.Wait:
                    // вышли из режима ожидания - ознокомление с правилами
                    Stage = WorkerStateStage.Rules;
                    MainWorkerTask.Run();
                    break;
                case WorkerStateStage.Rules:
                    // вышли из режима ознокомления с правилами - теперь выберем услугу
                    Stage = WorkerStateStage.ChooseService;
                    MainWorkerTask.Run();
                    break;
                case WorkerStateStage.FailRules:
                    // вышли из режима ознокомления с правилами c отказом - опять ждем клиента
                    Stage = WorkerStateStage.Wait;
                    MainWorkerTask.Run();
                    break;
            }
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
            }
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
