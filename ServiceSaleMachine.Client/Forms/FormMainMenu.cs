using ServiceSaleMachine.Drivers;
using System;
using System.Data;
using System.Drawing;
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

            Image image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\" + Globals.DesignConfiguration.Settings.PanelBackGround);
            this.BackgroundImage = image;

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxBegin, Globals.DesignConfiguration.Settings.ButtonStartServices);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxPhilosof, Globals.DesignConfiguration.Settings.ButtonPhilosof);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxInstruction, Globals.DesignConfiguration.Settings.ButtonHelp);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureBoxInter, Globals.DesignConfiguration.Settings.ButtonInter);

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
            // читаем состояние устройства
            byte[] res;
            res = data.drivers.control.GetStatusControl(data.log);

            if (res != null)
            {
                if (res[0] == 0)
                {
                    data.stage = WorkerStateStage.ErrorControl;
                    this.Close();
                }
            }
        }

        private void FormMainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
            timer1.Enabled = false;
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            if (data.log != null)
            {
                data.log.Write(LogMessageType.Debug, "MAIN MENU: Событие: " + e.Message.Content + ".");
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
            Globals.HelpFileName = Globals.GetPath(PathEnum.Text) + "\\Instruction.rtf";
            data.stage = WorkerStateStage.Rules;
            this.Close();
        }

        private void FormMainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
            else if (e.Alt & e.KeyCode == Keys.F5)
            {
                if (Globals.IsDebug)
                {
                    // пошлем событие вставки обратно приемника
                    Drivers.Message message = new Drivers.Message();

                    message.Event = DeviceEvent.DropCassetteBillAcceptor;
                    message.Content = "Drop bill";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    reciveResponse(null, e1);
                }
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

        private void pbxLogin_Click(object sender, EventArgs e)
        {
            FormManager.OpenForm<UserRequest>(this, FormShowTypeEnum.Dialog, FormReasonTypeEnum.Modify);
           // UserRequest ureq = new UserRequest();
           // ureq.LoadFullKeyBoard();
           // ureq.ShowDialog();
        }

        private void pBxPhilosof_Click(object sender, EventArgs e)
        {
            Globals.HelpFileName = Globals.GetPath(PathEnum.Text) + "\\Philosof.rtf";
            data.stage = WorkerStateStage.Philosof;
            this.Close();
        }

        private void pictureBoxInter_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.InterUser;
            this.Close();
        }
    }
}
