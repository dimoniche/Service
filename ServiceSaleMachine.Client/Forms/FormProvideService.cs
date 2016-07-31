﻿using System.Data;
using System.Linq;

namespace ServiceSaleMachine.Client
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

                    Interval = data.timework;
                }
            }

            pBxStopService.Load(Globals.GetPath(PathEnum.Image) + "\\fail.png");

            timerService.Enabled = true;
        }

        private void pBxStopService_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.Fail;
            Close();
        }

        private void FormProvideService_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Params.Result = data;
            timerService.Enabled = false;
        }

        private void timerService_Tick(object sender, System.EventArgs e)
        {
            if(Interval-- == 0)
            {
                // услугу оказали полностью
                data.stage = WorkerStateStage.EndService;
                Close();
            }

            ServiceText.Text = "Идет оказание услуги. Осталось еще " + (Interval/60).ToString() + " минуты и " + (Interval % 60).ToString() + " секунд";
        }
    }
}
