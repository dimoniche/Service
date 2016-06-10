using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServiceSaleMachine
{
    public partial class MainForm : Form
    {
        FormSettings setting = new FormSettings();
        Services services = new Services();

        public MainForm()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            setting.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            services.Show();
        }
    }
}
