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
    public partial class FormWaitStage : Form
    {
        MachineDrivers drivers;
        Form form;

        public FormWaitStage(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;

            //MediaPlayer.Visible = true;
            //MediaPlayer.URL = Globals.GetPath(PathEnum.Video) + "\\advert.mpg";
        }

        public FormWaitStage()
        {
            InitializeComponent();
        }

        private void FormWaitStage_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormWaitStage_FormClosed(object sender, FormClosedEventArgs e)
        {
            // покажем основную форму
            form.Show();
        }

        //private void MediaPlayer_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        //{
        //    this.Close();
        //}

        private void FormWaitStage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Alt & e.KeyCode == Keys.F4)
            {
                ((MainForm)form).Stage = WorkerStateStage.ExitProgram;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ((MainForm)form).Stage = WorkerStateStage.ManualSetting;
            this.Close();
        }
    }
}
