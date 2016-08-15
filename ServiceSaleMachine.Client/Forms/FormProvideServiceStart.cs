using System.Data;
using System.Linq;

namespace ServiceSaleMachine.Client
{
    public partial class FormProvideServiceStart : MyForm
    {
        FormResultData data;
        int interval = 3;

        public FormProvideServiceStart()
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

                    interval = data.timework;
                }
            }

            LabelNameService2.Text = Globals.ClientConfiguration.Settings.services[data.numberService].caption.ToLower();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMainMenu, Globals.DesignConfiguration.Settings.ButtonRetToMain);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStartService, Globals.DesignConfiguration.Settings.ButtonStartServices);

            intervalLabel.Text = interval.ToString() + " мин";

            pBxMinus.Load(Globals.GetPath(PathEnum.Image) + "\\back.png");
            pBxPlus.Load(Globals.GetPath(PathEnum.Image) + "\\forward.png");
        }

        private void FormProvideServiceStart1_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            TextInstruction.LoadFile(Globals.GetPath(PathEnum.Text) + "\\service_step2.rtf");
            timer1.Enabled = false;
        }

        private void pBxMainMenu_Click(object sender, System.EventArgs e)
        {
            // отказались от услуги
            data.stage = WorkerStateStage.Fail;
            this.Close();
        }

        private void pBxStartService_Click(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.StartService;
            data.timework = interval;
            Close();
        }

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            if (interval > 1) interval--;
            intervalLabel.Text = interval.ToString() + " мин";
        }

        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            if (interval < data.timework) interval++;
            intervalLabel.Text = interval.ToString() + " мин";
        }
    }
}
