using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        SettingsData sd;
        int FCurrentPage;
        int FServCount;
        int FPageCount;
        string PathToImg = Environment.CurrentDirectory + "\\..\\";
        string EmptyServ;
        db mydb;


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
        { get { return FCurrentPage; }
          set
            {

                if (value > FServCount / 4)
                    FCurrentPage = FServCount / 4;
                else
                    FCurrentPage = value;
                pbxPrev.Visible = FCurrentPage != 0;
                pbxNext.Visible = FCurrentPage != (FServCount / 4);
                pictureBox1.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 1)) ||
                    ( FCurrentPage < FPageCount);
                pictureBox2.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 2)) ||
                    (FCurrentPage < FPageCount - 1);
                pictureBox3.Enabled = ((FPageCount == FCurrentPage + 1) && (FServCount % 4 >= 3)) ||
                    (FCurrentPage < FPageCount - 1);
                pictureBox4.Enabled = ((FPageCount == FCurrentPage + 1) && ((FServCount % 4 == 2)&&(FServCount != 0))) ||
                    (FCurrentPage < FPageCount - 1);

                if (pictureBox1.Enabled)
                    pictureBox1.Load(sd.ServiceByIndex(FCurrentPage * 4).filename);
                else
                    pictureBox1.Load(EmptyServ);
                if (pictureBox2.Enabled)
                    pictureBox2.Load(sd.ServiceByIndex(FCurrentPage * 4 + 1).filename);
                else
                    pictureBox2.Load(EmptyServ);
                if (pictureBox3.Enabled)
                    pictureBox3.Load(sd.ServiceByIndex(FCurrentPage * 4 + 2).filename);
                else
                    pictureBox3.Load(EmptyServ);
                if (pictureBox4.Enabled)
                    pictureBox4.Load(sd.ServiceByIndex(FCurrentPage * 4 + 3).filename);
                else
                    pictureBox4.Load(EmptyServ);

            }
        }

        public Form1()
        {
            InitializeComponent();
            sd = new SettingsData();
            sd.LoadXml();
            ServCount = sd.ServCount;
            EmptyServ = PathToImg + "serv0.bmp";

            CurrentPage = 0;
            pbxNext.BackColor = Color.Transparent;

            this.WindowState = FormWindowState.Maximized;

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox5_Click_1(object sender, EventArgs e)
        {
            if (FCurrentPage + 1 > 0)
                CurrentPage -= 1;

        }

        private void pbxNext_Click(object sender, EventArgs e)
        {
            if (FCurrentPage + 1 < FPageCount)
                CurrentPage += 1;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int current = CurrentPage * 4;
            FormProgress fprgs = new FormProgress();
            Service serv = sd.ServiceByIndex(current);
            fprgs.timework = serv.timework;
            fprgs.ServName = serv.caption;
            fprgs.Start();


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

        private void btnCheckDB_Click(object sender, EventArgs e)
        {
            mydb = new db();
            if (mydb.Connect())
            { label1.Text = "ok"; }
            else
            { label1.Text = "error";
                return;
            }
            dataGridView1.DataSource = mydb.GetDevices();
            mydb.CreateTables();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            mydb.WriteWorkTime(1, 5);
        }
    }
}
