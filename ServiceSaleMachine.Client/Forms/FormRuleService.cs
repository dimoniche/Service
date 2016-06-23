using ServiceSaleMachine.Drivers;
using ServiceSaleMachine.MainWorker;
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
            ((MainForm)form).Stage = WorkerStateStage.FailRules;
            this.Close();
        }

        private void FormRuleService_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }
    }
}
