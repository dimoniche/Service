using ServiceSaleMachine.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormChoosePay : Form
    {
        MachineDrivers drivers;
        Form form;

        public FormChoosePay()
        {
            InitializeComponent();
        }

        public FormChoosePay(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;

            pbxCheck.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonCheck);
            pbxMoney.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonMoney);
        }

        private void FormChoosePay_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // оплата по чеку
            ((MainForm)form).Stage = WorkerStateStage.PayCheckService;
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // оплата деньгами
            ((MainForm)form).Stage = WorkerStateStage.PayBillService;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            ((MainForm)form).Stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormChoosePay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
