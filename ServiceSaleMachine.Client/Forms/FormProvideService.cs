using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

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

                    Interval = data.timework * 60;
                }
            }

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStopService, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            timerService.Enabled = true;

            ServiceText.Text = "Идет оказание услуги. Осталось еще " + (Interval / 60).ToString() + " минуты и " + (Interval % 60).ToString() + " секунд";

            // оказываем услугу
            data.drivers.control.SendOpenControl(data.numberCurrentDevice);
        }

        private void FormProvideService1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // заканчиваем оказывать услугу
            data.drivers.control.SendCloseControl(data.numberCurrentDevice);

            Params.Result = data;
            timerService.Enabled = false;
        }

        private void timerService_Tick(object sender, EventArgs e)
        {
            if (Interval-- == 0)
            {
                // услугу оказали полностью
                data.stage = WorkerStateStage.EndService;
                Close();
            }

            ServiceText.Text = "Идет оказание услуги. Осталось еще " + (Interval / 60).ToString() + " минуты и " + (Interval % 60).ToString() + " секунд";
        }

        private void pBxStopService_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.Fail;
            Close();
        }
    }
}
