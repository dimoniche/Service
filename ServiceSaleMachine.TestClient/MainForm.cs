using ServiceSaleMachine.Drivers;
using System.Text;
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

                int counter = 0;
                foreach(object item in comboBox3.Items)
                {
                    if((string)item == Globals.ClientConfiguration.Settings.comPortBill)
                    {
                        break;
                    }
                    counter++;
                }

                comboBox3.SelectedIndex = counter;

                drivers.CCNETDriver.openPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
            }

            if (Globals.ClientConfiguration.Settings.adressBill == null || Globals.ClientConfiguration.Settings.adressBill.Contains("NULL"))
            {
                drivers.CCNETDriver.BillAdr = 3;
            }
            else
            {
                string index = Globals.ClientConfiguration.Settings.adressBill;
                int int_index = 0;
                int.TryParse(index, out int_index);
                textBox1.Text = Globals.ClientConfiguration.Settings.adressBill;

                drivers.CCNETDriver.BillAdr = int_index;
            }
        }

        private void reciveResponse(object sender, ServiceClientResponseEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new ServiceClientResponseEventHandler(reciveResponse), sender, e);
                return;
            }

            switch (e.Message.Event)
            {
                case DeviceEvent.Scaner:
                    LabelCode.Text = (string)e.Message.Content;
                    break;
                case DeviceEvent.BillAcceptor:
                    richTextBox1.Text = (string)e.Message.Content + "\n" + richTextBox1.Text;
                    break;
                case DeviceEvent.BillAcceptorCredit:
                    label5.Text = (string)e.Message.Content + " руб";
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

            if (textBox1.Text.Contains("NULL"))
            {
                drivers.CCNETDriver.BillAdr = 3;
            }
            else
            {
                string index = textBox1.Text;
                int int_index = 0;

                if (int.TryParse(index, out int_index))
                {
                    Globals.ClientConfiguration.Settings.adressBill = textBox1.Text;
                    drivers.CCNETDriver.BillAdr = int_index;
                }
                else
                {
                    Globals.ClientConfiguration.Settings.adressBill = "3";
                    drivers.CCNETDriver.BillAdr = 3;
                }
            }

            Globals.ClientConfiguration.Settings.comPortBill = (string)comboBox3.Items[comboBox3.SelectedIndex];
            Globals.ClientConfiguration.Settings.adressBill = drivers.CCNETDriver.BillAdr.ToString();
            Globals.ClientConfiguration.Save();
        }

        private void button3_Click_1(object sender, System.EventArgs e)
        {
            label9.Text = drivers.GetBillTable();
        }

        private void button4_Click_1(object sender, System.EventArgs e)
        {
            string result = "ОК";

            if (drivers.CCNETDriver.Cmd(CCNETCommandEnum.Poll, (byte)drivers.CCNETDriver.BillAdr) == true)
            {

            }
            else
            {
                result = "СБОЙ";
            }
        }

        private void button5_Click(object sender, System.EventArgs e)
        {
            drivers.StopWaitBill();
        }

        private void button6_Click(object sender, System.EventArgs e)
        {
            drivers.startPollBill();
        }

        private void button7_Click(object sender, System.EventArgs e)
        {
            drivers.stopPollBill();
        }

        private void button8_Click(object sender, System.EventArgs e)
        {
            string result = "ОК";

            if (drivers.CCNETDriver.Cmd(CCNETCommandEnum.Reset, (byte)drivers.CCNETDriver.BillAdr, (long)0x00ffffff, (long)0x00) == true)
            {

            }
            else
            {
                result = "СБОЙ";
            }

        }

        private void button9_Click(object sender, System.EventArgs e)
        {
            string result = drivers.getInfoBill();

            label15.Text = result;
        }

        private void button10_Click(object sender, System.EventArgs e)
        {
            drivers.WaitBill();
        }

        private void button11_Click(object sender, System.EventArgs e)
        {
            drivers.StackBill();
        }

        private void button12_Click(object sender, System.EventArgs e)
        {
            drivers.returnBill();
        }

        private void button13_Click(object sender, System.EventArgs e)
        {
            drivers.CCNETDriver.Cmd(CCNETCommandEnum.Hold, (byte)drivers.CCNETDriver.BillAdr);
            drivers.hold_bill = true;
        }

        private void button14_Click(object sender, System.EventArgs e)
        {
            drivers.printer.StartPrint();
        }

        private void button15_Click(object sender, System.EventArgs e)
        {
            drivers.printer.EndPrint();
        }

        private void button16_Click(object sender, System.EventArgs e)
        {
            drivers.printer.PrintBarCode(textBox2.Text);
        }
    }
}
