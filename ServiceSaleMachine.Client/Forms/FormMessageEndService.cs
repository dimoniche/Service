using System.Data;
using System.Linq;

namespace ServiceSaleMachine.Client
{
    public partial class FormMessageEndService : MyForm
    {
        FormResultData data;

        public int CurrentWork;
        public int FTimeWork;

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

            FTimeWork = 30;
            progressBar.Maximum = 30;
            progressBar.Minimum = 0;
            progressBar.Value = 0;

            timer1.Enabled = true;

            // включаем подсветку
            data.drivers.SendOpenControl(3);
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            CurrentWork++;
            progressBar.Value++;
            
            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;

                data.stage = WorkerStateStage.EndService;

                this.Close();
            }
        }

        private void FormMessageEndService_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            // отключаем подсветку
            data.drivers.SendCloseControl(3);

            Params.Result = data;
        }
    }
}
