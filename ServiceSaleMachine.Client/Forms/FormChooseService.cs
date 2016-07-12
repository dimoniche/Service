using ServiceSaleMachine.Drivers;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormChooseService : Form
    {
        int FCurrentPage;
        int FServCount;
        int FPageCount;
        string EmptyServ;

        MachineDrivers drivers;
        Form form;

        // запуск приложения
        public FormChooseService()
        {
            InitializeComponent();

            ServCount = Globals.ClientConfiguration.ServCount;

            EmptyServ = Globals.GetPath(PathEnum.Image) + "\\serv0.bmp";


            CurrentPage = 0;
            pbxNext.BackColor = Color.Transparent;

            this.WindowState = FormWindowState.Maximized;
        }

        public FormChooseService(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;

            ServCount = Globals.ClientConfiguration.ServCount;

            EmptyServ = Globals.GetPath(PathEnum.Image) + "\\serv0.bmp";
            pbxUser.Load(Globals.GetPath(PathEnum.Image) + "\\user.png");

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
            ((MainForm)form).numberService = CurrentPage * 4;

            this.Close();
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
            ((MainForm)form).numberService = CurrentPage * 4 + 3;

            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ((MainForm)form).numberService = CurrentPage * 4 + 2;

            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ((MainForm)form).numberService = CurrentPage * 4 + 1;

            this.Close();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        private void FormChooseService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {
            ((MainForm)form).Stage = WorkerStateStage.UserRequestService;
            this.Close();
        }
    }
}
