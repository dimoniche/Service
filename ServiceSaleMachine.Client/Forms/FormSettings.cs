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
    public partial class FormSettings : MyForm
    {
        FormResultData data;

        string[] currentPort;

        // база данных
        public db mydb;

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            try
            {
                data.drivers.ReceivedResponse += reciveResponse;

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

                if (Globals.ClientConfiguration.Settings.offCheck != 1)
                { 
                    buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
                    buttonStopScanerPoll.Enabled = data.drivers.ScanerIsWork();
                }

                button6.Enabled = !data.drivers.BillPollIsWork();
                button7.Enabled = data.drivers.BillPollIsWork();

                if (Globals.ClientConfiguration.Settings.offCheck != 1)
                {
                    // не платим чеком - не нужен сканер
                    if (data.drivers.scaner.getNumberComPort().Contains("NULL"))
                    {
                        comboBox1.SelectedIndex = -1;
                    }
                    else if (data.drivers.scaner.getNumberComPort().Contains("COM"))
                    {
                        string index = data.drivers.scaner.getNumberComPort().Remove(0, 3);
                        int int_index = 0;
                        int.TryParse(index, out int_index);

                        int counter = 0;
                        foreach (object item in comboBox1.Items)
                        {
                            if ((string)item == data.drivers.scaner.getNumberComPort())
                            {
                                break;
                            }
                            counter++;
                        }

                        comboBox1.SelectedIndex = counter;

                        data.drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
                    }
                }

                if (data.drivers.control.getNumberComPort().Contains("NULL"))
                {
                    comboBox2.SelectedIndex = -1;
                }
                else if (data.drivers.control.getNumberComPort().Contains("COM"))
                {
                    string index = data.drivers.control.getNumberComPort().Remove(0, 3);
                    int int_index = 0;
                    int.TryParse(index, out int_index);

                    int counter = 0;
                    foreach (object item in comboBox2.Items)
                    {
                        if ((string)item == data.drivers.control.getNumberComPort())
                        {
                            break;
                        }
                        counter++;
                    }

                    comboBox2.SelectedIndex = counter;

                    data.drivers.control.openPort((string)comboBox2.Items[comboBox1.SelectedIndex]);
                }

                if (data.drivers.CCNETDriver.getNumberComPort().Contains("NULL"))
                {
                    comboBox3.SelectedIndex = -1;
                }
                else if (data.drivers.CCNETDriver.getNumberComPort().Contains("COM"))
                {
                    string index = data.drivers.CCNETDriver.getNumberComPort().Remove(0, 3);
                    int int_index = 0;
                    int.TryParse(index, out int_index);

                    int counter = 0;
                    foreach (object item in comboBox3.Items)
                    {
                        if ((string)item == data.drivers.CCNETDriver.getNumberComPort())
                        {
                            break;
                        }
                        counter++;
                    }

                    comboBox3.SelectedIndex = counter;

                    data.drivers.CCNETDriver.openPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
                }

                if (Globals.ClientConfiguration.Settings.adressBill == null || Globals.ClientConfiguration.Settings.adressBill.Contains("NULL"))
                {
                    data.drivers.CCNETDriver.BillAdr = 3;
                    textBox1.Text = data.drivers.CCNETDriver.BillAdr.ToString();
                }
                else
                {
                    string index = Globals.ClientConfiguration.Settings.adressBill;
                    int int_index = 0;
                    int.TryParse(index, out int_index);
                    textBox1.Text = Globals.ClientConfiguration.Settings.adressBill;

                    data.drivers.CCNETDriver.BillAdr = int_index;
                }

                cbxComPortPrinter.Items.Clear();
                cbxComPortPrinter.Items.AddRange(data.drivers.printer.findAllPrinter());

                if (data.drivers.printer.getNamePrinter().Contains("NULL"))
                {
                    cbxComPortPrinter.SelectedIndex = -1;
                }
                else
                {
                    cbxComPortPrinter.SelectedIndex = data.drivers.printer.findPrinterIndex(data.drivers.printer.getNamePrinter());
                }
            }
            catch
            {

            }

            // база данных
            mydb = new db();

            if (mydb.Connect())
            {

            }
            else
            {

            }

            mydb.CreateTables();

            if(Globals.ClientConfiguration.Settings.offDataBase == 1)
            {
                cbxOffDataBase.Checked = true;
            }
            else
            {
                cbxOffDataBase.Checked = false;
            }

            if (Globals.ClientConfiguration.Settings.offCheck == 1)
            {
                cbxCheckOff.Checked = true;
            }
            else
            {
                cbxCheckOff.Checked = false;
            }

            if (Globals.ClientConfiguration.Settings.offHardware == 1)
            {
                cbxOffHardware.Checked = true;
            }
            else
            {
                cbxOffHardware.Checked = false;
            }

        }

        public FormSettings()
        {
            InitializeComponent();
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
            Params.Result = data;

            try
            {

                if (Globals.ClientConfiguration.Settings.offCheck != 1)
                {
                    // не платим чеком - не нужен сканер
                    data.drivers.scaner.closePort();
                }

                data.drivers.CCNETDriver.closePort();
                data.drivers.printer.ClosePrint();
            }
            catch
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (Globals.ClientConfiguration.Settings.offCheck != 1)
            {
                // не платим чеком - не нужен сканер
                if (!((string)comboBox1.Items[comboBox1.SelectedIndex]).Contains("NULL"))
                {
                    data.drivers.scaner.openPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
                }
                else
                {
                    data.drivers.scaner.closePort();
                }

                data.drivers.scaner.setNumberComPort((string)comboBox1.Items[comboBox1.SelectedIndex]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
           data.drivers.startScanerPoll();

            buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled =data.drivers.ScanerIsWork();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           data.drivers.stopScanerPoll();

            buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled =data.drivers.ScanerIsWork();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (!((string)comboBox3.Items[comboBox3.SelectedIndex]).Contains("NULL"))
            {
               data.drivers.CCNETDriver.openPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
            }
            else
            {
               data.drivers.CCNETDriver.closePort();
            }

            if (textBox1.Text.Contains("NULL"))
            {
               data.drivers.CCNETDriver.BillAdr = 3;
            }
            else
            {
                string index = textBox1.Text;
                int int_index = 0;

                if (int.TryParse(index, out int_index))
                {
                   data.drivers.CCNETDriver.BillAdr = int_index;
                }
                else
                {
                   data.drivers.CCNETDriver.BillAdr = 3;
                }
            }

           data.drivers.CCNETDriver.setNumberComPort((string)comboBox3.Items[comboBox3.SelectedIndex]);
           data.drivers.CCNETDriver.setAdress(data.drivers.CCNETDriver.BillAdr.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
           data.drivers.startPollBill();

            button6.Enabled = !data.drivers.BillPollIsWork();
            button7.Enabled = data.drivers.BillPollIsWork();
        }

        private void button7_Click(object sender, EventArgs e)
        {
           data.drivers.stopPollBill();

            button6.Enabled = !data.drivers.BillPollIsWork();
            button7.Enabled = data.drivers.BillPollIsWork();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label14.Text = data.drivers.restartBill();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            label16.Text = data.drivers.WaitBill();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]).Contains("NULL"))
            {
               data.drivers.printer.OpenPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
            }
            else
            {
               data.drivers.printer.ClosePrint();
            }

           data.drivers.printer.setNamePrinter((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!((string)comboBox2.Items[comboBox2.SelectedIndex]).Contains("NULL"))
            {
               data.drivers.control.openPort((string)comboBox2.Items[comboBox2.SelectedIndex]);
            }
            else
            {
               data.drivers.control.closePort();
            }

           data.drivers.control.setNumberComPort((string)comboBox2.Items[comboBox2.SelectedIndex]);
        }

        private void button16_Click(object sender, EventArgs e)
        {
           data.drivers.printer.StartPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);

            if (data.drivers.printer.prn.PrinterIsOpen)
            {
               data.drivers.printer.PrintHeader();
               data.drivers.printer.PrintBarCode(textBox2.Text);
               data.drivers.printer.PrintFooter();
               data.drivers.printer.EndPrint();
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
           data.drivers.printer.StartPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);

            if (data.drivers.printer.prn.PrinterIsOpen)
            {
               data.drivers.printer.PrintHeader();
               data.drivers.printer.PrintBody();
               data.drivers.printer.PrintFooter();
               data.drivers.printer.EndPrint();
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
           data.drivers.printer.setNamePrinter((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
        }

        private void btnShowDB_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = mydb.GetLogWork();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int count = mydb.GetWorkTime(1, 1, new DateTime(2016, 07, 12));
            MessageBox.Show(count.ToString());

            DateTime dt = mydb.GetLastRefreshTime(1, 1);
            MessageBox.Show(dt.ToString());
        }

        private void cbxOffHardware_CheckStateChanged(object sender, EventArgs e)
        {
            if(cbxOffHardware.Checked) Globals.ClientConfiguration.Settings.offHardware = 1;
            else Globals.ClientConfiguration.Settings.offHardware = 0;

            Globals.ClientConfiguration.Save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            data.drivers.StopWaitBill();
        }

        private void cbxCheckOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxCheckOff.Checked) Globals.ClientConfiguration.Settings.offCheck = 1;
            else Globals.ClientConfiguration.Settings.offCheck = 0;

            Globals.ClientConfiguration.Save();
        }

        private void cbxOffDataBase_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxOffDataBase.Checked) Globals.ClientConfiguration.Settings.offDataBase = 1;
            else Globals.ClientConfiguration.Settings.offDataBase = 0;

            Globals.ClientConfiguration.Save();
        }
    }
}
