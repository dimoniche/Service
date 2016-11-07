﻿using System;
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

            pBxRegister.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRegister);
            pBxRemember.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRemember);

            LoadFullKeyBoard();
            tbx = tbxLogin;
        }

        public void LoadNumberKeyBoard()
        {
          
        }

        public void LoadFullKeyBoard()
        {
            string[,] str = new string[NumberBoard.CountRow, NumberBoard.CountCol];

            str[0, 0] = Globals.GetPath(PathEnum.Image) + "\\1.png";
            str[0, 1] = Globals.GetPath(PathEnum.Image) + "\\2.png";
            str[0, 2] = Globals.GetPath(PathEnum.Image) + "\\3.png";
            str[1, 0] = Globals.GetPath(PathEnum.Image) + "\\4.png";
            str[1, 1] = Globals.GetPath(PathEnum.Image) + "\\5.png";
            str[1, 2] = Globals.GetPath(PathEnum.Image) + "\\6.png";
            str[2, 0] = Globals.GetPath(PathEnum.Image) + "\\7.png";
            str[2, 1] = Globals.GetPath(PathEnum.Image) + "\\8.png";
            str[2, 2] = Globals.GetPath(PathEnum.Image) + "\\9.png";
           
            str[3, 0] = Globals.GetPath(PathEnum.Image) + "\\fail.png";
            str[3, 1] = Globals.GetPath(PathEnum.Image) + "\\0.png";
            str[3, 2] = Globals.GetPath(PathEnum.Image) + "\\Yes.jpg";

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
                {
                    data.CurrentUserId = ui.Id;

                    data.retLogin = Globals.UserConfiguration.UserLogin;
                    data.retPassword = Globals.UserConfiguration.UserPassword;
                }
                else
                {
                    data.CurrentUserId = 0;
                    data.retLogin = "";
                }

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
            string row0 = "123";
            string row1 = "456";
            string row2 = "789";
            string row3 = " 0 ";

            if (e.Message.Y == 0)
            {
                if (tbx.Text.Length < 10 && tbx == tbxLogin
                 || tbx.Text.Length < 4 && tbx == tbxPassword)
                {
                    tbx.Text += row0[e.Message.X];
                }
            }
            else if (e.Message.Y == 1)
            {
                if (tbx.Text.Length < 10 && tbx == tbxLogin
                 || tbx.Text.Length < 4 && tbx == tbxPassword)
                {
                    tbx.Text += row1[e.Message.X];
                }
            }
            else if (e.Message.Y == 2)
            {
                if (tbx.Text.Length < 10 && tbx == tbxLogin
                 || tbx.Text.Length < 4 && tbx == tbxPassword)
                {
                    tbx.Text += row2[e.Message.X];
                }
            }
            else if (e.Message.Y == 3)
            {
                if (e.Message.X == 0)
                {
                    // стереть символ последний
                    string ss = tbxLogin.Text;
                    if (ss.Length > 0)
                    {
                        tbx.Text = ss.Remove(ss.Length - 1);
                    }
                    else
                    {
                        // выйти в главное меню
                        data.stage = WorkerStateStage.MainScreen;
                        Close();
                    }
                }
                else if (e.Message.X == 1)
                {
                    if (tbx.Text.Length < 10 && tbx == tbxLogin
                     || tbx.Text.Length < 4 && tbx == tbxPassword)
                    {
                        tbx.Text += row3[e.Message.X];
                    }
                }
                else if (e.Message.X == 2)
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
                            // успешно занеслось в БД
                            UserInfo ui = GlobalDb.GlobalBase.GetUserByName(tbxLogin.Text, tbxPassword.Text);
                            if (ui != null)
                            {
                                Globals.UserConfiguration.ID = ui.Id;
                                // получили ID из БД
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

        private void UserRequest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pBxRegister_Click(object sender, EventArgs e)
        {
            chbNew.Checked = true;
            pBxRegister.Visible = false;
            pBxRemember.Visible = false;

            tbxLogin.Text = "";
            tbxPassword.Text = "";
            tbx = tbxLogin;

            tbxPassword.BackColor = System.Drawing.Color.Gray;
            tbxLogin.BackColor = System.Drawing.Color.Lime;
        }
    }
}
