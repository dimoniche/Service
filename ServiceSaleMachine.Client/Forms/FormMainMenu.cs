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

            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxHelp, Globals.ClientConfiguration.Settings.ButtonHelp);
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxLogo, Globals.ClientConfiguration.Settings.ButtonLogo);
            Globals.ClientConfiguration.Settings.LoadPictureBox(pbxStart, Globals.ClientConfiguration.Settings.ButtonStartServices);

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
            lblTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

            if (Timeout > 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }
    }
}
