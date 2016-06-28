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
    public partial class FormRuleService : Form
    {
        MachineDrivers drivers;
        Form form;

        public FormRuleService(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;
        }

        public FormRuleService()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // согласен с правилами - дальше
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // не согласен с правилами - опять ожидание клиента
            ((MainForm)form).Stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormRuleService_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        private void FormRuleService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
