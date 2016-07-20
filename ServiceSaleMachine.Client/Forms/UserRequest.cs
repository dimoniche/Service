using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class UserRequest : MyForm
    {
        FormResultData data;

        public UserRequest()
        {
            InitializeComponent();

            pbxOk.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonYes);
            pbxCancel.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonFail);
            pbxEnterName.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonEnterUserName);
            pbxEnterPsw.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonEnterUserPasw);

            string[,] str = new string[NumberBoard.CountRow, NumberBoard.CountCol ];

            str[0, 0] = Globals.GetPath(PathEnum.Image) + "\\0.png";
            str[0, 1] = Globals.GetPath(PathEnum.Image) + "\\1.png";
            str[1, 0] = Globals.GetPath(PathEnum.Image) + "\\2.png";
            str[1, 1] = Globals.GetPath(PathEnum.Image) + "\\3.png";
            str[2, 0] = Globals.GetPath(PathEnum.Image) + "\\4.png";
            str[2, 1] = Globals.GetPath(PathEnum.Image) + "\\5.png";
            str[3, 0] = Globals.GetPath(PathEnum.Image) + "\\6.png";
            str[3, 1] = Globals.GetPath(PathEnum.Image) + "\\7.png";
            str[4, 0] = Globals.GetPath(PathEnum.Image) + "\\8.png";
            str[4, 1] = Globals.GetPath(PathEnum.Image) + "\\9.png";
            str[5, 0] = Globals.GetPath(PathEnum.Image) + "\\fail.png";
            str[5, 1] = Globals.GetPath(PathEnum.Image) + "\\Yes.jpg";

            NumberBoard.LoadPicture(str);

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

        private void UserRequest_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            data.retLogin = tbxLogin.Text;
            data.retPassword = tbxPassword.Text;
            Close();
        }

        private void pbxCancel_Click(object sender, EventArgs e)
        {
            data.retLogin = "";
            Close();
        }

        private void NumberBoard_KeyboardEvent(object sender, KeyBoardEventArgs e)
        {
            int numb = e.Message.X + e.Message.Y * 2;
            tbxLogin.Text += numb;
        }
    }
}
