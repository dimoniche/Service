using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormChooseService : MyForm
    {
        FormResultData data;

        int Timeout = 0;

        public FormChooseService()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pictureTitle, "Smes_txt.png");

			if (Globals.DesignConfiguration.Settings.ButtonService1 != "default.png")
			{
				Globals.DesignConfiguration.Settings.LoadPictureBox(pBxService1, Globals.DesignConfiguration.Settings.ButtonService1);
			}
			else
			{
				pBxService1.Visible = false;
			}

			if (Globals.DesignConfiguration.Settings.ButtonService2 != "default.png")
			{
				Globals.DesignConfiguration.Settings.LoadPictureBox(pBxService3, Globals.DesignConfiguration.Settings.ButtonService2);
			}
			else
			{
				pBxService3.Visible = false;
			}

			if (Globals.DesignConfiguration.Settings.ButtonService3 != "default.png")
			{
				Globals.DesignConfiguration.Settings.LoadPictureBox(pBxService2, Globals.DesignConfiguration.Settings.ButtonService3);
			}
			else
			{
				pBxService2.Visible = false;
			}

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxreturntoMain, Globals.DesignConfiguration.Settings.ButtonRetToMain);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxWhatsDiff, Globals.DesignConfiguration.Settings.ButtonWhatsDiff);

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
                data.log.Write(LogMessageType.Debug, "CHOOSE SERVICE: Событие: " + e.Message.Content + ".");
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

        private void TimeOutTimer_Tick(object sender, System.EventArgs e)
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

        private void FormChooseService1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            TimeOutTimer.Enabled = false;
            data.drivers.ReceivedResponse -= reciveResponse;
            Params.Result = data;
        }

        private void FormChooseService1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pBxService1_Click(object sender, System.EventArgs e)
        {
            data.numberService = NumberServiceEnum.Before;
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }

        private void pBxService2_Click(object sender, System.EventArgs e)
        {
            data.numberService = NumberServiceEnum.After;
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }

        private void pBxreturntoMain_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            this.Close();
        }

        private void pBxWhatsDiff_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.WhatsDiff;
            this.Close();
        }

        private void scalableLabel1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void pBxService3_Click(object sender, EventArgs e)
        {
            data.numberService = NumberServiceEnum.Continue;
            data.stage = WorkerStateStage.ChoosePay;
            this.Close();
        }
    }
}
