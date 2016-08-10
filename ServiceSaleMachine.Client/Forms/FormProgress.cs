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
        FormResultData data;

        public int CurrentWork;
        public int FTimeWork;
        public string ServName;

        public FormProgress()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            FTimeWork = data.timeRecognize;
            progressBar.Maximum = data.timeRecognize;
            progressBar.Minimum = 0;
            progressBar.Value = 0;
            CurrentWork = 0;

            timer1.Enabled = true;
            this.WindowState = FormWindowState.Maximized;

            pBxInstruction.Load(Globals.GetPath(PathEnum.Image) + "\\instruction.png");

            // включаем подсветку
            data.drivers.SendOpenControl((int)ControlDeviceEnum.light1);
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

                data.stage = WorkerStateStage.EndService;

                this.Close();
            }
        }

        private void FormProgress_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Отключаем подстветку
            data.drivers.SendCloseControl((int)ControlDeviceEnum.light1);

            Params.Result = data;
        }

        private void FormProgress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
