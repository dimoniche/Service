using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class UserRequest : MyForm
    {
        FormResultData data;
        TextBox tbx;

        public UserRequest()
        {
            InitializeComponent();

            pbxOk.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonYes);
            pbxCancel.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonFail);
            pbxEnterName.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonEnterUserName);
            pbxEnterPsw.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonEnterUserPasw);
            LoadFullKeyBoard();
            tbx = tbxLogin;

       /*     string[,] str = new string[NumberBoard.CountRow, NumberBoard.CountCol ];

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

            NumberBoard.LoadPicture(str);*/

        }

        public void LoadNumberKeyBoard()
        {
          
        }

        public void LoadFullKeyBoard()
        {
           // NumberBoard.CountRow = 4;
           // NumberBoard.CountCol = 10;
            string[,] str = new string[NumberBoard.CountRow, NumberBoard.CountCol];

            str[0, 0] = Globals.GetPath(PathEnum.Image) + "\\1.png";
            str[0, 1] = Globals.GetPath(PathEnum.Image) + "\\2.png";
            str[0, 2] = Globals.GetPath(PathEnum.Image) + "\\3.png";
            str[0, 3] = Globals.GetPath(PathEnum.Image) + "\\4.png";
            str[0, 4] = Globals.GetPath(PathEnum.Image) + "\\5.png";
            str[0, 5] = Globals.GetPath(PathEnum.Image) + "\\6.png";
            str[0, 6] = Globals.GetPath(PathEnum.Image) + "\\7.png";
            str[0, 7] = Globals.GetPath(PathEnum.Image) + "\\8.png";
            str[0, 8] = Globals.GetPath(PathEnum.Image) + "\\9.png";
            str[0, 9] = Globals.GetPath(PathEnum.Image) + "\\0.png";
            str[1, 0] = Globals.GetPath(PathEnum.Image) + "\\q.png";
            str[1, 1] = Globals.GetPath(PathEnum.Image) + "\\w.png";
            str[1, 2] = Globals.GetPath(PathEnum.Image) + "\\e.png";
            str[1, 3] = Globals.GetPath(PathEnum.Image) + "\\r.png";
            str[1, 4] = Globals.GetPath(PathEnum.Image) + "\\t.png";
            str[1, 5] = Globals.GetPath(PathEnum.Image) + "\\y.png";
            str[1, 6] = Globals.GetPath(PathEnum.Image) + "\\u.png";
            str[1, 7] = Globals.GetPath(PathEnum.Image) + "\\i.png";
            str[1, 8] = Globals.GetPath(PathEnum.Image) + "\\o.png";
            str[1, 9] = Globals.GetPath(PathEnum.Image) + "\\p.png";
            str[2, 0] = Globals.GetPath(PathEnum.Image) + "\\a.png";
            str[2, 1] = Globals.GetPath(PathEnum.Image) + "\\s.png";
            str[2, 2] = Globals.GetPath(PathEnum.Image) + "\\d.png";
            str[2, 3] = Globals.GetPath(PathEnum.Image) + "\\f.png";
            str[2, 4] = Globals.GetPath(PathEnum.Image) + "\\g.png";
            str[2, 5] = Globals.GetPath(PathEnum.Image) + "\\h.png";
            str[2, 6] = Globals.GetPath(PathEnum.Image) + "\\j.png";
            str[2, 7] = Globals.GetPath(PathEnum.Image) + "\\k.png";
            str[2, 8] = Globals.GetPath(PathEnum.Image) + "\\l.png";
            str[2, 9] = Globals.GetPath(PathEnum.Image) + "\\delete.png";
            str[3, 1] = Globals.GetPath(PathEnum.Image) + "\\z.png";
            str[3, 2] = Globals.GetPath(PathEnum.Image) + "\\x.png";
            str[3, 3] = Globals.GetPath(PathEnum.Image) + "\\c.png";
            str[3, 4] = Globals.GetPath(PathEnum.Image) + "\\v.png";
            str[3, 5] = Globals.GetPath(PathEnum.Image) + "\\b.png";
            str[3, 6] = Globals.GetPath(PathEnum.Image) + "\\n.png";
            str[3, 7] = Globals.GetPath(PathEnum.Image) + "\\m.png";

            str[3, 0] = Globals.GetPath(PathEnum.Image) + "\\fail.png";
            str[3, 8] = Globals.GetPath(PathEnum.Image) + "\\Yes.jpg";

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
            if (data != null)
            {
                // вернем вниз ID пользователя
                UserInfo ui = GlobalDb.GlobalBase.GetUserByName(Globals.UserConfiguration.UserLogin, Globals.UserConfiguration.UserPassword);
                if ( ui != null)
                { data.CurrentUserId = ui.Id; }

                Params.Result = data;
            }
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

        private bool AddInDB()
        {
            if (GlobalDb.GlobalBase.InsertUser(tbxLogin.Text, tbxPassword.Text))
            {
                return true; 
                
            }
            return false;
        }

        private void NumberBoard_KeyboardEvent(object sender, KeyBoardEventArgs e)
        {
            string row0 = "1234567890";
            string row1 = "QWERTYUIOP";
            string row2 = "ASDFGHJKL";
            string row3 = "ZXCVBNM";
             
            if (e.Message.Y == 0)//первая строка
            { tbx.Text += row0[e.Message.X]; }
            else
            if (e.Message.Y == 1)//первая строка
            { tbx.Text += row1[e.Message.X]; }
            else
            if (e.Message.Y == 2)//вторая строка
            {
                if (e.Message.X < 9)
                { tbx.Text += row2[e.Message.X]; }
                else
                { //стереть символ последний
                    string ss = tbxLogin.Text;
                    if (ss.Length > 0)
                    { tbx.Text = ss.Remove(ss.Length - 1); }
                }
            }
            else
            if (e.Message.Y == 3)//третья строка
            {
                if ((e.Message.X < 8) && (e.Message.X > 0))
                {
                    tbx.Text += row3[e.Message.X-1];
                }else
                if (e.Message.X == 0)
                { this.Close(); }
                else
                if (e.Message.X == 8) //типа Enter
                {
                    if (tbx == tbxLogin)
                    {
                        tbx = tbxPassword;
                        tbxLogin.BackColor = System.Drawing.Color.Gray;
                        tbxPassword.BackColor = System.Drawing.Color.Lime;
                        return;
                    }
                    Globals.UserConfiguration.UserLogin = tbxLogin.Text;
                    Globals.UserConfiguration.UserPassword = tbxPassword.Text;
                    if (chbNew.Checked)
                    {
                        if (AddInDB())
                        {
                            //успешно занеслось в БД
                            UserInfo ui = GlobalDb.GlobalBase.GetUserByName(tbxLogin.Text, tbxPassword.Text);
                            if (ui != null)
                            {
                                Globals.UserConfiguration.ID = ui.Id;
                                //получили ID из БД
                            }
                        }
                        else
                        {
                            Globals.UserConfiguration.Clear();
                        }

                    }
                    else
                    {
                        //проверить - есть такой в БД?
                        UserInfo ui = GlobalDb.GlobalBase.GetUserByName(tbxLogin.Text, tbxPassword.Text);
                        if (ui == null) return;
                        int sum = GlobalDb.GlobalBase.GetUserMoney(ui.Id);
                        Globals.UserConfiguration.Amount = sum;

                    }
                    this.Close();
                }
            }

        }

        private void pbxOk_Click(object sender, EventArgs e)
        {

        }

        private void pbxCancel_Click_1(object sender, EventArgs e)
        {

        }
    }
}
