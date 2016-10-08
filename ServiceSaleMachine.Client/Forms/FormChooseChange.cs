using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormChooseChange : MyForm
    {
        ChooseChangeEnum ch = ChooseChangeEnum.None;
        string Nominal = "";
        int Amount = 0;

        public FormChooseChange()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxAccount, Globals.DesignConfiguration.Settings.ButtonMoneyToAccount);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxCheck, Globals.DesignConfiguration.Settings.ButtonMoneyToCheck);

            if (Globals.ClientConfiguration.Settings.changeToAccount > 0)
            {
                pBxAccount.Enabled = true;
            }
            else
            {
                pBxAccount.Enabled = false;
            }

            if (Globals.ClientConfiguration.Settings.changeToCheck > 0)
            {
                pBxCheck.Enabled = true;
            }
            else
            {
                pBxCheck.Enabled = false;
            }

            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(string))
                {
                    Nominal = (string)obj;
                }
                else if (obj.GetType() == typeof(int))
                {
                    Amount = (int)obj;
                }
            }

            if (!string.IsNullOrEmpty(Nominal))
            {
                label1.Text = "Остается сдача в размере " + Nominal + " руб";
            }
            if (Amount != 0)
            {
                label1.Text = "Остается сдача в размере " + Amount + " руб";
            }
        }

        private void FormChooseChange_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = ch;
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            ch = ChooseChangeEnum.ChangeToAccount;
            Close();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            ch = ChooseChangeEnum.ChangeToCheck;
            Close();
        }
    }
}
