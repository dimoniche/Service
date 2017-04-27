using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormProgress : MyForm
    {
        FormResultData data;

        public int CurrentWork;
        public int FTimeWork;
        public string ServName;

        private bool fileLoaded = false;

        public FormProgress()
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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStart, Globals.DesignConfiguration.Settings.ButtonStartServices);

            LabelNameService2.Text = Globals.ClientConfiguration.Settings.services[data.numberService].caption.ToLower();

            FTimeWork = data.timeRecognize;
            CurrentWork = 0;

            timer1.Enabled = true;

            // включаем подсветку расходников
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.light1);

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!fileLoaded)
            {
                TextInstruction.LoadFile(Globals.GetPath(PathEnum.Text) + "\\service_step1.rtf");
                fileLoaded = true;
                timer1.Interval = 1000;
                return;
            }

            CurrentWork++;

            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;

                data.stage = WorkerStateStage.EndService;

                this.Close();
            }
        }

        private void FormProgress1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Отключаем подстветку расходников
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.light1);
            timer1.Enabled = false;

            Params.Result = data;

            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void pBxStart_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            data.stage = WorkerStateStage.EndService;
            this.Close();
        }

        private void LabelNameService1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
