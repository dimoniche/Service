using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormInsertBill : MyForm
    {
        string Nominal;
        bool result = false;

        public FormInsertBill()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            Nominal = "00";

            pictureBox1.Load(Globals.GetPath(PathEnum.Image) + "\\TakeAwayMoney.png");
            pictureBox2.Load(Globals.GetPath(PathEnum.Image) + "\\returnMoney.png");

            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(string))
                {
                    Nominal = (string)obj;
                }
            }

            label1.Text = "Вы внесли купюру достоинством " + Nominal + " руб";
        }

        private void FormInsertBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = result;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            result = true;
            Close();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            result = false;
            Close();
        }
    }
}
