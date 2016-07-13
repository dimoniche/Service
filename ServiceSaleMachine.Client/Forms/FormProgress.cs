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
    public partial class FormProgress : MyForm
    {
        MachineDrivers drivers;
        Form form;

        public int CurrentWork;
        public int FTimeWork;
        public string ServName;

        public FormProgress()
        {
            InitializeComponent();
        }

        public FormProgress(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;
        }

        public int timework
        {
            get { return 0; }
            set
            {
                FTimeWork = value;
                progressBar.Maximum = value;
                progressBar.Minimum = 0;
                progressBar.Value = 0;
                timer1.Enabled = true;
                CurrentWork = 0;
            }
        }

        public void Start()
        {
            timer1.Enabled = true;
            this.WindowState = FormWindowState.Maximized;
            this.ShowDialog();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentWork++;
            progressBar.Value++;
            label1.Text = String.Format("{0} \nОсталось времени работы {1} секунд.", ServName, FTimeWork - CurrentWork);
            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;

                ((MainForm)form).Stage = WorkerStateStage.EndService;

                this.Close();
            }
        }

        private void FormProgress_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        private void FormProgress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
