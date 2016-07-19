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
    public partial class FormMainMenu : MyForm
    {
        FormResultData data;

        public FormMainMenu()
        {
            InitializeComponent();
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxHelp, Globals.ClientConfiguration.Settings.ButtonHelp);
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxLogo, Globals.ClientConfiguration.Settings.ButtonLogo);
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxStart, Globals.ClientConfiguration.Settings.ButtonStartServices);
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

            timer1.Enabled = true;
            this.WindowState = FormWindowState.Maximized;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void FormMainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }
    }
}
