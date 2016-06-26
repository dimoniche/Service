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

namespace ServiceSaleMachine.Client
{
    public partial class FormSettings : Form
    {
        string[] currentPort;

        MachineDrivers drivers;
        Form form;

        public FormSettings(MachineDrivers drivers, Form form)
        {
            InitializeComponent();

            this.drivers = drivers;
            this.form = form;

            drivers.ReceivedResponse += reciveResponse;

            currentPort = SerialPortHelper.GetSerialPorts();

            comboBox1.Items.Clear();
            comboBox1.Items.Add("NULL");
            comboBox1.Items.AddRange(currentPort);

            comboBox3.Items.Clear();
            comboBox3.Items.Add("NULL");
            comboBox3.Items.AddRange(currentPort);

            comboBox2.Items.Clear();
            comboBox2.Items.Add("NULL");
            comboBox2.Items.AddRange(currentPort);

            buttonStartScanerPoll.Enabled = !drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled = drivers.ScanerIsWork();

            button6.Enabled = !drivers.BillPollIsWork();
            button7.Enabled = drivers.BillPollIsWork();

            if (drivers.scaner.getNumberComPort().Contains("NULL"))
            {
                comboBox1.SelectedIndex = -1;
            }
            else if (drivers.scaner.getNumberComPort().Contains("COM"))
            {
                string index = drivers.scaner.getNumberComPort().Remove(0, 3);
                int int_index = 0;
                int.TryParse(index, out int_index);

                int counter = 0;
                foreach (object item in comboBox1.Items)
                {
                    if ((string)item == drivers.scaner.getNumberComPort())
                    {
                        break;
                    }
                    counter++;
                }

                comboBox1.SelectedIndex = counter;

                drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
            }

            if (drivers.control.getNumberComPort().Contains("NULL"))
            {
                comboBox2.SelectedIndex = -1;
            }
            else if (drivers.control.getNumberComPort().Contains("COM"))
            {
                string index = drivers.control.getNumberComPort().Remove(0, 3);
                int int_index = 0;
                int.TryParse(index, out int_index);

                int counter = 0;
                foreach (object item in comboBox2.Items)
                {
                    if ((string)item == drivers.control.getNumberComPort())
                    {
                        break;
                    }
                    counter++;
                }

                comboBox2.SelectedIndex = counter;

                drivers.control.openPort((string)comboBox2.Items[comboBox1.SelectedIndex]);
            }

            if (drivers.CCNETDriver.getNumberComPort().Contains("NULL"))
            {
                comboBox3.SelectedIndex = -1;
            }
            else if (drivers.CCNETDriver.getNumberComPort().Contains("COM"))
            {
                string index = drivers.CCNETDriver.getNumberComPort().Remove(0, 3);
                int int_index = 0;
                int.TryParse(index, out int_index);

                int counter = 0;
                foreach (object item in comboBox3.Items)
                {
                    if ((string)item == drivers.CCNETDriver.getNumberComPort())
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
                textBox1.Text = drivers.CCNETDriver.BillAdr.ToString();
            }
            else
            {
                string index = Globals.ClientConfiguration.Settings.adressBill;
                int int_index = 0;
                int.TryParse(index, out int_index);
                textBox1.Text = Globals.ClientConfiguration.Settings.adressBill;

                drivers.CCNETDriver.BillAdr = int_index;
            }

            cbxComPortPrinter.Items.Clear();
            cbxComPortPrinter.Items.AddRange(drivers.printer.findAllPrinter());

            if (drivers.printer.getNamePrinter().Contains("NULL"))
            {
                cbxComPortPrinter.SelectedIndex = -1;
            }
            else
            {
                cbxComPortPrinter.SelectedIndex = drivers.printer.findPrinterIndex(drivers.printer.getNamePrinter());
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

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            drivers.scaner.closePort();
            drivers.CCNETDriver.closePort();
            drivers.printer.ClosePrint();

            // покажем основную форму
            form.Show();
            // вернем обработчик обратно
            drivers.ReceivedResponse += ((MainForm)form).reciveResponse;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!((string)comboBox1.Items[comboBox1.SelectedIndex]).Contains("NULL"))
            {
                drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
            }
            else
            {
                drivers.scaner.closePort();
            }

            drivers.scaner.setNumberComPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            drivers.startScanerPoll();

            buttonStartScanerPoll.Enabled = !drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled = drivers.ScanerIsWork();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            drivers.stopScanerPoll();

            buttonStartScanerPoll.Enabled = !drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled = drivers.ScanerIsWork();
        }

        private void button30_Click(object sender, EventArgs e)
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
                    drivers.CCNETDriver.BillAdr = int_index;
                }
                else
                {
                    drivers.CCNETDriver.BillAdr = 3;
                }
            }

            drivers.CCNETDriver.setNumberComPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
            drivers.CCNETDriver.setAdress(drivers.CCNETDriver.BillAdr.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            drivers.startPollBill();

            button6.Enabled = !drivers.BillPollIsWork();
            button7.Enabled = drivers.BillPollIsWork();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            drivers.stopPollBill();

            button6.Enabled = !drivers.BillPollIsWork();
            button7.Enabled = drivers.BillPollIsWork();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label14.Text = drivers.restartBill();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            label16.Text = drivers.WaitBill();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]).Contains("NULL"))
            {
                drivers.printer.OpenPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
            }
            else
            {
                drivers.printer.ClosePrint();
            }

            drivers.printer.setNamePrinter((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!((string)comboBox2.Items[comboBox2.SelectedIndex]).Contains("NULL"))
            {
                drivers.control.openPort((string)comboBox2.Items[comboBox2.SelectedIndex]);
            }
            else
            {
                drivers.control.closePort();
            }

            drivers.control.setNumberComPort((string)comboBox2.Items[comboBox2.SelectedIndex]);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            drivers.printer.StartPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);

            if (drivers.printer.prn.PrinterIsOpen)
            {
                drivers.printer.PrintHeader();
                drivers.printer.PrintBarCode(textBox2.Text);
                drivers.printer.PrintFooter();
                drivers.printer.EndPrint();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            drivers.printer.StartPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);

            if (drivers.printer.prn.PrinterIsOpen)
            {
                drivers.printer.PrintHeader();
                drivers.printer.PrintBody();
                drivers.printer.PrintFooter();
                drivers.printer.EndPrint();
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            drivers.printer.setNamePrinter((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
        }
    }
}
