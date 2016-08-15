﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
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

            // включаем подсветку
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.light1);
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
            // Отключаем подстветку
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.light1);
            timer1.Enabled = false;

            Params.Result = data;
        }

        private void pBxStart_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            data.stage = WorkerStateStage.EndService;
            this.Close();
        }
    }
}
