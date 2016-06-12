using Drivers.Zebex;
using ServiceSaleMachine.Drivers;
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

        private void button1_Click(object sender, System.EventArgs e)
        {
            drivers.scaner.Request(comboBox2.SelectedIndex);
        }
    }
}
