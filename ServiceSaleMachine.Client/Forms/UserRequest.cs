using ServiceSaleMachine.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class UserRequest : Form
    {
        MachineDrivers drivers;
        Form form;
        public string retLogin;
        public string retPassword;

        public UserRequest()
        {
            InitializeComponent();
        }

        public UserRequest(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;
            pbxOk.Load(Globals.GetPath(PathEnum.Image) + "\\yes.jpg");
            pbxCancel.Load(Globals.GetPath(PathEnum.Image) + "\\fail.png");
        }

        private void UserRequest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(((MainForm)form).Stage == WorkerStateStage.UserRequestService)
            {
                ((MainForm)form).Stage = WorkerStateStage.Rules;
            }

            // покажем основную форму
            form.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            retLogin = tbxLogin.Text;
            retPassword = tbxPassword.Text;
            Close();
        }

        private void pbxCancel_Click(object sender, EventArgs e)
        {
            retLogin = "";
        }
    }
}
