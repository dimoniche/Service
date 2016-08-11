using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormChooseService1 : MyForm
    {
        FormResultData data;

        int Timeout = 0;

        public FormChooseService1()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxService1, Globals.DesignConfiguration.Settings.ButtonService1);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxService2, Globals.DesignConfiguration.Settings.ButtonService2);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxreturntoMain, Globals.DesignConfiguration.Settings.ButtonRetToMain);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxWhatsDiff, Globals.DesignConfiguration.Settings.ButtonWhatsDiff);

            TimeOutTimer.Enabled = true;
            Timeout = 0;
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
        }

        private void TimeOutTimer_Tick(object sender, System.EventArgs e)
        {
            Timeout++;

            if (Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }

        private void FormChooseService1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;
            Params.Result = data;
        }

        private void FormChooseService1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pBxService1_Click(object sender, System.EventArgs e)
        {
            data.numberService = 0;
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }

        private void pBxService2_Click(object sender, System.EventArgs e)
        {
            data.numberService = 1;
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }

        private void pBxreturntoMain_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.TimeOut;
            this.Close();
        }

        private void pBxWhatsDiff_Click(object sender, System.EventArgs e)
        {

        }
    }
}
