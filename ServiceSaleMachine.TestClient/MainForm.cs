using ServiceSaleMachine.Drivers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.TestClient
{
    public partial class MainForm : Form
    {
        MachineDrivers drivers;

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
                    LabelCode.Text = e.Message.Content;
                    break;
            }
        }
    }
}
