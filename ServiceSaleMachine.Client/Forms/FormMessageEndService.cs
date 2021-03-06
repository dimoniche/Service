﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ServiceSaleMachine.Drivers;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxFinish, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            FTimeWork = data.serv.timeLightUrn;

            timer1.Enabled = true;

            // включаем подсветку урны
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.light2);

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
            // отключаем подсветку урны
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.light2);

            Params.Result = data;

            data.drivers.ReceivedResponse -= reciveResponse;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentWork++;

            if (!fileLoaded)
            {
                TextEndService.LoadFile(Globals.GetPath(PathEnum.Text) + "\\EndService.rtf");
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
