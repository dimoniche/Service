using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormWelcomeUser : MyForm
    {
        FormResultData data;

        public FormWelcomeUser()
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

        private void FormWelcomeUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }
    }
}
