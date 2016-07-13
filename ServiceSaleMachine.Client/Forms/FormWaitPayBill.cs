using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitPayBill : MyForm
    {
        FormResultData data;

        public FormWaitPayBill()
        {
            InitializeComponent();

            pbxFail.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonFail);
            pbxForward.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.ClientConfiguration.Settings.ButtonForward);
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

        private void FormWaitPayBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // запуск услуги
            data.stage = WorkerStateStage.StartService;
            this.Close();
        }

        private void FormWaitPayBill_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
