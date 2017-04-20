using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    public partial class UserRequest : MyForm
    {
        FormResultData data;
        ScalableLabel tbx;

        string User = "";
        string Password = "";

        bool fileLoaded = false;

        int countRetry = 0;

        public UserRequest()
        {
            InitializeComponent();

            pBxRegister.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRegister);
            pBxRemember.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRemember);

            LoadFullKeyBoard();
            tbx = tbxLogin;

            timer1.Enabled = true;
            timer1.Interval = 50;
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

            rTBxHelp.LoadFile(Globals.GetPath(PathEnum.Text) + "\\HelpAccount.rtf");

            if (Globals.ClientConfiguration.Settings.offModem == 1)
            {
                tableLayoutPanel11.Visible = false;
            }
        }

        private void UserRequest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (data != null)
            {
                if (data.stage == WorkerStateStage.FindPhone)
                {
                    data.CurrentUserId = 0;
                    data.retPassword = "";
                }
                else
                {
                    // вернем вниз ID пользователя
                    UserInfo ui = GlobalDb.GlobalBase.GetUserByName(Globals.UserConfiguration.UserLogin, Globals.UserConfiguration.UserPassword);

                    if (ui != null)
                    {
                        data.CurrentUserId = ui.Id;

                        data.retLogin = Globals.UserConfiguration.UserLogin;
                        data.retPassword = Globals.UserConfiguration.UserPassword;
                    }
                    else
                    {
                        data.CurrentUserId = 0;

                        data.retLogin = "";
                        data.retPassword = "";
                    }
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
            if (GlobalDb.GlobalBase.InsertUser(User, Password))
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

            ErrorText.Text = "";

            // кнопку нажали - сбросили тайм аут
            Timeout = 0;

            if (e.Message.Y == 0)
            {
                if (User.Length < 10 && tbx == tbxLogin
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == tbxLogin)
                    {
                        tbx.Text += row0[e.Message.X];
                        User += row0[e.Message.X];
                    }
                    else
                    {
                        tbx.Text += "*";
                        Password += row0[e.Message.X];
                    }
                }
            }
            else if (e.Message.Y == 1)
            {
                if (User.Length < 10 && tbx == tbxLogin
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == tbxLogin)
                    {
                        tbx.Text += row1[e.Message.X];
                        User += row1[e.Message.X];
                    }
                    else
                    {
                        tbx.Text += "*";
                        Password += row1[e.Message.X];
                    }
                }
            }
            else if (e.Message.Y == 2)
            {
                if (User.Length < 10 && tbx == tbxLogin
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == tbxLogin)
                    {
                        tbx.Text += row2[e.Message.X];
                        User += row2[e.Message.X];
                    }
                    else
                    {
                        tbx.Text += "*";
                        Password += row2[e.Message.X];
                    }
                }
            }
            else if (e.Message.Y == 3)
            {
                if (e.Message.X == 0)
                {
                    // стереть символ последний
                    if (tbx.Text.Length > 0)
                    {
                        if (tbx == tbxLogin)
                        {
                            tbxLogin.Text = tbxLogin.Text.Remove(tbxLogin.Text.Length - 1);
                            User = User.Remove(User.Length - 1);
                        }
                        else
                        {
                            tbxPassword.Text = tbxPassword.Text.Remove(tbxPassword.Text.Length - 1);
                            Password = Password.Remove(Password.Length - 1);
                        }
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
                    if (User.Length < 10 && tbx == tbxLogin
                     || Password.Length < 4 && tbx == tbxPassword)
                    {
                        if (tbx == tbxLogin)
                        {
                            tbx.Text += row3[e.Message.X];
                            User += row3[e.Message.X];
                        }
                        else
                        {
                            tbx.Text += "*";
                            Password += row3[e.Message.X];
                        }
                    }
                }
                else if (e.Message.X == 2)
                {
                    if (tbx == tbxLogin)
                    {
                        if (User.Length < 10)
                        {
                            ErrorText.Text = "Номер телефона должен состоять из 10 цифр";
                            data.log.Write(LogMessageType.Information, "Номер телефона должен состоять из 10 цифр");
                            data.log.Write(LogMessageType.Information, User);

                            return;
                        }
                    }
                    else
                    {
                        if(Password.Length < 4)
                        {
                            ErrorText.Text = "Пароль должен состоять из 4 цифр";
                            data.log.Write(LogMessageType.Information, "Пароль должен состоять из 4 цифр");
                            data.log.Write(LogMessageType.Information, Password);

                            return;
                        }
                    }

                    if (rememberBox.Checked)
                    {
                        // восстанавливаем пароль
                        data.stage = RestorePassword(User);

                        if (data.stage == WorkerStateStage.FindPhone)
                        {
                            data.retLogin = User;
                        }

                        Close();
                        return;
                    }

                    if (tbx == tbxLogin)
                    {
                        tbx = tbxPassword;
                        tbxLogin.BackColor = System.Drawing.Color.Gray;
                        tbxPassword.BackColor = System.Drawing.Color.Lime;
                        return;
                    }

                    Globals.UserConfiguration.UserLogin = User;
                    Globals.UserConfiguration.UserPassword = Password;

                    if (chbNew.Checked)
                    {
                        UserInfo ui = GlobalDb.GlobalBase.GetUserByName(User);

                        if (ui != null)
                        {
                            // есть такой пользователь - а пароль совпадает?
                            ui = GlobalDb.GlobalBase.GetUserByName(User, Password);

                            if (ui == null)
                            {
                                // такой пользователь есть - но пароль не совпадает
                                Globals.UserConfiguration.Clear();

                                data.stage = WorkerStateStage.ErrorRegisterNewUser;
                                Close();
                            }
                            else
                            {
                                // и пользователь такой есть и пароль совпадает
                                data.stage = WorkerStateStage.AuthorizeUser;
                                Close();
                            }
                        }
                        else
                        {
                            // новый пользователь - добавим его
                            if (AddInDB())
                            {
                                // успешно занеслось в БД
                                ui = GlobalDb.GlobalBase.GetUserByName(User, Password);
                                if (ui != null)
                                {
                                    Globals.UserConfiguration.ID = ui.Id;
                                    // получили ID из БД
                                }

                                // зарегистрировали нового пользователя
                                data.stage = WorkerStateStage.RegisterNewUser;
                                Close();
                            }
                            else
                            {
                                Globals.UserConfiguration.Clear();

                                data.stage = WorkerStateStage.NotAuthorizeUser;
                                Close();
                            }
                        }
                    }
                    else
                    {
                        // проверить - есть такой в БД?
                        UserInfo ui = GlobalDb.GlobalBase.GetUserByName(User, Password);

                        if (ui == null)
                        {
                            // нет такого пользователя
                            Globals.UserConfiguration.Clear();

                            // очередная попытка ввода пароля
                            countRetry++;

                            if (countRetry >= 5)
                            {
                                data.log.Write(LogMessageType.Information, "Превысили количество попыток воода пароля");
                                data.log.Write(LogMessageType.Information, User);
                                data.log.Write(LogMessageType.Information, Password);

                                User = "";
                                Password = "";

                                data.stage = WorkerStateStage.NotAuthorizeUser;
                                Close();
                            }
                            else
                            {
                                tbxLogin.Text = "";
                                tbxPassword.Text = "";
                                ErrorText.Text = "Вы ввели неправильный номер или пароль. Попробуйте еще раз.";

                                tbx = tbxLogin;
                                tbxLogin.BackColor = System.Drawing.Color.Lime;
                                tbxPassword.BackColor = System.Drawing.Color.Gray;

                                data.log.Write(LogMessageType.Information, "Ввели неправильный пароль");
                                data.log.Write(LogMessageType.Information, User);
                                data.log.Write(LogMessageType.Information, Password);

                                User = "";
                                Password = "";
                            }
                        }
                        else
                        {
                            // авторизовались
                            Globals.UserConfiguration.ID = ui.Id;

                            int sum = GlobalDb.GlobalBase.GetUserMoney(ui.Id);
                            Globals.UserConfiguration.Amount = sum;

                            // вошли
                            data.stage = WorkerStateStage.AuthorizeUser;
                            Close();
                        }
                    }
                }
            }
        }

        private WorkerStateStage RestorePassword(string phone)
        {
            string number = "+7" + phone;

            UserInfo ui = GlobalDb.GlobalBase.GetUserByName(phone);

            if (ui != null)
            {
                data.drivers.modem.SendSMS(ui.Password, null, number);

                return WorkerStateStage.FindPhone;
            }
            else
            {
                return WorkerStateStage.NotFindPhone;
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

            ErrorText.Text = "";
            tbxLogin.Text = "";
            tbxPassword.Text = "";
            tbx = tbxLogin;

            tbxPassword.BackColor = System.Drawing.Color.Gray;
            tbxLogin.BackColor = System.Drawing.Color.Lime;
        }

        private void pBxRemember_Click(object sender, EventArgs e)
        {
            rememberBox.Checked = true;
            pBxRegister.Visible = false;
            pBxRemember.Visible = false;
            tbxPassword.Visible = false;

            tablePassword.Visible = false;

            ErrorText.Text = "";
            tbxLogin.Text = "";
            tbxPassword.Text = "";
            tbx = tbxLogin;

            tbxLogin.BackColor = System.Drawing.Color.Lime;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!fileLoaded)
            {
                rTBxHelp.LoadFile(Globals.GetPath(PathEnum.Text) + "\\HelpAccount.rtf");

                fileLoaded = true;
                timer1.Interval = 1000;
                timer1.Enabled = false;
            }
        }

        int Timeout = 0;

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            Timeout++;

            if (Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }
    }
}
