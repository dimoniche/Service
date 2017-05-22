using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    public partial class FormRegisterNewUser : MyForm
    {
        FormResultData data;

        public FormRegisterNewUser()
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
                }
                else if (obj.GetType() == typeof(string))
                {
                    string Text = (string)obj;

                    if(Text.Contains("Зарегистрировали нового пользователя:"))
                    {
                        tableLayoutPanel3.ColumnStyles[0].Width = 26.25F;
                        tableLayoutPanel3.ColumnStyles[1].Width = 47.5F;
                        tableLayoutPanel3.ColumnStyles[2].Width = 26.25F;

                        Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Registration_ok.png");
                    }
                    else
                    {
                        Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Password_mail.png");
                    }
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");
        }

        private void FormRegisterNewUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void NewUserinfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            Close();
        }
    }
}
