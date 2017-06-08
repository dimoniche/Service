using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormMessageEndService : MyForm
    {
        FormResultData data;

        public int CurrentWork;
        public int FTimeWork;
        private bool fileLoaded = false;

        public FormMessageEndService()
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
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxFinishText, "Spasibo_txt.png");

            FTimeWork = data.serv.timePause;

            timer1.Enabled = true;

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

        private void FormMessageEndService1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // закрываем окна Шланг и Мусор
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Holder);
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Garbage);

            Params.Result = data;

            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentWork++;

            if (!fileLoaded)
            {
                if (data.numberService == 0)
                {
                    TextThanks.LoadFile(Globals.GetPath(PathEnum.Text) + "\\text_thanks1.rtf");
                }
                else
                {
                    TextThanks.LoadFile(Globals.GetPath(PathEnum.Text) + "\\text_thanks2.rtf");
                }

                fileLoaded = true;
                timer1.Interval = 1000;
            }
              
            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;
                data.stage = WorkerStateStage.EndService;
                this.Close();
            }
        }

        private void pBxFinish_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            data.stage = WorkerStateStage.EndService;
            this.Close();
        }

        private void scalableLabel1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
