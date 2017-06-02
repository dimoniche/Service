using AirVitamin.Drivers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static AirVitamin.Drivers.MachineDrivers;
using static System.Windows.Forms.TabControl;

namespace AirVitamin.Client
{
    public partial class FormSettings : MyForm
    {
        FormResultData data;

        bool init = false;

        TabPageCollection tabTemp;

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

                stc.PauseLeave += PauseLeave;
                stc.BeforeLeave += BeforeLeave;
                stc.CaptionServiceLeave += CaptionServiceLeave;
                stc.PriceServiceLeave += PriceServiceLeave;
                stc.TimeServiceLeave += TimeServiceLeave;

                stc.TextBoxPause.Text = serv.timePause.ToString();
                stc.TextBoxBefore.Text = serv.timeBefore.ToString();
                stc.TextBoxPriceService.Text = serv.price.ToString();
                stc.TextBoxCaptionService.Text = serv.caption;
                stc.TextBoxTimeService.Text = serv.timework.ToString();

                tabSettingService.TabPages[tabSettingService.TabPages.Count - 1].Controls.Add(stc);

                stc.DeviceTab.TabPages.Clear();

                if (serv.devs != null)
                {
                    foreach (Device dev in serv.devs)
                    {
                        stc.DeviceTab.TabPages.Add(dev.caption);

                        DeviceTabControl devTab = new DeviceTabControl();
                        devTab.Dock = DockStyle.Fill;
                        devTab.IdDeviceLeave += IdDeviceLeave;
                        devTab.NameDeviceLeave += NameDeviceLeave;

                        devTab.TextBoxIdDevice.Text = dev.id.ToString();
                        devTab.TextBoxNameDevice.Text = dev.caption;

                        stc.DeviceTab.TabPages[stc.DeviceTab.TabPages.Count - 1].Controls.Add(devTab);
                    }
                }
            }

            // загрузим тултипы
            LoadToolTips();

