using ServiceSaleMachine.Drivers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
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

        bool init = false;

        FormWait wait;
        private SaleThread WorkerWait { get; set; }

        public override void LoadData()
        {
            foreach (object obj in Params.Objects.Where(obj => obj != null))
            {
                if (obj.GetType() == typeof(FormResultData))
                {
                    data = (FormResultData)obj;
                }
            }

            ReLoad();
        }

        void ReLoad()
        {
            init = true;

            try
            {
                data.drivers.ReceivedResponse += reciveResponse;

                currentPort = SerialPortHelper.GetSerialPorts();

                cBxComPortScaner.Items.Clear();
                cBxComPortScaner.Items.Add("нет");
                cBxComPortScaner.Items.AddRange(currentPort);

                cBxComPortBill.Items.Clear();
                cBxComPortBill.Items.Add("нет");
                cBxComPortBill.Items.AddRange(currentPort);

                comboBox2.Items.Clear();
                comboBox2.Items.Add("нет");
                comboBox2.Items.AddRange(currentPort);

                if (Globals.ClientConfiguration.Settings.offHardware != 1)
                {
                    changeStateHardWare(true);

                    if (Globals.ClientConfiguration.Settings.offCheck != 1)
                    {
                        buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
                        buttonStopScanerPoll.Enabled = data.drivers.ScanerIsWork();
                    }

                    butStartPoll.Enabled = !data.drivers.BillPollIsWork();
                    butStopPoll.Enabled = data.drivers.BillPollIsWork();

                    if (Globals.ClientConfiguration.Settings.offCheck != 1)
                    {
                        // не платим чеком - не нужен сканер
                        if (data.drivers.scaner.getNumberComPort().Contains("нет"))
                        {
                            cBxComPortScaner.SelectedIndex = 0;

                            buttonStartScanerPoll.Enabled = false;
                            buttonStopScanerPoll.Enabled = false;
                        }
                        else if (data.drivers.scaner.getNumberComPort().Contains("COM"))
                        {
                            string index = data.drivers.scaner.getNumberComPort().Remove(0, 3);
                            int int_index = 0;
                            int.TryParse(index, out int_index);

                            int counter = 0;
                            foreach (object item in cBxComPortScaner.Items)
                            {
                                if ((string)item == data.drivers.scaner.getNumberComPort())
                                {
                                    break;
                                }
                                counter++;
                            }

                            cBxComPortScaner.SelectedIndex = counter;

                            data.drivers.scaner.openPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]);
                        }

                        buttonStartScanerPoll.Enabled = true;
                        buttonStopScanerPoll.Enabled = true;
                        cBxComPortScaner.Enabled = true;
                        butWriteComPortScaner.Enabled = true;
                    }
                    else
                    {
                        buttonStartScanerPoll.Enabled = false;
                        buttonStopScanerPoll.Enabled = false;
                        cBxComPortScaner.Enabled = false;
                        butWriteComPortScaner.Enabled = false;
                    }

                    if (data.drivers.control.getNumberComPort().Contains("нет"))
                    {
                        comboBox2.SelectedIndex = 0;
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

                        data.drivers.control.openPort((string)comboBox2.Items[cBxComPortScaner.SelectedIndex]);
                    }

                    if (Globals.ClientConfiguration.Settings.offBill == 1 || data.drivers.CCNETDriver.getNumberComPort().Contains("нет"))
                    {
                        cBxComPortBill.SelectedIndex = 0;

                        butStartPoll.Enabled = false;
                        butStopPoll.Enabled = false;
                        butResetBill.Enabled = false;
                        butWaitNoteOn.Enabled = false;
                        butWaitNoteOff.Enabled = false;
                    }
                    else if (data.drivers.CCNETDriver.getNumberComPort().Contains("COM"))
                    {
                        string index = data.drivers.CCNETDriver.getNumberComPort().Remove(0, 3);
                        int int_index = 0;
                        int.TryParse(index, out int_index);

                        int counter = 0;
                        foreach (object item in cBxComPortBill.Items)
                        {
                            if ((string)item == data.drivers.CCNETDriver.getNumberComPort())
                            {
                                break;
                            }
                            counter++;
                        }

                        cBxComPortBill.SelectedIndex = counter;

                        data.drivers.CCNETDriver.openPort((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]);
                    }

                    if (Globals.ClientConfiguration.Settings.adressBill == null || Globals.ClientConfiguration.Settings.adressBill.Contains("NULL"))
                    {
                        data.drivers.CCNETDriver.BillAdr = 3;
                        tBxAdress.Text = data.drivers.CCNETDriver.BillAdr.ToString();
                    }
                    else
                    {
                        string index = Globals.ClientConfiguration.Settings.adressBill;
                        int int_index = 0;
                        int.TryParse(index, out int_index);
                        tBxAdress.Text = Globals.ClientConfiguration.Settings.adressBill;

                        data.drivers.CCNETDriver.BillAdr = int_index;
                    }

                    cbxComPortPrinter.Items.Clear();
                    cbxComPortPrinter.Items.AddRange(data.drivers.printer.findAllPrinter());

                    if (data.drivers.printer.getNamePrinter().Contains("нет"))
                    {
                        cbxComPortPrinter.SelectedIndex = -1;
                    }
                    else
                    {
                        cbxComPortPrinter.SelectedIndex = data.drivers.printer.findPrinterIndex(data.drivers.printer.getNamePrinter());
                    }
                }
                else
                {
                    // все отключено
                    changeStateHardWare(false);
                }
            }
            catch
            {

            }


            if (Globals.ClientConfiguration.Settings.offDataBase != 1)
            {
                // база данных
                mydb = new db();

                if (mydb.Connect())
                {

                }
                else
                {

                }

                mydb.CreateTables();

                tabDataBaseSetting.Enabled = true;
            }
            else
            {
                tabDataBaseSetting.Enabled = false;
            }


            if (Globals.ClientConfiguration.Settings.offDataBase == 1)
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

            if (Globals.ClientConfiguration.Settings.offBill == 1)
            {
                cBxBillOff.Checked = true;
            }
            else
            {
                cBxBillOff.Checked = false;
            }

            init = false;
        }

        public FormSettings()
        {
            InitializeComponent();

            // задача отображения долгих операций
            WorkerWait = new SaleThread { ThreadName = "WorkerWaitSetting" };
            WorkerWait.Work += WorkerWait_Work;
            WorkerWait.Complete += WorkerWait_Complete;
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
                case DeviceEvent.BillAcceptorEscrow:
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
                    //data.drivers.scaner.closePort();
                }

                //data.drivers.CCNETDriver.closePort();
                //data.drivers.printer.ClosePrint();
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
                if (!((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]).Contains("нет"))
                {
                    data.drivers.scaner.openPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]);
                }
                else
                {
                    data.drivers.scaner.closePort();
                }

                data.drivers.scaner.setNumberComPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;

            data.drivers.startScanerPoll();

            buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled =data.drivers.ScanerIsWork();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;

            data.drivers.stopScanerPoll();

            buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled =data.drivers.ScanerIsWork();
        }

        private void button30_Click(object sender, EventArgs e)
        {
            if (!((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]).Contains("нет"))
            {
               data.drivers.CCNETDriver.openPort((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]);
            }
            else
            {
               data.drivers.CCNETDriver.closePort();
            }

            if (tBxAdress.Text.Contains("нет"))
            {
               data.drivers.CCNETDriver.BillAdr = 3;
            }
            else
            {
                string index = tBxAdress.Text;
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

           data.drivers.CCNETDriver.setNumberComPort((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]);
           data.drivers.CCNETDriver.setAdress(data.drivers.CCNETDriver.BillAdr.ToString());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;

            data.drivers.startPollBill();

            butStartPoll.Enabled = !data.drivers.BillPollIsWork();
            butStopPoll.Enabled = data.drivers.BillPollIsWork();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;

            data.drivers.stopPollBill();

            butStartPoll.Enabled = !data.drivers.BillPollIsWork();
            butStopPoll.Enabled = data.drivers.BillPollIsWork();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label14.Text = data.drivers.restartBill();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label16.Text = data.drivers.WaitBill();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cbxComPortPrinter.SelectedIndex == -1) return;

            if (!((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]).Contains("нет"))
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
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (comboBox2.SelectedIndex == -1) return;

            if (!((string)comboBox2.Items[comboBox2.SelectedIndex]).Contains("нет"))
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
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cbxComPortPrinter.SelectedIndex == -1) return;

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
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cbxComPortPrinter.SelectedIndex == -1) return;

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

        void changeStateHardWare(bool state)
        {
            buttonStartScanerPoll.Enabled = state;
            buttonStopScanerPoll.Enabled = state;
            cBxComPortScaner.Enabled = state;
            butWriteComPortScaner.Enabled = state;

            cBxComPortBill.Enabled = state;
            tBxAdress.Enabled = state;
            butWriteComPortBill.Enabled = state;
            butStartPoll.Enabled = state;
            butStopPoll.Enabled = state;
            butResetBill.Enabled = state;
            butWaitNoteOn.Enabled = state;
            butWaitNoteOff.Enabled = state;

            cbxComPortPrinter.Enabled = state;
            butWriteComPortPrinter.Enabled = state;
            butWriteBarCode.Enabled = state;
            butPrintCheck.Enabled = state;

            cbxCheckOff.Enabled = state;
        }

        private void cbxOffHardware_CheckStateChanged(object sender, EventArgs e)
        {
            if (cbxOffHardware.Checked)
            {
                Globals.ClientConfiguration.Settings.offHardware = 1;
                changeStateHardWare(false);
            }
            else
            {
                Globals.ClientConfiguration.Settings.offHardware = 0;
                changeStateHardWare(true);
            }

            Globals.ClientConfiguration.Save();

            if (!init)
            {
                WorkerWait.Run();
                data.drivers.InitAllDevice();
                ReLoad();
                WorkerWait.Abort();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            data.drivers.StopWaitBill();
        }

        private void cbxCheckOff_CheckedChanged(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                if (cbxCheckOff.Checked)
                {
                    Globals.ClientConfiguration.Settings.offCheck = 1;

                    buttonStartScanerPoll.Enabled = false;
                    buttonStopScanerPoll.Enabled = false;
                    cBxComPortScaner.Enabled = false;
                    butWriteComPortScaner.Enabled = false;
                }
                else
                {
                    Globals.ClientConfiguration.Settings.offCheck = 0;

                    buttonStartScanerPoll.Enabled = true;
                    buttonStopScanerPoll.Enabled = true;
                    cBxComPortScaner.Enabled = true;
                    butWriteComPortScaner.Enabled = true;
                }

                Globals.ClientConfiguration.Save();

                if (!init)
                {
                    WorkerWait.Run();
                    data.drivers.InitAllDevice();
                    ReLoad();
                    WorkerWait.Abort();
                }
            }
        }

        private void cbxOffDataBase_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxOffDataBase.Checked)
            {
                Globals.ClientConfiguration.Settings.offDataBase = 1;
                tabDataBaseSetting.Enabled = false;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offDataBase = 0;
                tabDataBaseSetting.Enabled = true;
            }

            Globals.ClientConfiguration.Save();
        }

        private void cBxComPortBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComPortPrinter.SelectedIndex == -1) return;

            if (((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]).Contains("нет"))
            {
                butStartPoll.Enabled = false;
                butStopPoll.Enabled = false;
                butResetBill.Enabled = false;
                butWaitNoteOn.Enabled = false;
                butWaitNoteOff.Enabled = false;
            }
            else
            {
                butStartPoll.Enabled = true;
                butStopPoll.Enabled = true;
                butResetBill.Enabled = true;
                butWaitNoteOn.Enabled = true;
                butWaitNoteOff.Enabled = true;
            }
        }

        private void WorkerWait_Complete(object sender, ThreadCompleteEventArgs e)
        {
            wait.Hide();
        }

        private void WorkerWait_Work(object sender, ThreadWorkEventArgs e)
        {
            wait = new FormWait();
            wait.Show();

            while (!e.Cancel)
            {
                try
                {
                    wait.Refresh();
                }
                catch
                {
                }
                finally
                {
                    if (!e.Cancel)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void cBxBillOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cBxBillOff.Checked)
            {
                Globals.ClientConfiguration.Settings.offBill = 1;

                butStartPoll.Enabled = false;
                butStopPoll.Enabled = false;
                butResetBill.Enabled = false;
                butWaitNoteOn.Enabled = false;
                butWaitNoteOff.Enabled = false;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offBill = 0;

                butStartPoll.Enabled = true;
                butStopPoll.Enabled = true;
                butResetBill.Enabled = true;
                butWaitNoteOn.Enabled = true;
                butWaitNoteOff.Enabled = true;
            }

            Globals.ClientConfiguration.Save();

        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label16.Text = data.drivers.WaitBill();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            List<_Cassete> cassets = data.drivers.GetStatus();
            int position = 0;
            int countmoney = 0;

            foreach(_Cassete casset in cassets)
            {
                if (casset.Status == CCRSProtocol.CS_OK)
                {
                    countmoney = casset.BillNumber;
                    break;
                }
                position++;
            }

            CountBill.Text = countmoney.ToString();
        }
    }
}
