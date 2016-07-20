using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormChoosePay : MyForm
    {
        FormResultData data;

        public FormChoosePay()
        {
            InitializeComponent();

            pbxCheck.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonCheck);
            pbxMoney.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonMoney);
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

        private void FormChoosePay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // оплата по чеку
            data.stage = WorkerStateStage.PayCheckService;
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // оплата деньгами
            data.stage = WorkerStateStage.PayBillService;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // уходим в ожидание клиента
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void FormChoosePay_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
