using System;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormRuleService : MyForm
    {
        FormResultData data;

        public FormRuleService()
        {
            InitializeComponent();

            pbxYes.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonYes);
            pbxNo.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonFail);
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

            string file = Globals.GetPath(PathEnum.Text) + "\\Rule.txt";

            TimeOutTimer.Enabled = true;
            Timeout = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // согласен с правилами - дальше
            data.stage = WorkerStateStage.ChooseService;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // не согласен с правилами - опять ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormRuleService_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;
            Params.Result = data;
        }

        private void FormRuleService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        int Timeout = 0;

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            Timeout++;

            if (Timeout > 30)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }
    }
}
