using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormChooseChange : MyForm
    {
        ChooseChangeEnum ch = ChooseChangeEnum.None;

        public FormChooseChange()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            if (Globals.ClientConfiguration.Settings.changeToAccount > 0)
            {
                pictureBox2.Load(Globals.GetPath(PathEnum.Image) + "\\ChangeToAccount.png");
            }
            else
            {
                pictureBox2.Enabled = false;
            }

            if (Globals.ClientConfiguration.Settings.changeToCheck > 0)
            {
                pictureBox1.Load(Globals.GetPath(PathEnum.Image) + "\\ChangeToCheck.png");
            }
            else
            {
                pictureBox1.Enabled = false;
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
