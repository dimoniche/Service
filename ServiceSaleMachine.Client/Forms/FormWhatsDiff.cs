using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormWhatsDiff : MyForm
    {
        FormResultData data;

        public FormWhatsDiff()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxBegin, Globals.DesignConfiguration.Settings.ButtonStartServices);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLabelService1, Globals.DesignConfiguration.Settings.LogoService1);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLabelService2, Globals.DesignConfiguration.Settings.LogoService2);
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
                case DeviceEvent.ConnectBillError:
                    {
                        // нет связи с купюроприемником
                        data.stage = WorkerStateStage.ErrorBill;
                        this.Close();
                    }
                    break;
            }
        }

        private void FormWhatsDiff_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void pBxBegin_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ChoosePay;
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TextService1.LoadFile(Globals.GetPath(PathEnum.Text) + "\\service1.rtf");
            TextService2.LoadFile(Globals.GetPath(PathEnum.Text) + "\\service2.rtf");

            timer1.Enabled = false;
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
