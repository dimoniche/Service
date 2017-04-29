﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using AirVitamin.Drivers;
using static AirVitamin.Drivers.MachineDrivers;

namespace AirVitamin.Client
{
    public partial class FormProvideService : MyForm
    {
        FormResultData data;
        int Interval = 60;

        public FormProvideService()
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

                    Interval = data.timework * 60;
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLogo, "Logo_O2.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxProvideServiceText, "Dishy_txt.png");

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxAnimation, "Dishy_txt.png");
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxReturnMainMenu, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            timerService.Enabled = true;

            // оказываем услугу пока так (перепутали устройства - поменял местами)
            if(data.numberCurrentDevice == (int)ControlDeviceEnum.dev3)
            {
                data.drivers.control.SendOpenControl((int)ControlDeviceEnum.dev3);
                data.drivers.control.SendOpenControl((int)ControlDeviceEnum.dev4);
            }
            else
            {
                data.drivers.control.SendOpenControl((int)ControlDeviceEnum.dev3);
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
            }
        }

        private void FormProvideService1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // заканчиваем оказывать услугу (перепутали устройства - поменял местами)
            if (data.numberCurrentDevice == (int)ControlDeviceEnum.dev3)
            {
                data.drivers.control.SendCloseControl((int)ControlDeviceEnum.dev3);
                data.drivers.control.SendCloseControl((int)ControlDeviceEnum.dev4);
            }
            else
            {
                data.drivers.control.SendCloseControl((int)ControlDeviceEnum.dev3);
            }

            Params.Result = data;
            timerService.Enabled = false;

            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void timerService_Tick(object sender, EventArgs e)
        {
            if (Interval-- == 0)
            {
                // услугу оказали полностью
                data.stage = WorkerStateStage.EndService;
                Close();
            }
        }

        private void pBxStopService_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.Fail;
            Close();
        }

        private void ServiceText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }
    }
}
