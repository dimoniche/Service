using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormProvideServiceStart : MyForm
    {
        FormResultData data;
        int interval = 3;

        public FormProvideServiceStart()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;

                    interval = data.timework;
                }
            }

            LabelNameService2.Text = Globals.ClientConfiguration.Settings.services[data.numberService].caption.ToLower();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStartService, Globals.DesignConfiguration.Settings.ButtonStartServices);

            intervalLabel.Text = interval.ToString() + " мин";

            pBxMinus.Load(Globals.GetPath(PathEnum.Image) + "\\back.png");
            pBxPlus.Load(Globals.GetPath(PathEnum.Image) + "\\forward.png");

            data.drivers.ReceivedResponse += reciveResponse;
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

        private void FormProvideServiceStart1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            TextInstruction.LoadFile(Globals.GetPath(PathEnum.Text) + "\\service_step2.rtf");
            timer1.Enabled = false;
        }

        private void pBxStartService_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.StartService;
            data.timework = interval;
            Close();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            if (interval > 1) interval--;
            intervalLabel.Text = interval.ToString() + " мин";
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            if (interval < data.timework) interval++;
            intervalLabel.Text = interval.ToString() + " мин";
        }

        private void LabelNameService1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
