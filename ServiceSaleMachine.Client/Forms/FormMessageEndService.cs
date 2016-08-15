using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxFinish, Globals.DesignConfiguration.Settings.ButtonRetToMain);

            FTimeWork = data.serv.timeLightUrn;

            timer1.Enabled = true;

            // включаем подсветку
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.light2);
        }

        private void FormMessageEndService1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // отключаем подсветку
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.light2);

            Params.Result = data;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentWork++;

            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;
                data.stage = WorkerStateStage.EndService;
                this.Close();
            }
        }

        private void pBxFinish_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            data.stage = WorkerStateStage.EndService;
            this.Close();
        }
    }
}
