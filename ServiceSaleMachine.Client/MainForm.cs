using ServiceSaleMachine.Drivers;
using System;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : Form
    {
        MachineDrivers drivers;

        FormSettings setting;
        Services services;

        FormWait wait;
        private SaleThread WorkerWait { get; set; }

        // задача очистки логов
        ClearFilesControlServiceTask ClearFilesTask { get; set; }

        // запуск приложения
        public MainForm()
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
                    MessageBox.Show("Нет доступных COM портов. Дальнейшая работа бесмысленна.");
                    Close();
                    break;
                case DeviceEvent.NeedSettingProgram:
                    this.Hide();
                    setting = new FormSettings(drivers,this);
                    setting.Show();
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setting = new FormSettings(drivers,this);
            setting.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            services = new Services();
            services.Show();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            int heght = this.Size.Height;
            int width = this.Size.Width;


        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            WorkerWait.Run();

            // инициализируем устройства
            drivers.InitAllDevice();

            WorkerWait.Abort();
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
                    SaleThread.Sleep(100);
                }
            }
        }
    }
}
