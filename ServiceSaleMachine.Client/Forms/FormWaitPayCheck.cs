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
    public partial class FormWaitPayCheck : Form
    {
        MachineDrivers drivers;
        Form form;

        public FormWaitPayCheck()
        {
            InitializeComponent();
        }

        public FormWaitPayCheck(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            ((MainForm)form).Stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormWaitPayCheck_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // запуск услуги
            ((MainForm)form).Stage = WorkerStateStage.StartService;
            this.Close();
        }

        private void FormWaitPayCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
