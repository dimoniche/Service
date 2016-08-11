using System.Data;
using System.Linq;

namespace ServiceSaleMachine.Client
{
    public partial class FormTemporallyNoWork : MyForm
    {
        FormResultData data;

        public FormTemporallyNoWork()
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
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            // читаем состояние устройства
            byte[] res;
            res = data.drivers.GetStatusControl();

            if (res != null)
            {
                if (res[0] == 0)
                {
                    data.stage = WorkerStateStage.ErrorEndControl;
                    this.Close();
                }
            }
        }

        private void FormTemporallyNoWork_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            Params.Result = data;
        }
    }
}
