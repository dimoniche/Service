using AirVitamin.Drivers;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormMainMenu : MyForm
    {
        FormResultData data;

        public FormMainMenu()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxBegin, Globals.DesignConfiguration.Settings.ButtonStartServices);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxPhilosof, Globals.DesignConfiguration.Settings.ButtonPhilosof);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxInstruction, Globals.DesignConfiguration.Settings.ButtonHelp);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureBoxInter, Globals.DesignConfiguration.Settings.ButtonInter);

            TimeOutTimer.Enabled = true;
            Timeout = 0;

            if(Globals.ClientConfiguration.Settings.changeToAccount == 0)
            {
                pictureBoxInter.Visible = false;
            }
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

            if (data.retLogin != "")
            {
                numberTelefon.Text = "+7";
            }
            else
            {
                numberTelefon.Text = "";
            }

            numberTelefon.Text += data.retLogin;

            if (data.retLogin != "")
            {
                AmountText.Text = "Сумма: " + Globals.UserConfiguration.Amount + " руб.";
            }
            else
            {
                AmountText.Text = "";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReasonEnum reason = CheckError.GetStatus(data);

            if (reason == ReasonEnum.FormClose)
            {
                this.Close();
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
                    {
                        data.stage = WorkerStateStage.BillFull;
                        this.Close();
                    }
                    break;
                case DeviceEvent.BillAcceptorError:
                case DeviceEvent.DropCassetteJammed:
                case DeviceEvent.BillCheated:
                    {
                        // ошибка купюроприемника
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
                    break;
                case DeviceEvent.ConnectBillError:
                    {
                        // нет связи с купюроприемником
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
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
                    // пошлем событие вынимания приемника
                    Drivers.Message message = new Drivers.Message();

                    message.Event = DeviceEvent.DropCassetteBillAcceptor;
                    message.Content = "Drop bill";

                    ServiceClientResponseEventArgs e1 = new ServiceClientResponseEventArgs(message);

                    reciveResponse(null, e1);
                }
            }
            else if (e.Alt & e.KeyCode == Keys.F6)
            {
                if (Globals.IsDebug)
                {
                    data.stage = WorkerStateStage.ErrorControl;
                    Close();
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

                numberTelefon.Text = "";

                data.retLogin = "";
                data.retPassword = "";
                data.CurrentUserId = 0;

                this.Close();
            }
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
