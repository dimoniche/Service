using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormWhatsDiff : MyForm
    {
        FormResultData data;

        public FormWhatsDiff()
        {
            InitializeComponent();

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxBegin, Globals.DesignConfiguration.Settings.ButtonStartServices);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLabelService1, Globals.DesignConfiguration.Settings.LogoService1);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxLabelService2, Globals.DesignConfiguration.Settings.LogoService2);
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

        private void FormWhatsDiff_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pBxBegin_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ChoosePay;
            Close();
        }
    }
}
