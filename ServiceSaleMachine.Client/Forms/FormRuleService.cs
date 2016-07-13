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

            pbxYes.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonYes);
            pbxNo.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonFail);
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
            Params.Result = data;
        }

        private void FormRuleService_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
