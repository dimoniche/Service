using ServiceSaleMachine.Drivers;
using System;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.Client
{
    public partial class MainForm : Form
    {
        MachineDrivers drivers;

        FormSettings setting = new FormSettings();
        Services services = new Services();

        public MainForm()
        {
            InitializeComponent();

            drivers = new MachineDrivers();
            drivers.ReceivedResponse += reciveResponse;

        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Recipient)
            {
                case MessageEndPoint.Scaner:
                    
                    break;
                case MessageEndPoint.BillAcceptor:

                    break;
            }
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
