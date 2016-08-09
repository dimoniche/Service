using System.Data;
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

                    Interval = data.timework * 60;
                }
            }

            pBxStopService.Load(Globals.GetPath(PathEnum.Image) + "\\fail.png");

            timerService.Enabled = true;

            ServiceText.Text = "Идет оказание услуги. Осталось еще " + (Interval / 60).ToString() + " минуты и " + (Interval % 60).ToString() + " секунд";
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
            // нарастим время наработки
            

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
