using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormBigBill : MyForm
    {
        string Nominal = "";

        public FormBigBill()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            Nominal = "00";

            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(string))
                {
                    Nominal = (string)obj;
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTakeAwayMoney, Globals.DesignConfiguration.Settings.ButtonOK);

            timer1.Enabled = true;
        }

        private void FormBigBill1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Close();
        }

        private void pBxTakeAwayMoney_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Close();
        }
    }
}
