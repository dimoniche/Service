using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    public partial class FormRegisterNewUser : MyForm
    {
        FormResultData data;

        public FormRegisterNewUser()
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
                else if (obj.GetType() == typeof(string))
                {
                    NewUserinfo.Text = (string)obj;
                }
            }
        }

        private void FormRegisterNewUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void NewUserinfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            Close();
        }
    }
}
