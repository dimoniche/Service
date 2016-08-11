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

            tabSettingService.TabPages.Clear();

            foreach (Service serv in Globals.ClientConfiguration.Settings.services)
            {
                tabSettingService.TabPages.Add(new TabPage(serv.caption));

                ServiceTabControl stc = new ServiceTabControl();
                stc.Dock = DockStyle.Fill;
                stc.AddDevice += AddDevice;
                stc.DeleteDevice += DeleteDevice;

                stc.RecognizeLeave += RecognizeLeave;
                stc.LightUrnLeave += LightUrnLeave;
                stc.CaptionServiceLeave += CaptionServiceLeave;
                stc.PriceServiceLeave += PriceServiceLeave;

                stc.TextBoxRecognize.Text = serv.timeRecognize.ToString();
                stc.TextBoxPriceService.Text = serv.price.ToString();
                stc.TextBoxCaptionService.Text = serv.caption;
                stc.TextBoxLightUrn.Text = serv.timeLightUrn.ToString();

                tabSettingService.TabPages[tabSettingService.TabPages.Count - 1].Controls.Add(stc);

                stc.DeviceTab.TabPages.Clear();

                if (serv.devs != null)
                {
                    foreach (Device dev in serv.devs)
                    {
                        stc.DeviceTab.TabPages.Add(dev.caption);

                        DeviceTabControl devTab = new DeviceTabControl();
                        devTab.Dock = DockStyle.Fill;
                        devTab.LimitTimeLeave += LimitTimeLeave;
                        devTab.TimeWorkLeave += TimeWorkLeave;
                        devTab.IdDeviceLeave += IdDeviceLeave;
                        devTab.NameDeviceLeave += NameDeviceLeave;

                        devTab.LimitTime.Text = dev.limitTime.ToString();
                        devTab.TimeWork.Text = dev.timework.ToString();
                        devTab.TextBoxIdDevice.Text = dev.id.ToString();
                        devTab.TextBoxNameDevice.Text = dev.caption;

                        stc.DeviceTab.TabPages[stc.DeviceTab.TabPages.Count - 1].Controls.Add(devTab);
                    }
                }
            }


            ReLoad();
        }

        /// <summary>
        /// Получение всего количества устройств
        /// </summary>
        /// <returns></returns>
        int GetCountAllDevice()
        {
            int count = 2; // 2 устройства всегда есть - это подсветка

            foreach (Service serv in Globals.ClientConfiguration.Settings.services)
            {
                if (serv.devs != null)
                {
                    foreach (Device dev in serv.devs)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void NameDeviceLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab = (DeviceTabControl)currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Controls[0];

            stc.devs[currServiceTab.DeviceTab.SelectedIndex].caption = currDeviceTab.TextBoxNameDevice.Text;

            Globals.ClientConfiguration.Save();

            currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Text = currDeviceTab.TextBoxNameDevice.Text;
        }

        private void IdDeviceLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab = (DeviceTabControl)currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Controls[0];

            int.TryParse(currDeviceTab.TextBoxIdDevice.Text, out stc.devs[currServiceTab.DeviceTab.SelectedIndex].id);

            Globals.ClientConfiguration.Save();
        }

        private void PriceServiceLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxPriceService.Text, out stc.price);

            Globals.ClientConfiguration.Save();
        }

        private void CaptionServiceLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            stc.caption = currServiceTab.TextBoxCaptionService.Text;

            Globals.ClientConfiguration.Save();

            tabSettingService.TabPages[tabSettingService.SelectedIndex].Text = stc.caption;
        }

        private void LightUrnLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxLightUrn.Text, out stc.timeLightUrn);

            Globals.ClientConfiguration.Save();
        }

        private void TimeWorkLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab = (DeviceTabControl)currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Controls[0];

            int.TryParse(currDeviceTab.TimeWork.Text, out stc.devs[currServiceTab.DeviceTab.SelectedIndex].timework);

            Globals.ClientConfiguration.Save();
        }

        private void LimitTimeLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab = (DeviceTabControl)currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Controls[0];

            int.TryParse(currDeviceTab.LimitTime.Text, out stc.devs[currServiceTab.DeviceTab.SelectedIndex].limitTime);

            Globals.ClientConfiguration.Save();
        }

        private void RecognizeLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxRecognize.Text, out stc.timeRecognize);

            Globals.ClientConfiguration.Save();
        }

        private void DeleteDevice(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];

            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab = (DeviceTabControl)currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex].Controls[0];

            stc.devs.RemoveAt(currServiceTab.DeviceTab.SelectedIndex);

            currServiceTab.DeviceTab.TabPages.Remove(currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.SelectedIndex]);

            Globals.ClientConfiguration.Save();
        }

        private void AddDevice(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];

            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);
            DeviceTabControl currDeviceTab;

            Device dev = new Device();
            dev.id = GetCountAllDevice() + 1;
            dev.caption = "Устройство " + dev.id;
            

            if (stc.devs == null) stc.devs = new List<Device>();
            stc.devs.Add(dev);

            currDeviceTab = new DeviceTabControl();

            currServiceTab.DeviceTab.TabPages.Add(dev.caption);

            currDeviceTab.Dock = DockStyle.Fill;

            currDeviceTab.LimitTimeLeave += LimitTimeLeave;
            currDeviceTab.TimeWorkLeave += TimeWorkLeave;
            currDeviceTab.IdDeviceLeave += IdDeviceLeave;
            currDeviceTab.NameDeviceLeave += NameDeviceLeave;

            currDeviceTab.LimitTime.Text = dev.limitTime.ToString();
            currDeviceTab.TimeWork.Text = dev.timework.ToString();
            currDeviceTab.TextBoxIdDevice.Text = dev.id.ToString();
            currDeviceTab.TextBoxNameDevice.Text = dev.caption;

            currServiceTab.DeviceTab.TabPages[currServiceTab.DeviceTab.TabPages.Count - 1].Controls.Add(currDeviceTab);

            Globals.ClientConfiguration.Save();
        }

        private void butaddServ_Click(object sender, EventArgs e)
        {
            Service stc = new Service();

            stc.caption = "Услуга " + (tabSettingService.TabPages.Count + 1);
            stc.id = tabSettingService.TabPages.Count + 1;

            ServiceTabControl currServiceTab = new ServiceTabControl();

            currServiceTab.Dock = DockStyle.Fill;
            currServiceTab.AddDevice += AddDevice;
            currServiceTab.DeleteDevice += DeleteDevice;

            currServiceTab.RecognizeLeave += RecognizeLeave;
            currServiceTab.LightUrnLeave += LightUrnLeave;
            currServiceTab.CaptionServiceLeave += CaptionServiceLeave;
            currServiceTab.PriceServiceLeave += PriceServiceLeave;

            currServiceTab.TextBoxRecognize.Text = stc.timeRecognize.ToString();
            currServiceTab.TextBoxPriceService.Text = stc.price.ToString();
            currServiceTab.TextBoxCaptionService.Text = stc.caption;
            currServiceTab.TextBoxLightUrn.Text = stc.timeLightUrn.ToString();

            currServiceTab.TextBoxRecognize.Text = stc.timeRecognize.ToString();
            currServiceTab.TextBoxPriceService.Text = stc.price.ToString();
            currServiceTab.TextBoxCaptionService.Text = stc.caption;
            currServiceTab.TextBoxLightUrn.Text = stc.timeLightUrn.ToString();

            tabSettingService.TabPages.Add(stc.caption);
            tabSettingService.TabPages[tabSettingService.TabPages.Count - 1].Controls.Add(currServiceTab);

            currServiceTab.DeviceTab.TabPages.Clear();

            Globals.ClientConfiguration.Settings.services.Add(stc);
            Globals.ClientConfiguration.Save();
        }

        private void butDelServ_Click(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.services.RemoveAt(tabSettingService.SelectedIndex);
            tabSettingService.TabPages.Remove(tabSettingService.TabPages[tabSettingService.SelectedIndex]);

            Globals.ClientConfiguration.Save();
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

                cBxControlPort.Items.Clear();
                cBxControlPort.Items.Add("нет");
                cBxControlPort.Items.AddRange(currentPort);

                cBxModemComPort.Items.Clear();
                cBxModemComPort.Items.Add("нет");
                cBxModemComPort.Items.AddRange(currentPort);

                cBxSpeedModem.Items.Clear();
                cBxControlSpeed.Items.Clear();

                // Скорости
                ComPortSpeedEnum[] comPortSpeeds = CommonHelper.GetEnumValues<ComPortSpeedEnum>();
                cBxControlSpeed.Items.Clear();
                foreach (ComPortSpeedEnum speed in comPortSpeeds)
                {
                    cBxControlSpeed.Items.Add((int)speed);
                    cBxSpeedModem.Items.Add((int)speed);
                }


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

                    if (Globals.ClientConfiguration.Settings.offControl == 1 && data.drivers.control.getNumberComPort().Contains("нет"))
                    {
                        cBxControlPort.SelectedIndex = 0;

                        buttonwriteControlPort.Enabled = false;
                        cBxControlSpeed.Enabled = false;
                        Open1.Enabled = false;
                        Open1.Enabled = false;
                        Open1.Enabled = false;
                        Close1.Enabled = false;
                        Close2.Enabled = false;
                        Close3.Enabled = false;
                        Close4.Enabled = false;
                        Open4.Enabled = false;
                        LightOn1.Enabled = false;
                        LightOn2.Enabled = false;
                        LightOff1.Enabled = false;
                        LightOff2.Enabled = false;
                        butReadStatus.Enabled = false;
                    }
                    else if (data.drivers.control.getNumberComPort().Contains("COM"))
                    {
                        string index = data.drivers.control.getNumberComPort().Remove(0, 3);
                        int int_index = 0;
                        int.TryParse(index, out int_index);

                        int counter = 0;
                        foreach (object item in cBxControlPort.Items)
                        {
                            if ((string)item == data.drivers.control.getNumberComPort())
                            {
                                break;
                            }
                            counter++;
                        }

                        cBxControlPort.SelectedIndex = counter;

                        cBxControlSpeed.SelectedItem = (int)data.drivers.control.getComPortSpeed();

                        data.drivers.control.openPort((string)cBxControlPort.Items[cBxControlPort.SelectedIndex], data.drivers.control.getComPortSpeed());
                    }

                    if (Globals.ClientConfiguration.Settings.offBill == 1 || data.drivers.CCNETDriver.getNumberComPort().Contains("нет"))
                    {
                        cBxComPortBill.SelectedIndex = 0;

                        butStartPoll.Enabled = false;
                        butStopPoll.Enabled = false;
                        butResetBill.Enabled = false;
                        butWaitNoteOn.Enabled = false;
                        butWaitNoteOff.Enabled = false;

                        button1.Enabled = false;
                        button2.Enabled = false;
                        button7.Enabled = false;
                        button8.Enabled = false;
                        button6.Enabled = false;
                        button5.Enabled = false;
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

                    if (Globals.ClientConfiguration.Settings.offModem == 1 && data.drivers.modem.getNumberComPort().Contains("нет"))
                    {
                        cBxModemComPort.SelectedIndex = 0;

                        groupBxSettingModem.Enabled = false;
                        butsendsms.Enabled = false;
                    }
                    else if (data.drivers.modem.getNumberComPort().Contains("COM"))
                    {
                        string index = data.drivers.modem.getNumberComPort().Remove(0, 3);
                        int int_index = 0;
                        int.TryParse(index, out int_index);

                        int counter = 0;
                        foreach (object item in cBxControlPort.Items)
                        {
                            if ((string)item == data.drivers.modem.getNumberComPort())
                            {
                                break;
                            }
                            counter++;
                        }

                        cBxModemComPort.SelectedIndex = counter;

                        cBxSpeedModem.SelectedItem = data.drivers.modem.getComPortSpeed();

                        data.drivers.control.openPort((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex], (int)cBxSpeedModem.SelectedItem);
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

            if (Globals.ClientConfiguration.Settings.offControl == 1)
            {
                offControl.Checked = true;
            }
            else
            {
                offControl.Checked = false;
            }

            // номиналы купюр
            checkBox10.Checked = Globals.ClientConfiguration.Settings.nominals[2] > 0 ? true : false;
            checkBox50.Checked = Globals.ClientConfiguration.Settings.nominals[3] > 0 ? true : false;
            checkBox100.Checked = Globals.ClientConfiguration.Settings.nominals[4] > 0 ? true : false;
            checkBox500.Checked = Globals.ClientConfiguration.Settings.nominals[5] > 0 ? true : false;
            checkBox1000.Checked = Globals.ClientConfiguration.Settings.nominals[6] > 0 ? true : false;
            checkBox5000.Checked = Globals.ClientConfiguration.Settings.nominals[7] > 0 ? true : false;

            checkchangeOn.Checked = Globals.ClientConfiguration.Settings.changeOn > 0 ? true : false;

            checkBox3.Checked = Globals.ClientConfiguration.Settings.changeToAccount > 0 ? true : false;
            checkBox4.Checked = Globals.ClientConfiguration.Settings.changeToCheck > 0 ? true : false;

            cBxoffModem.Checked = Globals.ClientConfiguration.Settings.offModem > 0 ? true : false;

            textBoxTimeOut.Text = Globals.ClientConfiguration.Settings.timeout.ToString();

            textNumberPhone.Text = Globals.ClientConfiguration.Settings.numberTelephoneSMS;
            textSMSTimeEnd.Text = Globals.ClientConfiguration.Settings.SMSMessageTimeEnd;

            firmsname.Text = Globals.CheckConfiguration.Settings.firmsname;
            secondfirmsname.Text = Globals.CheckConfiguration.Settings.secondfirmsname;

            advert1.Text = Globals.CheckConfiguration.Settings.advert1;
            advert2.Text = Globals.CheckConfiguration.Settings.advert2;
            advert3.Text = Globals.CheckConfiguration.Settings.advert3;
            advert4.Text = Globals.CheckConfiguration.Settings.advert4;

            richTextStartService.Text = Globals.ClientConfiguration.Settings.TextStartService;
            richTextEndService.Text = Globals.ClientConfiguration.Settings.TextEndService;
        
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
                    label5.Text = ((BillNominal)e.Message.Content).Denomination + " руб";
                    break;
                case DeviceEvent.BillAcceptorEscrow:
                    label5.Text = ((BillNominal)e.Message.Content).Denomination + " руб";
                    break;
            }
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            data.drivers.ReceivedResponse -= reciveResponse;

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

            Globals.ClientConfiguration.Save();
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
            if (cBxControlPort.SelectedIndex == -1) return;
            if (cBxControlSpeed.SelectedIndex == -1) return;

            if (!((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]).Contains("нет"))
            {
               data.drivers.control.openPort((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]);
            }
            else
            {
               data.drivers.control.closePort();
            }

            data.drivers.control.setNumberComPort((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]);
            data.drivers.control.setComPortSpeed((int)cBxControlSpeed.Items[cBxControlSpeed.SelectedIndex]);
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
               data.drivers.printer.PrintBody(Globals.ClientConfiguration.ServiceByIndex(0));
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

            button1.Enabled = state;
            button2.Enabled = state;
            button6.Enabled = state;
            button8.Enabled = state;
            button7.Enabled = state;
            button5.Enabled = state;

            cBxControlPort.Enabled = state;
            buttonwriteControlPort.Enabled = state;
            Open1.Enabled = state;
            Open2.Enabled = state;
            Open3.Enabled = state;
            Close1.Enabled = state;
            Close2.Enabled = state;
            Close3.Enabled = state;
            Close4.Enabled = state;
            Open4.Enabled = state;
            LightOn1.Enabled = state;
            LightOn2.Enabled = state;
            LightOff1.Enabled = state;
            LightOff2.Enabled = state;

            butReadStatus.Enabled = state;

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
                //WorkerWait.Run();
                data.drivers.InitAllDevice();
                ReLoad();
                if (WorkerWait.IsWork) WorkerWait.Abort();
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
                    //WorkerWait.Run();
                    data.drivers.InitAllDevice();
                    ReLoad();
                    if (WorkerWait.IsWork) WorkerWait.Abort();
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
            if (cBxComPortBill.SelectedIndex == -1) return;

            if (((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]).Contains("нет"))
            {
                butStartPoll.Enabled = false;
                butStopPoll.Enabled = false;
                butResetBill.Enabled = false;
                butWaitNoteOn.Enabled = false;
                butWaitNoteOff.Enabled = false;

                button1.Enabled = false;
                button2.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;
                button7.Enabled = false;
                button5.Enabled = false;

                data.drivers.CCNETDriver.closePort();
            }
            else
            {
                if(!data.drivers.CCNETDriver.openPort((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]))
                {
                    cBxComPortBill.SelectedIndex = 0;
                    return;
                }

                butStartPoll.Enabled = true;
                butStopPoll.Enabled = true;
                butResetBill.Enabled = true;
                butWaitNoteOn.Enabled = true;
                butWaitNoteOff.Enabled = true;

                button1.Enabled = true;
                button2.Enabled = true;
                button6.Enabled = true;
                button8.Enabled = true;
                button7.Enabled = true;
                button5.Enabled = true;
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

                button1.Enabled = false;
                button2.Enabled = false;
                button6.Enabled = false;
                button8.Enabled = false;
                button7.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offBill = 0;

                butStartPoll.Enabled = true;
                butStopPoll.Enabled = true;
                butResetBill.Enabled = true;
                butWaitNoteOn.Enabled = true;
                butWaitNoteOff.Enabled = true;

                button1.Enabled = true;
                button2.Enabled = true;
                button6.Enabled = true;
                button8.Enabled = true;
                button7.Enabled = true;
                button5.Enabled = true;
            }

            Globals.ClientConfiguration.Save();
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label16.Text = data.drivers.WaitBillEscrow();
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

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            data.drivers.ReturnBill();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            data.drivers.PackBill();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            Info.Text = data.drivers.getInfoBill();
        }

        private void butWriteNominal_Click(object sender, EventArgs e)
        {
            if (init) return;

            Globals.ClientConfiguration.Settings.nominals[2] = checkBox10.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.nominals[3] = checkBox50.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.nominals[4] = checkBox100.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.nominals[5] = checkBox500.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.nominals[6] = checkBox1000.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.nominals[7] = checkBox5000.Checked ? 1 : 0;

            Globals.ClientConfiguration.Save();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.changeOn = checkchangeOn.Checked ? 1 : 0;
            Globals.ClientConfiguration.Save();
        }

        private void textBoxTimeOut_Leave(object sender, EventArgs e)
        {
           int time = 1;

            int.TryParse(textBoxTimeOut.Text, out time);

            Globals.ClientConfiguration.Settings.timeout = time;
            Globals.ClientConfiguration.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ChangeWrite_Click(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.changeToAccount = checkBox3.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.changeToCheck = checkBox4.Checked ? 1 : 0;

            Globals.ClientConfiguration.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (offControl.Checked)
            {
                Globals.ClientConfiguration.Settings.offControl = 1;

                cBxControlPort.Enabled = false;
                buttonwriteControlPort.Enabled = false;
                cBxControlSpeed.Enabled = false;
                Open1.Enabled = false;
                Open2.Enabled = false;
                Open3.Enabled = false;
                Close1.Enabled = false;
                Close2.Enabled = false;
                Close3.Enabled = false;
                Close4.Enabled = false;
                Open4.Enabled = false;
                LightOn1.Enabled = false;
                LightOn2.Enabled = false;
                LightOff1.Enabled = false;
                LightOff2.Enabled = false;
                butReadStatus.Enabled = false;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offControl = 0;

                cBxControlPort.Enabled = true;
                buttonwriteControlPort.Enabled = true;
                cBxControlSpeed.Enabled = true;
                Open1.Enabled = true;
                Open1.Enabled = true;
                Open1.Enabled = true;
                Close1.Enabled = true;
                Close2.Enabled = true;
                Close3.Enabled = true;
                Close4.Enabled = true;
                Open4.Enabled = true;
                LightOn1.Enabled = true;
                LightOn2.Enabled = true;
                LightOff1.Enabled = true;
                LightOff2.Enabled = true;
                butReadStatus.Enabled = true;
            }

            Globals.ClientConfiguration.Save();
        }

        private void cBxComPortScaner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxComPortScaner.SelectedIndex == -1) return;

            if (((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]).Contains("нет"))
            {
                buttonStartScanerPoll.Enabled = false;
                buttonStopScanerPoll.Enabled = false;
                cBxComPortScaner.Enabled = false;
                butWriteComPortScaner.Enabled = false;

                data.drivers.scaner.closePort();
            }
            else
            {
                if (!data.drivers.scaner.openPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]))
                {
                    cBxComPortScaner.SelectedIndex = 0;
                    return;
                }

                buttonStartScanerPoll.Enabled = true;
                buttonStopScanerPoll.Enabled = true;
                cBxComPortScaner.Enabled = true;
                butWriteComPortScaner.Enabled = true;
            }
        }

        private void cBxControlPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxControlPort.SelectedIndex == -1) return;
            if (cBxControlSpeed.SelectedIndex == -1) return;

            if (((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]).Contains("нет"))
            {
                cBxControlSpeed.Enabled = false;
                Open1.Enabled = false;
                Open2.Enabled = false;
                Open3.Enabled = false;
                Close1.Enabled = false;
                Close2.Enabled = false;
                Close3.Enabled = false;
                Close4.Enabled = false;
                Open4.Enabled = false;
                LightOn1.Enabled = false;
                LightOn2.Enabled = false;
                LightOff1.Enabled = false;
                LightOff2.Enabled = false;
                butReadStatus.Enabled = false;

                data.drivers.control.closePort();
            }
            else
            {
                if (!data.drivers.control.openPort((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]))
                {
                    cBxControlPort.SelectedIndex = 0;
                    return;
                }

                buttonwriteControlPort.Enabled = true;
                cBxControlSpeed.Enabled = true;
                Open1.Enabled = true;
                Open2.Enabled = true;
                Open3.Enabled = true;
                Close1.Enabled = true;
                Close2.Enabled = true;
                Close3.Enabled = true;
                Close4.Enabled = true;
                Open4.Enabled = true;
                LightOn1.Enabled = true;
                LightOn2.Enabled = true;
                LightOff1.Enabled = true;
                LightOff2.Enabled = true;
                butReadStatus.Enabled = true;
            }
        }

        private void Open1_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.dev3);
        }

        private void Open2_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.dev4);
        }

        private void Open3_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.dev5);
        }

        private void Close1_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.dev3);
        }

        private void Close2_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.dev4);
        }

        private void Close3_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.dev5);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = mydb.GetUsers();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cBxModemComPort.SelectedIndex == -1) return;
            if (cBxSpeedModem.SelectedIndex == -1) return;

            if (!((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex]).Contains("нет"))
            {
                data.drivers.modem.openPort((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex], (int)cBxSpeedModem.Items[cBxSpeedModem.SelectedIndex]);
            }
            else
            {
                data.drivers.modem.closePort();
            }

            data.drivers.modem.setNumberComPort((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex]);
            data.drivers.modem.setComPortSpeed((int)cBxSpeedModem.Items[cBxSpeedModem.SelectedIndex]);
        }

        private void cBxoffModem_CheckedChanged(object sender, EventArgs e)
        {
            if (cBxoffModem.Checked)
            {
                Globals.ClientConfiguration.Settings.offModem = 1;

                cBxModemComPort.Enabled = false;
                butWriteModemComPort.Enabled = false;
                groupBxSettingModem.Enabled = false;
                butsendsms.Enabled = false;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offModem = 0;

                cBxModemComPort.Enabled = true;
                butWriteModemComPort.Enabled = true;
                groupBxSettingModem.Enabled = true;
                butsendsms.Enabled = true;
            }

            Globals.ClientConfiguration.Save();
        }

        private void button9_Click_2(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cbxComPortPrinter.SelectedIndex == -1) return;

            data.drivers.printer.StartPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);

            if (data.drivers.printer.prn.PrinterIsOpen)
            {
                data.drivers.printer.PrintBitMapHeader();
                data.drivers.printer.PrintBitMapBody(Globals.ClientConfiguration.ServiceByIndex(0));
                data.drivers.printer.PrintBitMapFooter();

                data.drivers.printer.EndPrint();
            }
        }

        private void cBxSpeedModem_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cBxModemComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxModemComPort.SelectedIndex == -1) return;
            if (cBxSpeedModem.SelectedIndex == -1) return;

            if (((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex]).Contains("нет"))
            {
                data.drivers.modem.closePort();

                groupBxSettingModem.Enabled = false;
                butsendsms.Enabled = false;
            }
            else
            {
                if (!data.drivers.modem.openPort((string)cBxModemComPort.Items[cBxModemComPort.SelectedIndex], (int)cBxSpeedModem.Items[cBxSpeedModem.SelectedIndex]))
                {
                    cBxModemComPort.SelectedIndex = 0;
                    return;
                }

                cBxModemComPort.Enabled = true;
                butWriteModemComPort.Enabled = true;
                groupBxSettingModem.Enabled = true;
                butsendsms.Enabled = true;
            }
        }

        private void textNumberPhone_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.numberTelephoneSMS = textNumberPhone.Text;
            Globals.ClientConfiguration.Save();
        }

        private void textSMSTimeEnd_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.SMSMessageTimeEnd = textSMSTimeEnd.Text;
            Globals.ClientConfiguration.Save();
        }

        private void butsendsms_Click(object sender, EventArgs e)
        {
            resSMS.Text = "Сообщение отправляется";

            if (data.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd))
            {
                resSMS.Text = "Сообщение отправлено";
            }
            else
            {
                resSMS.Text = "Сообщение не отправлено";
            }

            //data.drivers.modem.SendSMSRus(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            mydb.AddToAmount(id, Convert.ToInt32(edtMoney.Text));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = mydb.GetAmount();
        }

        private void firmsname_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.firmsname = firmsname.Text;
            Globals.CheckConfiguration.Save();
        }

        private void secondfirmsname_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.secondfirmsname = secondfirmsname.Text;
            Globals.CheckConfiguration.Save();
        }

        private void advert1_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.advert1 = advert1.Text;
            Globals.CheckConfiguration.Save();
        }

        private void advert2_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.advert2 = advert2.Text;
            Globals.CheckConfiguration.Save();
        }

        private void advert3_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.advert3 = advert3.Text;
            Globals.CheckConfiguration.Save();
        }

        private void advert4_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.advert4 = advert4.Text;
            Globals.CheckConfiguration.Save();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());

            DataTable dt = mydb.GetPaymentFromUser(id);
            if (dt != null)
            { dataGridView1.DataSource = dt; }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte[] res;

            res = data.drivers.GetStatusControl();

            if (res != null)
            {
                StatusControl.Text = "Устройство 1: ";

                if (res[0] > 0)
                {
                    StatusControl.Text += "включено";
                }
                else
                {
                    StatusControl.Text += "отключено";
                }

                StatusControl.Text += " Устройство 2: ";

                if (res[1] > 0)
                {
                    StatusControl.Text += "включено";
                }
                else
                {
                    StatusControl.Text += "отключено";
                }
            }
        }

        private void textMaxCountBanknote_Leave(object sender, EventArgs e)
        {
            int count = 500;

            int.TryParse(textMaxCountBanknote.Text, out count);

            Globals.ClientConfiguration.Settings.MaxCountBankNote = count;
            Globals.ClientConfiguration.Save();
        }

        private void textNeedCollect_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.SMSMessageNeedCollect = textNeedCollect.Text;
            Globals.ClientConfiguration.Save();
        }

        private void resetCheckNumeration_Click(object sender, EventArgs e)
        {
            // сброс чека
            int NumberCheck = 0;

            labelCurrNumberCheck.Text = "Текущий номер чека: " + NumberCheck;
        }

        private void Open4_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.dev6);
        }

        private void Close4_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.dev6);
        }

        private void LightOn1_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.light1);
        }

        private void LightOff1_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.light1);
        }

        private void LightOn2_Click(object sender, EventArgs e)
        {
            data.drivers.SendOpenControl((int)ControlDeviceEnum.light2);
        }

        private void LightOff2_Click(object sender, EventArgs e)
        {
            data.drivers.SendCloseControl((int)ControlDeviceEnum.light2);
        }

        private void richTextStartService_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.TextStartService = richTextStartService.Text;
            Globals.ClientConfiguration.Save();
        }

        private void richTextEndService_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.TextEndService = richTextEndService.Text;
            Globals.ClientConfiguration.Save();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            GlobalDb.GlobalBase.FillSystemValues();
            GlobalDb.GlobalBase.AddToCheck(0, 100, CheckHelper.GetUniqueNumberCheck(10));
        }
    }
}
