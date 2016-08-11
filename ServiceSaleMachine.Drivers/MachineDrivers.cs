using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class MachineDrivers
    {
        private SaleThread WorkerScanerDriver { get; set; }
        private SaleThread WorkerBillPollDriver { get; set; }

        // событие необходимости обработки данных сканера
        public static AutoResetEvent ScanerEvent = new AutoResetEvent(false);
        // событие необходимости обработки данных купюроприемника
        public static AutoResetEvent BillAcceptorEvent = new AutoResetEvent(false);

        // Драйвера устройств
        public ZebexScaner scaner;
        public CCRSProtocol CCNETDriver;
        public PrinterESC printer;

        /// <summary>
        /// Драйвер управляющего устройства
        /// </summary>
        public ControlDevice control;


        public Modem modem;

        public delegate void ServiceClientResponseEventHandler(object sender, ServiceClientResponseEventArgs e);

        // событие обновления данных
        public event ServiceClientResponseEventHandler ReceivedResponse;

        // массив номиналов купюр
        public _BillRecord[] bill_record = new _BillRecord[24];

        public bool hold_bill = false;  // удерживать купюру
        bool send_bill_command = false; // отправляем в данный момент команду в приемник купюр

        public bool gettingBill = false;        // грузим купюру
        public bool gettingEscrowBill = false;  // задержали купюру

        // логгер
        Log log;

        // количество драйверов
        static int COUNT_DEVICE = 3;

        /// <summary>
        /// Инициализация драйверов устройств
        /// </summary>
        /// <param name="log"></param>
        public MachineDrivers(Log log)
        {
            this.log = log;

            this.log.Write(LogMessageType.Information, "Старт драйверов. Версия " + Globals.ProductVersion);
        }

        public bool StopAllDevice()
        {
            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                WorkerBillPollDriver.Abort();
                CCNETDriver.closePort();

                if (Globals.ClientConfiguration.Settings.offCheck != 1)
                {
                    WorkerScanerDriver.Abort();
                    scaner.closePort();
                }

                if (Globals.ClientConfiguration.Settings.offControl != 1)
                {
                    control.closePort();
                }

                if (Globals.ClientConfiguration.Settings.offModem != 1)
                {
                    modem.closePort();
                }
            }

            return true;
        }

        public WorkerStateStage InitAllDevice()
        {
            WorkerStateStage res = WorkerStateStage.None;

            if (!CheckSerialPort() && getCountSerialPort() < COUNT_DEVICE)
            {
                this.log.Write(LogMessageType.Error, "COM портов нет. Работа не возможна.");

                sendMessage(DeviceEvent.NoCOMPort);

                return WorkerStateStage.NoCOMPort;
            }

            if (Globals.ClientConfiguration.Settings.offCheck != 1)
            {
                // не платим чеком - не нужен сканер
                if (scaner == null)
                {
                    scaner = new ZebexScaner();
                    WorkerScanerDriver = new SaleThread { ThreadName = "WorkerScanerDriver" };
                    WorkerScanerDriver.Work += WorkerScanerDriver_Work;
                    WorkerScanerDriver.Complete += WorkerScanerDriver_Complete;
                }
            }

            if (CCNETDriver == null)
            {
                CCNETDriver = new CCRSProtocol();
                WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
                WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
                WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;
            }

            if (printer == null)
            {
                printer = new PrinterESC();
            }

            if (control == null)
            {
                control = new ControlDevice();
            }

            if (modem == null)
            {
                modem = new Modem();
            }

            if ((Globals.ClientConfiguration.Settings.offCheck != 1 && scaner.getNumberComPort().Contains("нет"))
            || (Globals.ClientConfiguration.Settings.offBill != 1 && CCNETDriver.getNumberComPort().Contains("нет"))
            || printer.getNamePrinter().Contains("нет"))
            {
                // необходима настройка приложения
                this.log.Write(LogMessageType.Error, "Необходима настройка приложения");

                sendMessage(DeviceEvent.NeedSettingProgram);
                res = WorkerStateStage.NeedSettingProgram;
            }

            // настроим драйвер сканера
            if (Globals.ClientConfiguration.Settings.offCheck != 1 && !scaner.getNumberComPort().Contains("нет"))
            {
                this.log.Write(LogMessageType.Information, "Настройка сканера.");

                // не платим чеком - не нужен сканер
                if (scaner.openPort(scaner.getNumberComPort()))
                {
                    // запустим задачу ожидания сообщений от сканера
                    if (!WorkerScanerDriver.IsWork)
                    {
                        WorkerScanerDriver.Run();
                    }
                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "Сканер не верно настроен. Порт не доступен.");
                    sendMessage(DeviceEvent.NeedSettingProgram);
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Information, "Сканер отключен.");
            }

            this.log.Write(LogMessageType.Information, "Настройка купюроприемникa.");

            if (Globals.ClientConfiguration.Settings.offBill != 1 && !CCNETDriver.getNumberComPort().Contains("нет"))
            {
                try
                {
                    // настроим купюроприемник
                    if (CCNETDriver.openPort(CCNETDriver.getNumberComPort()))
                    {
                        if (restartBill().Contains("СБОЙ"))
                        {
                            // неудача
                            this.log.Write(LogMessageType.Error, "Купюроприемник не работает или не подключен.");
                            sendMessage(DeviceEvent.NeedSettingProgram);
                            res = WorkerStateStage.NeedSettingProgram;
                        }

                        // запустим задачу опроса купюроприемника
                        if (!WorkerBillPollDriver.IsWork)
                            WorkerBillPollDriver.Run();

                        for (int i = 0; i < 24; i++)
                        {
                            bill_record[i] = new _BillRecord();
                        }
                    }
                    else
                    {
                        // неудача
                        this.log.Write(LogMessageType.Error, "Купюроприемник не верно настроен. Порт не доступен.");
                        sendMessage(DeviceEvent.NeedSettingProgram);
                        res = WorkerStateStage.NeedSettingProgram;
                    }
                }
                catch (Exception exp)
                {

                }
            }
            else
            {
                this.log.Write(LogMessageType.Error, "Купюроприемник не настроен.");
            }

            this.log.Write(LogMessageType.Information, "Настройка принтера.");

            if (!printer.getNamePrinter().Contains("нет"))
            {
                // настроим принтер
                if (printer.OpenPrint("Citizen PPU-700"))
                {

                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "Принтер не верно настроен. Порт не доступен.");
                    sendMessage(DeviceEvent.NeedSettingProgram);
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Error, "Принтер не настроен.");
            }

            // настроим управляющее устройство
            this.log.Write(LogMessageType.Information, "Настройка управлящего устройства.");

            if (Globals.ClientConfiguration.Settings.offControl != 1 && !control.getNumberComPort().Contains("нет"))
            {
                if (control.openPort(control.getNumberComPort()))
                {


                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "Управляющее устройство не верно настроено. Порт не доступен.");
                    sendMessage(DeviceEvent.NeedSettingProgram);
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Error, "Управляющее устройство не настроенo.");
            }

            // настроим драйвер модема
            if (Globals.ClientConfiguration.Settings.offModem != 1 && !modem.getNumberComPort().Contains("нет"))
            {
                this.log.Write(LogMessageType.Information, "Настройка модема.");

                if (modem.openPort(modem.getNumberComPort(), modem.getComPortSpeed()))
                {

                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "Модем не верно настроен. Порт не доступен.");
                    sendMessage(DeviceEvent.NeedSettingProgram);
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Information, "Модем отключен.");
            }

            return res;
        }

        public bool bill_recordIsEmpty()
        {
            bool empty = true;

            foreach (_BillRecord record in bill_record)
            {
                if (record.Denomination > 0)
                {
                    empty = false;
                    break;
                }
            }

            return empty;
        }

        public void ManualInitDevice()
        {

            if (Globals.ClientConfiguration.Settings.offCheck != 1)
            {
                // не платим чеком - не нужен сканер
                scaner = new ZebexScaner();
                WorkerScanerDriver = new SaleThread { ThreadName = "WorkerScanerDriver" };
                WorkerScanerDriver.Work += WorkerScanerDriver_Work;
                WorkerScanerDriver.Complete += WorkerScanerDriver_Complete;
            }

            if (Globals.ClientConfiguration.Settings.offBill != 1)
            {
                CCNETDriver = new CCRSProtocol();
                WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
                WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
                WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;
            }

            printer = new PrinterESC();

            if (Globals.ClientConfiguration.Settings.offControl != 1)
            {
                control = new ControlDevice();
            }
        }

        void sendMessage(DeviceEvent devEvent)
        {
            Message message = new Message();

            message.Event = devEvent;
            message.Content = "";

            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
        }

        bool CheckSerialPort()
        {
            if (SerialPortHelper.GetSerialPorts().Length == 0)
            {
                return false;
            }

            return true;
        }

        int getCountSerialPort()
        {
            return SerialPortHelper.GetSerialPorts().Length;
        }

        public void startPollBill()
        {
            if (!WorkerBillPollDriver.IsWork)
            {
                WorkerBillPollDriver.Run();
            }
        }

        public void stopPollBill()
        {
            if (WorkerBillPollDriver.IsWork)
            {
                WorkerBillPollDriver.Abort();

                send_bill_command = true;

                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, (long)0x00, (long)0x00) == true)
                {

                }
                send_bill_command = false;
            }
        }

        public bool ScanerIsWork()
        {
            return WorkerScanerDriver.IsWork;
        }

        public void startScanerPoll()
        {
            if (!WorkerScanerDriver.IsWork)
            {
                WorkerScanerDriver.Run();
            }
        }

        public void stopScanerPoll()
        {
            if (WorkerScanerDriver.IsWork)
            {
                WorkerScanerDriver.Abort();
            }
        }

        public bool BillPollIsWork()
        {
            return WorkerBillPollDriver.IsWork;
        }

        private void WorkerBillPollDriver_Complete(object sender, ThreadCompleteEventArgs e)
        {

        }

        /// <summary>
        /// Запрос поддерживаемых купюр
        /// </summary>
        /// <returns></returns>
        public string GetBillTable()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.GetBillTable, (byte)CCNETDriver.BillAdr, bill_record) == true)
                {
                    foreach (_BillRecord rec in bill_record)
                    {
                        if (rec.Denomination != 0)
                        {
                            result += Encoding.UTF8.GetString(rec.sCountryCode) + rec.Denomination.ToString() + " ";
                        }
                    }
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            return result;
        }

        /// <summary>
        /// Ожидание ввода купюры, забор купюр сразу
        /// </summary>
        /// <returns></returns>
        public string WaitBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            long mask = 0x00;
            int i = 0;

            foreach (int nominal in Globals.ClientConfiguration.Settings.nominals)
            {
                if (nominal > 0)
                {
                    mask |= (((long)1) << i);
                }
                else
                {
                    mask &= ~(((long)1) << i);
                }

                i++;
            }

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, mask, (long)0x00) == true)
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            hold_bill = false;

            return result;
        }

        /// <summary>
        /// Ожидание ввода купюры, держим купюру
        /// </summary>
        /// <returns></returns>
        public string WaitBillEscrow()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            long mask = 0x00;
            int i = 0;

            foreach (int nominal in Globals.ClientConfiguration.Settings.nominals)
            {
                if (nominal > 0)
                {
                    mask |= (((long)1) << i);
                }
                else
                {
                    mask &= ~(((long)1) << i);
                }

                i++;
            }

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, mask, mask) == true)
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            return result;
        }

        /// <summary>
        /// Окончание ожидания ввода купюры
        /// </summary>
        /// <returns></returns>
        public string StopWaitBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, (long)0x00ffffff, (long)0x00) == true)
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, (long)0x00, (long)0x00) == true)
                {

                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            hold_bill = false;

            return result;
        }

        /// <summary>
        /// Перезагрузка приемника
        /// </summary>
        /// <returns></returns>
        public string restartBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Reset, (byte)CCNETDriver.BillAdr) == true)
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            return result;
        }

        /// <summary>
        /// Возврат купюры
        /// </summary>
        /// <returns></returns>
        public string returnBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Pack, (byte)CCNETDriver.BillAdr))
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            hold_bill = false;

            return result;
        }

        /// <summary>
        /// Забрать купюру
        /// </summary>
        /// <returns></returns>
        public string StackBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Pack, (byte)CCNETDriver.BillAdr))
                {
                    result = "ОК";
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            hold_bill = false;

            return result;
        }

        /// <summary>
        /// Идентификатор
        /// </summary>
        /// <returns></returns>
        public string getInfoBill()
        {
            string result = "";
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr) == true)
                {
                    result = Encoding.UTF8.GetString(CCNETDriver.Ident.PartNumber) + " " + Encoding.UTF8.GetString(CCNETDriver.Ident.SN);
                }
                else
                {
                    result = "СБОЙ";
                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }

            return result;
        }

        /// <summary>
        /// Обработчик опроса состояния приемника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerBillPollDriver_Work(object sender, ThreadWorkEventArgs e)
        {
            int stepCount = 0;

            // сначала загрузим массив принимаемых купюр
            while (bill_recordIsEmpty())
            {
                GetBillTable();
            }

            CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr);

            while (!e.Cancel)
            {
                bool hasErrors = false;

                try
                {
                    if (!send_bill_command)
                    {
                        send_bill_command = true;
                        if (hold_bill && (stepCount % 10) == 0)
                        {
                            CCNETDriver.Cmd(CCNETCommandEnum.Hold, (byte)CCNETDriver.BillAdr);
                        }

                        CCNETDriver.Cmd(CCNETCommandEnum.Poll, (byte)CCNETDriver.BillAdr);
                        send_bill_command = false;

                        // все события купюроприемника
                        if (CCNETDriver.PollResults.Z1 != 0 && CCNETDriver.PollResults.Z2 != 0)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptor;
                            message.Content = CCNETDriver.pollStatus();

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }

                        // выемка денег
                        if (CCNETDriver.PollResults.Z1 == 0x42)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.DropCassetteBillAcceptor;
                            message.Content = null;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }

                        // да фига денег - больше не надо
                        if (CCNETDriver.PollResults.Z1 == 0x41)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.DropCassetteFullBillAcceptor;
                            message.Content = null;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }

                        // ошибки приемника
                        if (CCNETDriver.PollResults.Z1 == 0x47)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorError;
                            message.Content = CCNETDriver.PollResults.Z2;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }

                        // задержали купюру
                        if (CCNETDriver.PollResults.Z1 == 0x80)
                        {
                            if (gettingEscrowBill) continue;    // уже удерживаем купюру - событий больше не надо
                            gettingEscrowBill = true;

                            // удерживаем купюру
                            hold_bill = true;

                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorEscrow;

                            BillNominal nominal = new BillNominal();

                            nominal.record = bill_record[CCNETDriver.PollResults.Z2];
                            nominal.nominalNumber = CCNETDriver.PollResults.Z2;
                            nominal.Denomination = bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

                            message.Content = nominal;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }
                        else
                        {
                            gettingEscrowBill = false;
                        }

                        // получили купюру
                        if (CCNETDriver.PollResults.Z1 == 0x81)
                        {
                            if (gettingBill) continue;
                            // получаем купюру
                            gettingBill = true;

                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorCredit;

                            BillNominal nominal = new BillNominal();

                            nominal.record = bill_record[CCNETDriver.PollResults.Z2];
                            nominal.nominalNumber = CCNETDriver.PollResults.Z2;
                            nominal.Denomination = bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

                            message.Content = nominal;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }
                        else
                        {
                            // другие события - купюру загрузили
                            gettingBill = false;
                        }

                        // вернем купюру
                        if (CCNETDriver.PollResults.Z1 == 0x82)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.returnBillAcceptor;
                            message.Content = null;

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                        }
                    }
                }
                catch (Exception exp)
                {
                    hasErrors = true;
                    send_bill_command = false;
                }
                finally
                {
                    if (hasErrors)
                    {

                    }

                    if (!e.Cancel)
                    {
                        // Нормальная работа
                        BillAcceptorEvent.WaitOne(100);

                        // Принудительный запуск сборки мусора, если возможно освободить больше установленного минимума
                        stepCount++;

                        if (stepCount >= 50)
                        {
                            stepCount = 0;

                            if (GC.GetTotalMemory(false) >= Globals.GcMinMemoryBlock)
                            {
                                GC.Collect();
                            }
                        }
                    }
                }
            }
        }

        public void SetComPortScaner(string port)
        {
            scaner.closePort();
            scaner.openPort(port);
        }

        void ScannerProcessResponse()
        {
            byte[] responseDriver = new byte[scaner.InputStream.Length];
            byte[] buffer = scaner.InputStream.GetBuffer();

            Array.Copy(buffer, 0, responseDriver, 0, responseDriver.Length);

            var str = System.Text.Encoding.Default.GetString(responseDriver);

            Message message = new Message();

            message.Event = DeviceEvent.Scaner;
            message.Content = str;

            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
        }

        private void WorkerScanerDriver_Complete(object sender, ThreadCompleteEventArgs e)
        {

        }

        /// <summary>
        /// Обработчик событий от сканера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerScanerDriver_Work(object sender, ThreadWorkEventArgs e)
        {
            int stepCount = 0;

            // сначала ждем первого события
            ScanerEvent.WaitOne();

            while (!e.Cancel)
            {
                bool hasErrors = false;

                try
                {
                    // обработка строк от сканера
                    ScannerProcessResponse();
                }
                catch
                {
                    hasErrors = true;
                }
                finally
                {
                    if (hasErrors)
                    {
                        //logDrivers.Write(LogMessageType.Information, "ScanerDriver : ERROR");
                    }

                    if (!e.Cancel)
                    {
                        // Нормальная работа
                        ScanerEvent.WaitOne();

                        // Принудительный запуск сборки мусора, если возможно освободить больше установленного минимума
                        stepCount++;

                        if (stepCount >= 50)
                        {
                            stepCount = 0;

                            if (GC.GetTotalMemory(false) >= Globals.GcMinMemoryBlock)
                            {
                                GC.Collect();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// получение статуса
        /// </summary>
        /// <returns></returns>
        public List<_Cassete> GetStatus()
        {
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.GetCassetteStatus, (byte)CCNETDriver.BillAdr))
                {

                }
                else
                {

                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }


            return CCNETDriver.Cassetes;
        }

        /// <summary>
        /// Забрать купюру
        /// </summary>
        /// <returns></returns>
        public List<_Cassete> PackBill()
        {
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Pack, (byte)CCNETDriver.BillAdr))
                {

                }
                else
                {

                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }


            return CCNETDriver.Cassetes;
        }

        /// <summary>
        /// Вернуть купюру
        /// </summary>
        /// <returns></returns>
        public List<_Cassete> ReturnBill()
        {
            int count = 0;

            // если занято - ждем - но не более 1сек
            while (send_bill_command == true) { if (count++ == 1000) { break; } Thread.Sleep(1); }

            send_bill_command = true;

            try
            {
                if (CCNETDriver.Cmd(CCNETCommandEnum.Return, (byte)CCNETDriver.BillAdr))
                {

                }
                else
                {

                }

                send_bill_command = false;
            }
            catch
            {
                send_bill_command = false;
            }


            return CCNETDriver.Cassetes;
        }
    }

    public class ServiceClientResponseEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public ServiceClientResponseEventArgs(Message message)
        {
            Message = message;
        }
    }
}
