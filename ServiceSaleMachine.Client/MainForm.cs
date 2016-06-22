using ServiceSaleMachine.Drivers;
using System;
using System.Drawing;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : Form
    {
        int FCurrentPage;
        int FServCount;
        int FPageCount;
        string EmptyServ;

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

            ServCount = Globals.ClientConfiguration.ServCount;

            EmptyServ = Globals.GetPath(PathEnum.Image) + "\\serv0.bmp";

            CurrentPage = 0;
            pbxNext.BackColor = Color.Transparent;

            this.WindowState = FormWindowState.Maximized;
        }

        public int ServCount
        {
            get { return FServCount; }
            set
            {
                FServCount = value;
                FPageCount = value / 4 + 1;
            }
        }

        public int CurrentPage
        {
            get { return FCurrentPage; }
            set
            {
                if (value > FServCount / 4)
                    FCurrentPage = FServCount / 4;
                else
                    FCurrentPage = value;

                pbxPrev.Visible = FCurrentPage != 0;
                pbxNext.Visible = FCurrentPage != (FServCount / 4);

                pictureBox1.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 1)) ||
                    (FCurrentPage < FPageCount);
                pictureBox2.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 2)) ||
                    (FCurrentPage < FPageCount - 1);
                pictureBox3.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 3)) ||
                    (FCurrentPage < FPageCount - 1);
                pictureBox4.Enabled = ((FPageCount == FCurrentPage + 1) && ((FServCount % 4 == 2) && (FServCount != 0))) ||
                    (FCurrentPage < FPageCount - 1);

                if (pictureBox1.Enabled)
                    pictureBox1.Load(Globals.ClientConfiguration.ServiceByIndex(FCurrentPage * 4).filename);
                else
                    pictureBox1.Load(EmptyServ);

                if (pictureBox2.Enabled)
                    pictureBox2.Load(Globals.ClientConfiguration.ServiceByIndex(FCurrentPage * 4 + 1).filename);
                else
                    pictureBox2.Load(EmptyServ);

                if (pictureBox3.Enabled)
                    pictureBox3.Load(Globals.ClientConfiguration.ServiceByIndex(FCurrentPage * 4 + 2).filename);
                else
                    pictureBox3.Load(EmptyServ);

                if (pictureBox4.Enabled)
                    pictureBox4.Load(Globals.ClientConfiguration.ServiceByIndex(FCurrentPage * 4 + 3).filename);
                else
                    pictureBox4.Load(EmptyServ);

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

        private void pbxNext_Click(object sender, EventArgs e)
        {
            if (FCurrentPage + 1 < FPageCount)
                CurrentPage += 1;
        }

        private void pbxPrev_Click(object sender, EventArgs e)
        {
            if (FCurrentPage + 1 > 0)
                CurrentPage -= 1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int current = CurrentPage * 4;
            FormProgress fprgs = new FormProgress();
            Service serv = Globals.ClientConfiguration.ServiceByIndex(current);
            fprgs.timework = serv.timework;
            fprgs.ServName = serv.caption;
            fprgs.Start();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
