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

            constructor(null, null);
        }

        public FormWaitPayCheck(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            constructor(drivers, form);
        }

        void constructor(MachineDrivers drivers, Form form)
        {
            this.drivers = drivers;
            this.form = form;

            pbxFail.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonFail);
            pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonForward);
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
