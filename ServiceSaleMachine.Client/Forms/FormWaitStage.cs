using ServiceSaleMachine.Drivers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitStage : MyForm
    {
        FormResultData data;

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }
        }

        public FormWaitStage()
        {
            InitializeComponent();
        }

        private void FormWaitStage_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.Rules;
            this.Close();
        }

        private void FormWaitStage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        //private void MediaPlayer_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        //{
        //    this.Close();
        //}

        private void FormWaitStage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ManualSetting;
            this.Close();
        }
    }
}
