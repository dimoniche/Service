using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormInsertBill : MyForm
    {
        FormResultData data;
        string Nominal;
        bool result = false;

        public FormInsertBill()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            Nominal = "00";

            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
                else if (obj.GetType() == typeof(string))
                {
                    Nominal = (string)obj;
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTakeAwayMoney, Globals.DesignConfiguration.Settings.ButtonTakeAwayMoney);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxreturnMoney, Globals.DesignConfiguration.Settings.ButtonreturnMoney);

            label1.Text = "Вы внесли купюру достоинством " + Nominal + " руб";
        }

        private void FormInsertBill1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = result;
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            result = true;
            Close();
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            result = false;
            Close();
        }

        private void label1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
