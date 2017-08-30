using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;
using System.Drawing;

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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle1, "Smes_txt.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStart, Globals.DesignConfiguration.Settings.ButtonStartServices);

            if (data.numberService == NumberServiceEnum.Before)
            {
                // до тренировки
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Do_tren_ver.png");
            }
            else
            {
                // после тренировки
                Globals.DesignConfiguration.Settings.LoadPictureBox(pBxTitle, "Posle_tren_ver.png");
            }

            FTimeWork = data.timeRecognize;
            CurrentWork = 0;

            timer1.Enabled = true;

            // После оплаты открываем все окна (Шланг, Мундштук, Мусор)
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Holder);
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Pipe);
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Garbage);

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
            // закрываем окно Мундштук 
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Holder);

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
