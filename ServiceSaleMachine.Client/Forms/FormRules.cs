using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;
using System.Drawing;

namespace AirVitamin.Client
{
    public partial class FormRules : MyForm
    {
        FormResultData data;
        private int Timeout;

        public bool IsSendSMS1 = false;
        public bool IsSendSMS2 = false;
        public bool IsSendSMS3 = false;
        public bool IsSendSMS4 = false;

        public FormRules()
        {
            InitializeComponent();

            timer1.Enabled = true;
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

            Image image = Image.FromFile(Globals.GetPath(PathEnum.Image) + "\\Instraction.png");
            tableLayoutInstruction.BackgroundImage = image;

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxInstructionTitle, "Instraction_txt.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMainMenu, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            data.drivers.ReceivedResponse += reciveResponse;
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

            res = data.drivers.control.GetStatusRelay(data.log);

            if (res != null)
            {
                // просто шлем смс - не выходим пока с ошибкой
                if (res[0] == 1 && !IsSendSMS1)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 1", data.log);

                    data.stage = WorkerStateStage.Gas1_low;
                    IsSendSMS1 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД1 - HIGH.");
                }
                else if (res[0] == 0) IsSendSMS1 = false;

                if (res[1] == 1 && !IsSendSMS2)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 2", data.log);

                    data.stage = WorkerStateStage.Gas2_low;
                    IsSendSMS2 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД2 - HIGH.");
                }
                else if (res[1] == 0) IsSendSMS2 = false;

                if (res[2] == 1 && !IsSendSMS3)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 3", data.log);

                    data.stage = WorkerStateStage.Gas3_low;
                    IsSendSMS3 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД3 - HIGH.");
                }
                else if (res[2] == 0) IsSendSMS3 = false;

                if (res[3] == 1 && !IsSendSMS4)
                {
                    data.drivers.modem.SendSMS("Низкое давление Газа 4", data.log);

                    data.stage = WorkerStateStage.Gas4_low;
                    IsSendSMS4 = true;

                    Program.Log.Write(LogMessageType.Error, "CHECK_STAT: РД4 - HIGH.");
                }
                else if (res[3] == 0) IsSendSMS4 = false;
            }

            Timeout++;

            if (Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;

                data.retLogin = "";
                data.retPassword = "";
                data.CurrentUserId = 0;

                Globals.UserConfiguration.UserLogin = "";
                Globals.UserConfiguration.UserPassword = "";

                this.Close();
            }
        }

        private void FormRules1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void pBxMainMenu_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            this.Close();
        }

        private void FormRules1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
                Close();
            }
        }

        private void InstructionText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
                Close();
            }
        }
    }
}
