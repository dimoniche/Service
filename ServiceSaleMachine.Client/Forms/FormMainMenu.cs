using ServiceSaleMachine.Drivers;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormMainMenu : MyForm
    {
        FormResultData data;

        public FormMainMenu()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pbxHelp, Globals.DesignConfiguration.Settings.ButtonHelp);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pbxLogo, Globals.DesignConfiguration.Settings.ButtonLogo);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pbxStart, Globals.DesignConfiguration.Settings.ButtonStartServices);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pbxLogin, Globals.DesignConfiguration.Settings.ButtonUser);

            TimeOutTimer.Enabled = true;
            Timeout = 0;

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

            data.drivers.ReceivedResponse += reciveResponse;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = Globals.UserConfiguration.UserLogin + "\n" + Globals.UserConfiguration.Amount.ToString();
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
            LblDate.Text = DateTime.Now.ToString("yyyy-MM-dd dddd");
        }

        private void FormMainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.DropCassetteBillAcceptor:
                    {
                        data.stage = WorkerStateStage.DropCassettteBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.DropCassetteFullBillAcceptor:

                    break;
                case DeviceEvent.BillAcceptorError:

                    break;
            }
        }

        private void pbxStart_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ChooseService;
            this.Close();
        }

        private void pbxHelp_Click(object sender, EventArgs e)
        {
            Globals.HelpFileName = "HelpMain.txt";
            data.stage = WorkerStateStage.Rules;
            this.Close();
        }

        private void FormMainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
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

            if ((Timeout % 20) == 0)
            {
                // читаем состояние устройства
                byte[] res;
                res = data.drivers.GetStatusControl();

                if (res != null)
                {
                    if (res[0] > 0 || res[1] > 0)
                    {
                        data.stage = WorkerStateStage.ErrorControl;
                        this.Close();
                    }
                }
            }

        }

        private void pbxLogin_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm<UserRequest>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify);
           // UserRequest ureq = new UserRequest();
           // ureq.LoadFullKeyBoard();
           // ureq.ShowDialog();
        }

        private void pbxLogo_Click(object sender, EventArgs e)
        {

        }
    }
}
