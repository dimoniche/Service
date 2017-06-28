using System;
using System.Data;
using System.Drawing;
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

        int countRetry = 0;

        Image image;

        public UserRequest()
        {
            InitializeComponent();

            pictureTelephon.Load(Globals.GetPath(PathEnum.Image) + "\\Phone_txt.png");
            picturePassword.Load(Globals.GetPath(PathEnum.Image) + "\\Password_txt.png");

            pBxRegister.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRegister);
            pBxRemember.Load(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.ButtonRemember);

            LoadFullKeyBoard();
            tbx = textBox1;

            image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Phone_b.png");
            tableLayoutPhoneEdit.BackgroundImage = image;

            image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b.png");
            tableLayoutPasswordEdit.BackgroundImage = image;
        }

        public void LoadFullKeyBoard()
        {
            string[,] str = new string[NumberBoard.CountRow, NumberBoard.CountCol];

            str[0, 0] = Globals.GetPath(PathEnum.Image) + "\\1_b.png";
            str[0, 1] = Globals.GetPath(PathEnum.Image) + "\\2_b.png";
            str[0, 2] = Globals.GetPath(PathEnum.Image) + "\\3_b.png";
            str[1, 0] = Globals.GetPath(PathEnum.Image) + "\\4_b.png";
            str[1, 1] = Globals.GetPath(PathEnum.Image) + "\\5_b.png";
            str[1, 2] = Globals.GetPath(PathEnum.Image) + "\\6_b.png";
            str[2, 0] = Globals.GetPath(PathEnum.Image) + "\\7_b.png";
            str[2, 1] = Globals.GetPath(PathEnum.Image) + "\\8_b.png";
            str[2, 2] = Globals.GetPath(PathEnum.Image) + "\\9_b.png";
           
            str[3, 0] = Globals.GetPath(PathEnum.Image) + "\\Del_b.png";
            str[3, 1] = Globals.GetPath(PathEnum.Image) + "\\0_b.png";
            str[3, 2] = Globals.GetPath(PathEnum.Image) + "\\Ok_b.png";

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

            if (Globals.ClientConfiguration.Settings.offModem == 1)
            {
                tableLayoutPanel11.Visible = false;
            }

            ErrorText.Font = new Font(data.FontCollection.Families[CustomFont.CeraRoundPro_Medium], 72, FontStyle.Regular);
            ErrorText.ForeColor = Color.Gray;
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

                        Globals.UserConfiguration.UserLogin = "";
                        Globals.UserConfiguration.UserPassword = "";
                    }
                }

                Params.Result = data;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            data.retLogin = textBox1.Text;
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
                if (User.Length < 10 && tbx == textBox1
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == textBox1)
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
                if (User.Length < 10 && tbx == textBox1
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == textBox1)
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
                if (User.Length < 10 && tbx == textBox1
                 || Password.Length < 4 && tbx == tbxPassword)
                {
                    if (tbx == textBox1)
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
                    if (tbx == textBox1)
                    {
                        if (tbx.Text.Length > 2)
                        {
                            textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                            User = User.Remove(User.Length - 1);
                        }
                        else
                        {
                            // выйти в главное меню
                            data.stage = WorkerStateStage.MainScreen;
                            Close();
                        }
                    }
                    else
                    {
                        if (tbx.Text.Length > 0)
                        {
                            tbxPassword.Text = tbxPassword.Text.Remove(tbxPassword.Text.Length - 1);
                            Password = Password.Remove(Password.Length - 1);
                        }
                        else
                        {
                            // выйти в главное меню
                            data.stage = WorkerStateStage.MainScreen;
                            Close();
                        }
                    }
                }
                else if (e.Message.X == 1)
                {
                    if (User.Length < 10 && tbx == textBox1
                     || Password.Length < 4 && tbx == tbxPassword)
                    {
                        if (tbx == textBox1)
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
                    if (tbx == textBox1)
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

                    if (tbx == textBox1)
                    {
                        tbx = tbxPassword;

                        image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b_" + Password.Length + ".png");
                        tableLayoutPasswordEdit.BackgroundImage = image;

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
                                textBox1.Text = "+7";
                                tbxPassword.Text = "";
                                ErrorText.Text = "Вы ввели неправильный номер или пароль. Попробуйте еще раз.";

                                tbx = textBox1;

                                data.log.Write(LogMessageType.Information, "Ввели неправильный пароль");
                                data.log.Write(LogMessageType.Information, User);
                                data.log.Write(LogMessageType.Information, Password);

                                User = "";
                                Password = "";

                                image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b.png");
                                tableLayoutPasswordEdit.BackgroundImage = image;
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

            if (tbx == tbxPassword)
            {
                image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b_" + Password.Length + ".png");
                tableLayoutPasswordEdit.BackgroundImage = image;
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
            textBox1.Text = "+7";
            tbxPassword.Text = "";
            tbx = textBox1;

            User = "";
            Password = "";

            image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b.png");
            tableLayoutPasswordEdit.BackgroundImage = image;
        }

        private void pBxRemember_Click(object sender, EventArgs e)
        {
            rememberBox.Checked = true;
            pBxRegister.Visible = false;
            pBxRemember.Visible = false;
            tbxPassword.Visible = false;

            //tablePassword.Visible = false;

            ErrorText.Text = "";
            textBox1.Text = "+7";
            tbxPassword.Text = "";
            tbx = textBox1;

            User = "";
            Password = "";

            image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Password_b.png");
            tableLayoutPasswordEdit.BackgroundImage = image;
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
