using ServiceSaleMachine.Drivers;
using System;
using System.Linq;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class FormWaitStage : MyForm
    {
        FormResultData data;

        private bool fileLoaded = false;

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            data.drivers.ReceivedResponse += reciveResponse;
            timer1.Enabled = true;
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

        public FormWaitStage()
        {
            InitializeComponent();
        }

        private void FormWaitStage_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            this.Close();
        }

        private void FormWaitStage_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
            timer1.Enabled = false;
        }

        private void FormWaitStage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!fileLoaded)
            {
                TextWaitClient.LoadFile(Globals.GetPath(PathEnum.Text) + "\\WaitClient.rtf");
                fileLoaded = true;
                timer1.Interval = Globals.IntervalCheckControl;
            }

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
    }
}
