using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    public partial class FormRules : MyForm
    {
        FormResultData data;
        private int Timeout;

        bool fileLoaded = false;

        public FormRules()
        {
            InitializeComponent();

            timer1.Enabled = true;
            timer1.Interval = 50;
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

            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxMainMenu, Globals.DesignConfiguration.Settings.ButtonRetToMain);
            Globals.DesignConfiguration.Settings.LoadPictureBox(pBxStartService, Globals.DesignConfiguration.Settings.ButtonStartServices);

            InstructionText.LoadFile(Globals.HelpFileName);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Timeout++;

            if (!fileLoaded)
            {
                InstructionText.LoadFile(Globals.HelpFileName);
                fileLoaded = true;
                timer1.Interval = 1000;
            }

            if (Globals.ClientConfiguration.Settings.timeout == 0)
            {
                Timeout = 0;
                return;
            }

            if (Timeout > Globals.ClientConfiguration.Settings.timeout * 60)
            {
                data.stage = WorkerStateStage.TimeOut;
                this.Close();
            }
        }

        private void FormRules1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Params.Result = data;
        }

        private void pBxMainMenu_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.MainScreen;
            this.Close();
        }

        private void pBxStartService_Click(object sender, EventArgs e)
        {
            data.stage = WorkerStateStage.ChooseService;
            this.Close();
        }

        private void FormRules1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt & e.KeyCode == Keys.F4)
            {
                data.stage = WorkerStateStage.ExitProgram;
                Close();
            }
        }
    }
}
