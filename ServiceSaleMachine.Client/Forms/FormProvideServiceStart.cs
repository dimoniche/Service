using System;
using System.Data;
using System.Linq;

namespace ServiceSaleMachine.Client
{
    public partial class FormProvideServiceStart : MyForm
    {
        FormResultData data;
        int interval = 3;

        public FormProvideServiceStart()
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

                    interval = data.timework;
                }
            }
 
            pBxMinus.Load(Globals.GetPath(PathEnum.Image) + "\\back.png");
            pBxPlus.Load(Globals.GetPath(PathEnum.Image) + "\\forward.png");

            pBxServiceStart.Load(Globals.GetPath(PathEnum.Image) + "\\forward.png");
            pBxServiceStop.Load(Globals.GetPath(PathEnum.Image) + "\\fail.png");

            pBxInstruction.Load(Globals.GetPath(PathEnum.Image) + "\\instruction.png");
        }

        private void pBxPlus_Click(object sender, EventArgs e)
        {
            interval++;
            intervalLabel.Text = interval.ToString();
        }

        private void pBxMinus_Click(object sender, EventArgs e)
        {
            interval--;
            intervalLabel.Text = interval.ToString();
        }

        private void pBxServiceStart_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.Fail;
            data.timework = interval;
            Close();
        }

        private void pBxServiceStop_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.StartService;
            data.timework = interval;
            Close();
        }

        private void FormProvideService_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Params.Result = data;
        }
    }
}
