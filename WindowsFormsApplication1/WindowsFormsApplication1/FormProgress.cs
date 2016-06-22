using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class FormProgress : Form
    {
        public int CurrentWork;
        public int FTimeWork;
        public string ServName;
        public FormProgress()
        {
            InitializeComponent();
        }

        public int timework {
            get { return 0; }
            set
            {
                FTimeWork = value;
                progressBar.Maximum = value;
                progressBar.Minimum = 0;
                progressBar.Value = 0;
                timer1.Enabled = true;
                CurrentWork = 0;
            }
        }

        public void Start()
        {
            timer1.Enabled = true;
            this.WindowState = FormWindowState.Maximized;
            this.ShowDialog();
        } 
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentWork++;
            progressBar.Value++;
            label1.Text = String.Format("{0} \nОсталось времени работы {1} секунд.", ServName, FTimeWork - CurrentWork);
            if (CurrentWork >= FTimeWork)
            {
                timer1.Enabled = false;
                this.Close();
            }
        }
    }
}