            ReLoad();
        }

        private void BeforeLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxBefore.Text, out stc.timeBefore);

            Globals.ClientConfiguration.Save();
        }

        private void LoadToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 500;
            toolTip1.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxCheckOff,    "Отключение оплаты чеком. Так же выключается сканер штрих кода.");
            toolTip1.SetToolTip(this.cbxOffDataBase, "Отключить базу данных. Не будут работать аккаунты.");
            toolTip1.SetToolTip(this.checkBox1,      "Отключение регистрации пользователей.");
            toolTip1.SetToolTip(this.cBxBillOff,     "Отключение купюроприемника. Приведет к невозможности работы с наличностью.");

            toolTip1.SetToolTip(this.textBoxTimeOut,        "Время через которое включается заставка, либо происходит выход на начальный экран.");
            toolTip1.SetToolTip(this.textBoxMaxTimeService, "Максимальное время оказания услуг, на которое рассчитан ресурс оборудования до его заправки.");
            toolTip1.SetToolTip(this.offControl,            "Отключение устройства для управления услугами (клапанами).");
            toolTip1.SetToolTip(this.cBxoffModem,           "Отключение модема. Не будет возможности оповещений о нештатных ситауциях и восстановления паролей пользователей.");
            toolTip1.SetToolTip(this.groupBox1,             "Возможные номиналы купюр с которыми может работать устройство.");
            toolTip1.SetToolTip(this.checkchangeOn,         "Работа устройства с выдачей сдачи чеком или заносом ее на счет пользвателя, если он зарегистрирован.");
            toolTip1.SetToolTip(this.checkBox3,             "Включение возможности переноса сдачи на счет пользователя.");
            toolTip1.SetToolTip(this.textMaxCountBanknote,  "Максимальное значение банкнот в купюроприемнике после которого работа устройства невозможна и выдается предупреждение об этом.");
            toolTip1.SetToolTip(this.labelCurrNumberCheck,  "Номера чеков содержащих стоимость услуг за наличные.");
            toolTip1.SetToolTip(this.labeldelivery,         "Номера чеков содержащих сдачу в виде штрих кода.");
            toolTip1.SetToolTip(this.labelAccount,          "Номера чеков содержащих отчет о остатках денег на аккаунте пользователя.");
            toolTip1.SetToolTip(this.cBxComPortScaner,      "Порт, к оторому подключен сканер штрих кодов.");
            toolTip1.SetToolTip(this.cBxCommand,            "Комманды управления сканером штрих кодов.");
            toolTip1.SetToolTip(this.cBxComPortBill,        "Порт к котрому подключен купюроприемник.");
            toolTip1.SetToolTip(this.button8,               "Получение серийного номера купюроприемника.");
            toolTip1.SetToolTip(this.richTextBox1,          "Лог действий купюроприемника.");
            toolTip1.SetToolTip(this.butStartPoll,          "Запуск лога купюропримника.");
            toolTip1.SetToolTip(this.butResetBill,          "Перезагрузка купюроприемника.");
            toolTip1.SetToolTip(this.button4,               "Вывод всех аккаунтов пользователей. И выбор аккаунта для просмотра подробностей.");
            toolTip1.SetToolTip(this.button12,              "Вывод истории платежей выбранного пользователя. Необходимо выбрать аккаунт нужного пользователя и нажать эту кнопку.");
            toolTip1.SetToolTip(this.button10,              "Внесение средств на счет выбранного пользователя.");
            toolTip1.SetToolTip(this.button11,              "История поступления и расходования средств с аккаунта выбранного пользователя.");
            toolTip1.SetToolTip(this.cbxAllUser,            "Флаг вывода логов и истории для всех пользователей.");
            toolTip1.SetToolTip(this.butCheck,              "Просмотр всех выданных чеков со сдачей.");
            toolTip1.SetToolTip(this.button13,              "Просмотр логов работы устройства.");

            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
            toolTip1.SetToolTip(this.cbxOffHardware, "Отключение всего оборудования.");
        }

        private void TimeServiceLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxTimeService.Text, out stc.timework);

            Globals.ClientConfiguration.Save();
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

        private void PauseLeave(object sender)
        {
            Service stc = Globals.ClientConfiguration.Settings.services[tabSettingService.SelectedIndex];
            ServiceTabControl currServiceTab = ((ServiceTabControl)tabSettingService.TabPages[tabSettingService.SelectedIndex].Controls[0]);

            int.TryParse(currServiceTab.TextBoxPause.Text, out stc.timePause);

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

            currDeviceTab.IdDeviceLeave += IdDeviceLeave;
            currDeviceTab.NameDeviceLeave += NameDeviceLeave;

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

            currServiceTab.PauseLeave += PauseLeave;
            currServiceTab.CaptionServiceLeave += CaptionServiceLeave;
            currServiceTab.PriceServiceLeave += PriceServiceLeave;

            currServiceTab.TextBoxPause.Text = stc.timePause.ToString();
            currServiceTab.TextBoxBefore.Text = stc.timeBefore.ToString();
            currServiceTab.TextBoxPriceService.Text = stc.price.ToString();
            currServiceTab.TextBoxCaptionService.Text = stc.caption;

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

        /// <summary>
        /// Установка всех доступных ком портов
        /// </summary>
        void setAvailableComPort()
        {
            string[] currentPort = SerialPortHelper.GetSerialPorts();

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

            cBxPrinterPort.Items.Clear();
            cBxPrinterPort.Items.Add("нет");
            cBxPrinterPort.Items.AddRange(currentPort);

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
        }

        /// <summary>
        /// Установка контролов относящихся к управляющему устройству
        /// </summary>
        /// <param name="state"></param>
        void SetStateControl (bool state)
        {
            buttonwriteControlPort.Enabled = state;
            cBxControlSpeed.Enabled = state;
            Open1.Enabled = state;
            Open2.Enabled = state;
            Close1.Enabled = state;
            Close2.Enabled = state;
            LightOn1.Enabled = state;
            LightOn2.Enabled = state;
            LightOff1.Enabled = state;
            LightOff2.Enabled = state;
            pipeOff.Enabled = state;
            PipeOn.Enabled = state;
            butReadStatus.Enabled = state;
        }

        /// <summary>
        /// Установка контролов относящихся к сканеру
        /// </summary>
        /// <param name="state"></param>
        void SetStateScaner(bool state)
        {
            buttonStartScanerPoll.Enabled = state;
            buttonStopScanerPoll.Enabled = state;
            cBxComPortScaner.Enabled = state;
            butWriteComPortScaner.Enabled = state;
        }

        void ReLoad()
        {
            init = true;

            try
            {
                // обработчик событий
                data.drivers.ReceivedResponse += reciveResponse;

                // все ком порты
                setAvailableComPort();
                edtBDname.Text = Globals.DbConfiguration.Database;
                edtDBServer.Text = Globals.DbConfiguration.Server;
                edtDBUserName.Text = Globals.DbConfiguration.UserID;
                edtDBPsw.Text = Globals.DbConfiguration.Password;

                if (Globals.ClientConfiguration.Settings.offHardware != 1)
                {
                    changeStateHardWare(true);

                    if (Globals.ClientConfiguration.Settings.offCheck != 1)
                    {
                        buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
                        buttonStopScanerPoll.Enabled = data.drivers.ScanerIsWork();
                    }

                    if (Globals.ClientConfiguration.Settings.offBill != 1)
                    {
                        butStartPoll.Enabled = !data.drivers.BillPollIsWork();
                        butStopPoll.Enabled = data.drivers.BillPollIsWork();
                    }

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

                        SetStateScaner(true);
                    }
                    else
                    {
                        SetStateScaner(false);
                    }

                    if (Globals.ClientConfiguration.Settings.offControl == 1 && data.drivers.control.getNumberComPort().Contains("нет"))
                    {
                        cBxControlPort.SelectedIndex = 0;

                        SetStateControl(false);
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

                        SetStatePoll(false);
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

                    if (data.drivers.printer.status.getNumberComPort().Contains("COM"))
                    {
                        string index = data.drivers.printer.status.getNumberComPort().Remove(0, 3);
                        int int_index = 0;
                        int.TryParse(index, out int_index);

                        int counter = 0;
                        foreach (object item in cBxPrinterPort.Items)
                        {
                            if ((string)item == data.drivers.printer.status.getNumberComPort())
                            {
                                break;
                            }
                            counter++;
                        }

                        cBxPrinterPort.SelectedIndex = counter;
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

            cbxCheckOff.Enabled = !cbxOffHardware.Checked;
            cBxBillOff.Enabled = !cbxOffHardware.Checked;
            offControl.Enabled = !cbxOffHardware.Checked;
            cBxoffModem.Enabled = !cbxOffHardware.Checked;

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

            if (Globals.ClientConfiguration.Settings.offPrinter == 1)
            {
                checkOffPrinter.Checked = true;
            }
            else
            {
                checkOffPrinter.Checked = false;
            }

            if (Globals.ClientConfiguration.Settings.offVideoSecondScreen == 1)
            {
                cBxVideoSecondScreen.Checked = true;
            }
            else
            {
                cBxVideoSecondScreen.Checked = false;
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

            cBxNoPaperWork.Checked = Globals.ClientConfiguration.Settings.NoPaperWork > 0 ? true : false;

            textBoxTimeOut.Text = Globals.ClientConfiguration.Settings.timeout.ToString();

            textNumberPhone.Text = Globals.ClientConfiguration.Settings.numberTelephoneSMS;
            textSMSTimeEnd.Text = Globals.ClientConfiguration.Settings.SMSMessageTimeEnd;
            textNeedCollect.Text = Globals.ClientConfiguration.Settings.SMSMessageNeedCollect;
            textCollectSMS.Text = Globals.ClientConfiguration.Settings.SMSMessageCollect;

            firmsname.Text = Globals.CheckConfiguration.Settings.firmsname;
            secondfirmsname.Text = Globals.CheckConfiguration.Settings.secondfirmsname;
            PreviouslyName.Text = Globals.CheckConfiguration.Settings.PreviouslyService;

            advert1.Text = Globals.CheckConfiguration.Settings.advert1;
            advert2.Text = Globals.CheckConfiguration.Settings.advert2;
            advert3.Text = Globals.CheckConfiguration.Settings.advert3;
            advert4.Text = Globals.CheckConfiguration.Settings.advert4;

            textBoxMaxTimeService.Text = Globals.ClientConfiguration.Settings.limitServiceTime.ToString();

            DateTime dt = GlobalDb.GlobalBase.GetLastRefreshTime();

            labelTimeLastRefresh.Text = "Время последнего обслуживания: " + dt.ToString();
            labelWorkFromLastRefresh.Text = "Всего проработали со времени последнего обслуживания: " + (GlobalDb.GlobalBase.GetWorkTime(dt) / 60).ToString() + " мин " + (GlobalDb.GlobalBase.GetWorkTime(dt) % 60).ToString() + " сек";

            MoneyStatistic();

            int NumberCheck = GlobalDb.GlobalBase.GetCurrentNumberCheck();
            labelCurrNumberCheck.Text = "Текущий номер фискального чека: " + NumberCheck;

            NumberCheck = GlobalDb.GlobalBase.GetCurrentNumberDeliveryCheck();
            labeldelivery.Text = "Текущий номер чека со сдачей: " + NumberCheck;

            NumberCheck = GlobalDb.GlobalBase.GetCurrentAccountNumberCheck();
            labelAccount.Text = "Текущий номер чека с балансом счета: " + NumberCheck;

            cBxCommand.Items.Clear();
            cBxCommand.Items.Add("Включить");
            cBxCommand.Items.Add("Выключить");
            cBxCommand.Items.Add("Подать питание");
            cBxCommand.Items.Add("Уснуть");
            cBxCommand.Items.Add("Проснуться");

            checkBox3.Enabled = checkchangeOn.Checked;

            if (Globals.ClientConfiguration.Settings.ScreenServerType == 0)
            {
                ButPicture.Checked = true;
                ButGIF.Checked = false;
                ButVideo.Checked = false;
            }
            else if (Globals.ClientConfiguration.Settings.ScreenServerType == 1)
            {
                ButPicture.Checked = false;
                ButGIF.Checked = true;
                ButVideo.Checked = false;
            }
            else if (Globals.ClientConfiguration.Settings.ScreenServerType == 2)
            {
                ButPicture.Checked = false;
                ButGIF.Checked = false;
                ButVideo.Checked = true;
            }

            textMaxCountBanknote.Text = Globals.ClientConfiguration.Settings.MaxCountBankNote.ToString();

            init = false;

            // вставим дату последней инкассации
            dateTimeStatistic.Value = GlobalDb.GlobalBase.GetLastEncashment();

            if (Globals.ClientConfiguration.Settings.offCheck == 1)
            {
                foreach(TabPage tab in tabControl2.TabPages)
                {
                    if(tab.Text == "Сканер")
                    {
                        tabControl2.TabPages.Remove(tab);
                    }
                }
            }

            if (Globals.ClientConfiguration.Settings.offPrinter == 1)
            {
                foreach (TabPage tab in tabControl2.TabPages)
                {
                    if (tab.Text == "Принтер")
                    {
                        tabControl2.TabPages.Remove(tab);
                    }
                }
            }
        }

        void MoneyStatistic()
        {
            // обновим из базы статистические данные
            data.statistic = GlobalDb.GlobalBase.GetMoneyStatistic();

            labelAllMoneySumm.Text = "Сумма денег в кассете " + data.statistic.AllMoneySumm.ToString() + " руб.";
            labelAccountMoneySumm.Text = "Cумма денег на аккаунтах " + data.statistic.AccountMoneySumm.ToString() + " руб.";
            labelBarCodeMoneySumm.Text = "Cумма денег на штрихкод-чеках " + data.statistic.BarCodeMoneySumm.ToString() + " руб.";
            labelServiceMoneySumm.Text = "Oказано услуг на сумму " + data.statistic.ServiceMoneySumm.ToString() + " руб.";
            labelCountBankNote.Text = "Количество принятых банкнот " + data.statistic.CountBankNote.ToString() + " шт.";

            labelAllAmountMoney.Text = "Сумма, внесенная наличными " + GlobalDb.GlobalBase.GetAmountMoney().ToString() + " руб.";
            labelAllAmountService.Text = "Oказано услуг на сумму " + GlobalDb.GlobalBase.GetAmountService().ToString() + " руб.";
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
                    {
                        CheckInfo info = GlobalDb.GlobalBase.GetCheckByStr((string)e.Message.Content);

                        if (info == null)
                        {
                            // нет такого чека
                            return;
                        }

                        labSummCheck.Text = info.Amount.ToString();
                    }
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
                case DeviceEvent.DropCassetteBillAcceptor:
                    // открыли кассету с деньгами
                    buttonEncashment.Enabled = true;
                    break;
            }
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            data.drivers.ReceivedResponse -= reciveResponse;

            Params.Result = data;

            textMaxCountBanknote_Leave(sender,e);
            textBoxTimeOut_Leave(sender, e);
            textNumberPhone_Leave(sender, e);
            textSMSTimeEnd_Leave(sender, e);
            firmsname_Leave(sender, e);
            textNeedCollect_Leave(sender, e);
            textBoxMaxTimeService_Leave(sender, e);

            Globals.ClientConfiguration.Save();

            Globals.DbConfiguration.Database = edtBDname.Text;
            Globals.DbConfiguration.Server = edtDBServer.Text;
            Globals.DbConfiguration.UserID = edtDBUserName.Text;
            Globals.DbConfiguration.Password = edtDBPsw.Text;
            Globals.DbConfiguration.Save();
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
            buttonStopScanerPoll.Enabled = data.drivers.ScanerIsWork();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;

            data.drivers.stopScanerPoll();

            buttonStartScanerPoll.Enabled = !data.drivers.ScanerIsWork();
            buttonStopScanerPoll.Enabled = data.drivers.ScanerIsWork();
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
            label14.Text = data.drivers.CCNETDriver.restartBill();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label16.Text = data.drivers.CCNETDriver.WaitBill();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            if (cbxComPortPrinter.SelectedIndex == -1) return;

            if (!((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]).Contains("нет"))
            {
               data.drivers.printer.OpenPrint((string)cbxComPortPrinter.Items[cbxComPortPrinter.SelectedIndex]);
            }

            data.drivers.printer.ClosePrint();

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
               data.drivers.printer.PrintHeader(1);

               int summ = 0;
               int.TryParse(tBxsumm.Text, out summ);

               data.drivers.printer.PrintBarCode(textBox2.Text,summ);
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
           
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int count = GlobalDb.GlobalBase.GetWorkTime(1, 1, new DateTime(2016, 07, 12));
            MessageBox.Show(count.ToString());

            DateTime dt = GlobalDb.GlobalBase.GetLastRefreshTime(1, 1);
            MessageBox.Show(dt.ToString());
        }

        void SetStatePoll(bool state)
        {
            butStartPoll.Enabled = state;
            butStopPoll.Enabled = state;
            butResetBill.Enabled = state;
            butWaitNoteOn.Enabled = state;
            butWaitNoteOff.Enabled = state;

            button1.Enabled = state;
            button2.Enabled = state;
            button6.Enabled = state;
            button8.Enabled = state;
            button7.Enabled = state;
            button5.Enabled = state;

        }

        void changeStateHardWare(bool state)
        {
            SetStateScaner(state);

            cBxComPortBill.Enabled = state;
            tBxAdress.Enabled = state;
            butWriteComPortBill.Enabled = state;

            cBxControlPort.Enabled = state;

            cbxComPortPrinter.Enabled = state;
            butWriteComPortPrinter.Enabled = state;
            butWriteBarCode.Enabled = state;
            butPrintCheck.Enabled = state;

            SetStateControl(state);

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

            cbxCheckOff.Enabled = !cbxOffHardware.Checked;
            cBxBillOff.Enabled = !cbxOffHardware.Checked;
            offControl.Enabled = !cbxOffHardware.Checked;
            cBxoffModem.Enabled = !cbxOffHardware.Checked;

            Globals.ClientConfiguration.Save();

            if (!init)
            {
                //WorkerWait.Run();
                data.drivers.InitAllDevice();
                ReLoad();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            data.drivers.CCNETDriver.StopWaitBill();
        }

        private void cbxCheckOff_CheckedChanged(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                if (cbxCheckOff.Checked)
                {
                    Globals.ClientConfiguration.Settings.offCheck = 1;

                    SetStateScaner(false);
                }
                else
                {
                    Globals.ClientConfiguration.Settings.offCheck = 0;

                    SetStateScaner(true);
                }

                Globals.ClientConfiguration.Save();

                if (!init)
                {
                    //WorkerWait.Run();
                    data.drivers.InitAllDevice();
                    ReLoad();
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
                GlobalDb.GlobalBase.Connect();
            }

            Globals.ClientConfiguration.Save();
        }

        private void cBxComPortBill_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxComPortBill.SelectedIndex == -1) return;

            if (((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]).Contains("нет"))
            {
                SetStatePoll(false);

                data.drivers.CCNETDriver.closePort();
            }
            else
            {
                if(!data.drivers.CCNETDriver.openPort((string)cBxComPortBill.Items[cBxComPortBill.SelectedIndex]))
                {
                    cBxComPortBill.SelectedIndex = 0;
                    return;
                }

                SetStatePoll(true);
            }
        }

        private void cBxBillOff_CheckedChanged(object sender, EventArgs e)
        {
            if (cBxBillOff.Checked)
            {
                Globals.ClientConfiguration.Settings.offBill = 1;

                SetStatePoll(false);
            }
            else
            {
                Globals.ClientConfiguration.Settings.offBill = 0;

                SetStatePoll(true);
            }

            Globals.ClientConfiguration.Save();
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            label16.Text = data.drivers.CCNETDriver.WaitBillEscrow();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            List<_Cassete> cassets = data.drivers.CCNETDriver.GetStatus();
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
            data.drivers.CCNETDriver.ReturnBill();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            data.drivers.CCNETDriver.PackBill();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware == 1) return;
            Info.Text = data.drivers.CCNETDriver.getInfoBill();
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
            if (init) return;

            Globals.ClientConfiguration.Settings.changeOn = checkchangeOn.Checked ? 1 : 0;
            Globals.ClientConfiguration.Settings.changeToCheck = checkchangeOn.Checked ? 1 : 0;

            checkBox4.Checked = checkchangeOn.Checked;
            checkBox3.Enabled = checkchangeOn.Checked;

            if (checkchangeOn.Checked == false)
            {
                checkBox3.Checked = checkchangeOn.Checked;
            }

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
            if (init) return;

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
                SetStateControl(false);
            }
            else
            {
                Globals.ClientConfiguration.Settings.offControl = 0;

                cBxControlPort.Enabled = true;
                SetStateControl(true);
            }

            Globals.ClientConfiguration.Save();
        }

        private void cBxComPortScaner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxComPortScaner.SelectedIndex == -1) return;

            if (((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]).Contains("нет"))
            {
                SetStateScaner(false);

                data.drivers.scaner.closePort();
            }
            else
            {
                if (!data.drivers.scaner.openPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]))
                {
                    cBxComPortScaner.SelectedIndex = 0;
                    return;
                }

                SetStateScaner(true);
            }

            data.drivers.scaner.setNumberComPort((string)cBxComPortScaner.Items[cBxComPortScaner.SelectedIndex]);
            Globals.ClientConfiguration.Save();
        }

        private void cBxControlPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBxControlPort.SelectedIndex == -1) return;
            if (cBxControlSpeed.SelectedIndex == -1) return;

            if (((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]).Contains("нет"))
            {
                SetStateControl(false);

                data.drivers.control.closePort();
            }
            else
            {
                if (!data.drivers.control.openPort((string)cBxControlPort.Items[cBxControlPort.SelectedIndex]))
                {
                    cBxControlPort.SelectedIndex = 0;
                    return;
                }

                SetStateControl(true);
            }
        }

        private void Open1_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.MixBefore);
        }

        private void Open2_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.MixAfter);
        }

        private void Close1_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.MixBefore);
        }

        private void Close2_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.MixAfter);
        }

        bool login = false;

        private void button4_Click_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = GlobalDb.GlobalBase.GetUsers();

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].HeaderText = "Индекс";
                dataGridView1.Columns[1].HeaderText = "Пользователь";
                dataGridView1.Columns[2].HeaderText = "Пароль";
                dataGridView1.Columns[3].HeaderText = "Тип";
                dataGridView1.Columns[4].HeaderText = "Дата регистрации";
            }

            login = true;

            button12.Enabled = true;
            button10.Enabled = true;
            button11.Enabled = true;
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
            butsendsms.Enabled = false;

            resSMS.Text = "Сообщение отправляется";

            if (data.drivers.modem.SendSMS(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd,data.log))
            {
                resSMS.Text = "Сообщение отправлено";
            }
            else
            {
                resSMS.Text = "Сообщение не отправлено";
            }

            butsendsms.Enabled = true;

            //data.drivers.modem.SendSMSRus(Globals.ClientConfiguration.Settings.SMSMessageTimeEnd);
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            int id = 0;

            try
            {
                id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            }
            catch
            {
                return;
            }
            
            GlobalDb.GlobalBase.AddToAmount(id, Convert.ToInt32(edtMoney.Text));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int id = 0;

            if (cbxAllUser.Checked)
            {
                DataTable dt1 = GlobalDb.GlobalBase.GetAmount();
                if (dt1 != null)
                {
                    dataGridView1.DataSource = dt1;
                }

                return;
            }

            try
            {
                id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            }
            catch
            {
                return;
            }

            dataGridView1.DataSource = GlobalDb.GlobalBase.GetAmount(id);

            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].HeaderText = "Индекс";
                dataGridView1.Columns[1].HeaderText = "Пользователь";
                dataGridView1.Columns[2].HeaderText = "Внесено";
                dataGridView1.Columns[3].HeaderText = "Дата внесения";
            }

            button12.Enabled = false;
            button10.Enabled = false;
            button11.Enabled = false;
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
            int id = 0;

            if (cbxAllUser.Checked)
            {
                DataTable dt1 = GlobalDb.GlobalBase.GetPaymentAllUser();
                if (dt1 != null)
                {
                    dataGridView1.DataSource = dt1;
                }

                return;
            }

            try
            {
                id = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            }
            catch
            {
                id = 0;
            }
            

            DataTable dt = GlobalDb.GlobalBase.GetPaymentFromUser(id);

            if (dt != null)
            {
                dataGridView1.DataSource = dt;

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "Индекс";
                    dataGridView1.Columns[1].HeaderText = "Пользователь";
                    dataGridView1.Columns[2].HeaderText = "Внесено";
                    dataGridView1.Columns[3].HeaderText = "Дата внесения";
                }

                button12.Enabled = false;
                button10.Enabled = false;
                button11.Enabled = false;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            byte[] res;

            res = data.drivers.control.GetStatusControl(data.log);

            if (res != null)
            {
                StatusControl.Text = "Устройство: ";

                if (res[0] > 0)
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
            int NumberCheck = 1;

            GlobalDb.GlobalBase.SetNumberCheck(NumberCheck);

            labelCurrNumberCheck.Text = "Текущий номер фискального чека: " + NumberCheck;
        }

        private void LightOn1_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Garbage);
        }

        private void LightOff1_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Garbage);
        }

        private void LightOn2_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Holder);
        }

        private void LightOff2_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Holder);
        }

        private void textBoxMaxTimeService_Leave(object sender, EventArgs e)
        {
            int time = 1000;

            int.TryParse(textBoxMaxTimeService.Text, out time);

            Globals.ClientConfiguration.Settings.limitServiceTime = time;
            Globals.ClientConfiguration.Save();
        }

        private void buttonResetTimeRefresh_Click(object sender, EventArgs e)
        {
            // обслужили устройство
            GlobalDb.GlobalBase.WriteRefreshTime(1,1);

            DateTime dt = GlobalDb.GlobalBase.GetLastRefreshTime();

            labelTimeLastRefresh.Text = "Время последнего обслуживания: " + dt.ToString();
            labelWorkFromLastRefresh.Text = "Всего проработали со времени последнего обслуживания: " + (GlobalDb.GlobalBase.GetWorkTime(dt)/60).ToString() + " мин " + (GlobalDb.GlobalBase.GetWorkTime(dt) % 60).ToString() + " сек";
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

            // печатаем чек c инкассацией
            if (data.drivers.printer.prn.PrinterIsOpen)
            {
                data.drivers.printer.PrintCheckСollection(data.statistic);
            }

            // запишем инфу о последнем обслуживании и инкасации
            DateTime dt = GlobalDb.GlobalBase.GetLastEncashment();

            int countmoney = 0;
            if (dt != null)
            {
                countmoney = GlobalDb.GlobalBase.GetCountMoney(dt);
            }
            else
            {
                countmoney = GlobalDb.GlobalBase.GetCountMoney(new DateTime(2000, 1, 1));
            }

            // запишем сколько инкассировали и обновим время инкасации
            GlobalDb.GlobalBase.Encashment(data.CurrentUserId, countmoney);

            // очистим накопленные банктноты
            GlobalDb.GlobalBase.ClearBankNotes();

            // обнулим статистику
            data.statistic.AllMoneySumm = 0;
            data.statistic.CountBankNote = 0;
            data.statistic.ServiceMoneySumm = 0;

            // обновляем статистику
            MoneyStatistic();
        }

        private void PreviouslyName_Leave(object sender, EventArgs e)
        {
            Globals.CheckConfiguration.Settings.PreviouslyService = PreviouslyName.Text;
            Globals.CheckConfiguration.Save();
        }

        private void Incasbutton_Click(object sender, EventArgs e)
        {
            data.drivers.printer.StartPrint(data.drivers.printer.getNamePrinter());

            // печатаем чек c инкассацией
            if (data.drivers.printer.prn.PrinterIsOpen)
            {
                data.drivers.printer.PrintCheckСollection(data.statistic);
            }
        }

        private void cBxNoPaperWork_CheckedChanged(object sender, EventArgs e)
        {
            if (cBxNoPaperWork.Checked)
            {
                Globals.ClientConfiguration.Settings.NoPaperWork = 1;
            }
            else
            {
                Globals.ClientConfiguration.Settings.NoPaperWork = 0;
            }

            Globals.ClientConfiguration.Save();
        }

        private void buttonPrinterPort_Click(object sender, EventArgs e)
        {
            data.drivers.printer.status.setNumberComPort((string)cBxPrinterPort.Items[cBxPrinterPort.SelectedIndex]);
        }

        private void cBxPrinterPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            data.drivers.printer.status.setNumberComPort((string)cBxPrinterPort.Items[cBxPrinterPort.SelectedIndex]);
        }

        private void butStatus_Click(object sender, EventArgs e)
        {
            PrinterStatus status = data.drivers.printer.GetStatus();

            if ((status & PrinterStatus.PRINTER_STATUS_PAPER_JAM) > 0)
            {
                labelStatusPaper.Text = "Проблемы с бумагой";
            }
            else if ((status & PrinterStatus.PRINTER_STATUS_PAPER_OUT) > 0)
            {
                labelStatusPaper.Text = "Бумага кончилась";
            }
            else if ((status & PrinterStatus.PRINTER_STATUS_PAPER_PROBLEM) > 0)
            {
                labelStatusPaper.Text = "Проблемы с бумагой";
            }
            else if ((status & PrinterStatus.PRINTER_STATUS_ERROR) > 0)
            {
                labelStatusPaper.Text = "Ошибка принтера";
            }
            else if ((status & PrinterStatus.PRINTER_STATUS_OFFLINE) > 0)
            {
                labelStatusPaper.Text = "Ошибка связи с принтером";
            }
            else if ((status & PrinterStatus.PRINTER_STATUS_DOOR_OPEN) > 0)
            {
                labelStatusPaper.Text = "Открыли дверцу.";
            }
            else if (status > 0)
            {
                labelStatusPaper.Text = "Ошибка принтера: " + status.ToString();
            }
            else
            {
                labelStatusPaper.Text = "Бумага есть";
            }
        }

        private void buttoninsertcheck_Click(object sender, EventArgs e)
        {
            // запомним такой чек
            string check = CheckHelper.GetUniqueNumberCheck(12);
            textBox2.Text = check;

            int summ = 0;

            if (int.TryParse(tBxsumm.Text, out summ) == true)
            {
                GlobalDb.GlobalBase.AddToCheck(data.CurrentUserId, summ, check);

                // распечатем чек
                button16_Click(sender, e);
            }
            else
            {
            }
        }

        private void cBxCommand_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void butSendCode_Click(object sender, EventArgs e)
        {
            data.drivers.scaner.Request((ZebexCommandEnum)cBxCommand.SelectedIndex);
        }

        private void butCheck_Click(object sender, EventArgs e)
        {
            CheckView.DataSource = GlobalDb.GlobalBase.GetChecks();

            if (CheckView.Columns.Count > 0)
            {
                CheckView.Columns[0].HeaderText = "Индекс";
                CheckView.Columns[1].HeaderText = "Создан";
                CheckView.Columns[2].HeaderText = "Использован";
                CheckView.Columns[3].HeaderText = "штрих-код";
                CheckView.Columns[4].HeaderText = "Пользователь";
                CheckView.Columns[5].HeaderText = "Номер чека";
                CheckView.Columns[6].HeaderText = "Статус";
                CheckView.Columns[7].HeaderText = "Сумма";
            }
        }

        private void button13_Click_2(object sender, EventArgs e)
        {
            LogView.DataSource = GlobalDb.GlobalBase.GetLogWork();

            if (LogView.Columns.Count > 0)
            {
                LogView.Columns[0].HeaderText = "Индекс";
                LogView.Columns[1].HeaderText = "№ устройства";
                LogView.Columns[2].HeaderText = "Время работы";
                LogView.Columns[3].HeaderText = "Услуга";
                LogView.Columns[4].HeaderText = "Начало";
            }
        }

        private void cbxAllUser_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxAllUser.Checked)
            {
                button4.Enabled = false;
                button11.Enabled = true;
                button12.Enabled = true;
                button10.Enabled = false;
            }
            else
            {
                button4.Enabled = true;

                button10.Enabled = false;
                button11.Enabled = false;
                button12.Enabled = false;
            }
        }

        private void buttonDelivery_Click(object sender, EventArgs e)
        {
            // сброс чека
            int NumberCheck = 1;

            GlobalDb.GlobalBase.SetNumberDeliveryCheck(NumberCheck);

            labeldelivery.Text = "Текущий номер чека со сдачей: " + NumberCheck;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Сбросим все чеки со сдачей
            if(MessageBox.Show(this, "Вы уверены что хотите сбросить все чеки со сдачей? \n Все данные будут потеряны.", FormManager.AppCaptionName, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // сбрасываем чеки
                GlobalDb.GlobalBase.FixedAllCheck();
            }

            // обновляем статистику
            data.statistic.BarCodeMoneySumm = 0;

            MoneyStatistic();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            button14_Click(sender, e);
        }

        private void dateTimeStatistic_ValueChanged(object sender, EventArgs e)
        {
            data.statistic = GlobalDb.GlobalBase.GetMoneyStatistic(dateTimeStatistic.Value);

            labelAllMoneySumm.Text = "Сумма денег в кассете " + data.statistic.AllMoneySumm.ToString() + " руб.";
            labelAccountMoneySumm.Text = "Cумма денег на аккаунтах " + data.statistic.AccountMoneySumm.ToString() + " руб.";
            labelBarCodeMoneySumm.Text = "Cумма денег на штрихкод-чеках " + data.statistic.BarCodeMoneySumm.ToString() + " руб.";
            labelServiceMoneySumm.Text = "Oказано услуг на сумму " + data.statistic.ServiceMoneySumm.ToString() + " руб.";
            labelCountBankNote.Text = "Количество принятых банкнот " + data.statistic.CountBankNote.ToString() + " шт.";
        }

        private void buttonResetAllAmount_Click(object sender, EventArgs e)
        {
            // сбросим глобальные счетчики
            GlobalDb.GlobalBase.SetAmountMoney(0);
            GlobalDb.GlobalBase.SetAmountService(0);

            MoneyStatistic();
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            // сброс чека
            int NumberCheck = 1;

            GlobalDb.GlobalBase.SetNumberAccountCheck(NumberCheck);

            labelAccount.Text = "Текущий номер чека с балансом счета: " + NumberCheck;
        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            // Сбросим все деньги с аккаунтов
            if (MessageBox.Show(this, "Вы уверены что хотите сбросить все деньги с аккаунтов?", FormManager.AppCaptionName, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // сбрасываем деньги
                GlobalDb.GlobalBase.ClearAllAccount();
            }

            // обновляем статистику
            MoneyStatistic();
        }

        private void ButPicture_CheckedChanged(object sender, EventArgs e)
        {
            if (ButPicture.Checked == true && ButGIF.Checked == false && ButVideo.Checked == false)
            {
                Globals.ClientConfiguration.Settings.ScreenServerType = 0;
            }
            else if (ButPicture.Checked == false && ButGIF.Checked == true && ButVideo.Checked == false)
            {
                Globals.ClientConfiguration.Settings.ScreenServerType = 1;
            }
            else if (ButPicture.Checked == false && ButGIF.Checked == false && ButVideo.Checked == true)
            {
                Globals.ClientConfiguration.Settings.ScreenServerType = 2;
            }

            Globals.CheckConfiguration.Save();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendOpenControl((int)ControlDeviceEnum.Pipe);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            data.drivers.control.SendCloseControl((int)ControlDeviceEnum.Pipe);
        }

        private void checkOffPrinter_CheckedChanged(object sender, EventArgs e)
        {
            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                if (checkOffPrinter.Checked)
                {
                    Globals.ClientConfiguration.Settings.offPrinter = 1;
                }
                else
                {
                    Globals.ClientConfiguration.Settings.offPrinter = 0;
                }

                Globals.ClientConfiguration.Save();
            }
        }

        private void cBxVideoSecondScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (cBxVideoSecondScreen.Checked)
            {
                Globals.ClientConfiguration.Settings.offVideoSecondScreen = 1;
            }
            else
            {
                Globals.ClientConfiguration.Settings.offVideoSecondScreen = 0;
            }
        }

        private void textCollectSMS_Leave(object sender, EventArgs e)
        {
            Globals.ClientConfiguration.Settings.SMSMessageCollect = textCollectSMS.Text;
            Globals.ClientConfiguration.Save();
        }
    }
}
