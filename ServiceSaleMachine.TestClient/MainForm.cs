﻿using ServiceSaleMachine.Drivers;
using System.Windows.Forms;
using static ServiceSaleMachine.Drivers.MachineDrivers;

namespace ServiceSaleMachine.TestClient
{
    public partial class MainForm : Form
    {
        string[] currentPort;

        MachineDrivers drivers;

        public MainForm()
        {
            InitializeComponent();

            currentPort = SerialPortHelper.GetSerialPorts();

            comboBox1.Items.Clear();
            comboBox1.Items.Add("NULL");
            comboBox1.Items.AddRange(currentPort);

            comboBox3.Items.Clear();
            comboBox3.Items.Add("NULL");
            comboBox3.Items.AddRange(currentPort);

            drivers = new MachineDrivers();
            drivers.ReceivedResponse += reciveResponse;

            if (Globals.ClientConfiguration.Settings.comPortScanner.Contains("NULL"))
            {
                comboBox1.SelectedIndex = -1;
            }
            else if(Globals.ClientConfiguration.Settings.comPortScanner.Contains("COM"))
            {
                string index = Globals.ClientConfiguration.Settings.comPortScanner.Remove(0, 3);
                int int_index = 0;
                int.TryParse(index, out int_index);
                comboBox1.SelectedIndex = int_index;

                drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
            }

            if (Globals.ClientConfiguration.Settings.comPortBill.Contains("NULL"))
            {
                comboBox3.SelectedIndex = -1;
            }
            else if (Globals.ClientConfiguration.Settings.comPortBill.Contains("COM"))
            {
                string index = Globals.ClientConfiguration.Settings.comPortBill.Remove(0, 3);
                int int_index = 0;
                int.TryParse(index, out int_index);
                comboBox3.SelectedIndex = int_index;

                drivers.CCNETDriver.openPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
            }
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
            drivers.scaner.Request((ZebexCommandEnum)comboBox2.SelectedIndex);
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (!((string)comboBox1.Items[comboBox1.SelectedIndex]).Contains("NULL"))
            {
                drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
            }
            else
            {
                drivers.scaner.closePort();
            }

            Globals.ClientConfiguration.Settings.comPortScanner = (string)comboBox1.Items[comboBox1.SelectedIndex];
            Globals.ClientConfiguration.Save();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            if (!((string)comboBox3.Items[comboBox3.SelectedIndex]).Contains("NULL"))
            {
                drivers.CCNETDriver.openPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
            }
            else
            {
                drivers.CCNETDriver.closePort();
            }

            Globals.ClientConfiguration.Settings.comPortBill = (string)comboBox3.Items[comboBox3.SelectedIndex];
            Globals.ClientConfiguration.Save();
        }
    }
}
