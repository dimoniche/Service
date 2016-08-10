using System;
using System.Linq;

namespace ServiceSaleMachine.Client
{
    public partial class FormBigBill : MyForm
    {
        string Nominal = "";

        public FormBigBill()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        public override void LoadData()
        {
            Nominal = "00";

            pictureBox1.Load(Globals.GetPath(PathEnum.Image) + "\\TakeAwayMoney.png");

            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(string))
                {
                    Nominal = (string)obj;
                }
            }

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
