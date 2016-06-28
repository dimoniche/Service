using System;
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
        public ControlDevice control;

        public delegate void ServiceClientResponseEventHandler(object sender, ServiceClientResponseEventArgs e);

        // событие обновления данных
        public event ServiceClientResponseEventHandler ReceivedResponse;

        // массив номиналов купюр
        public _BillRecord[] bill_record = new _BillRecord[24];

        public bool hold_bill = false;  // удерживать купюру
        bool send_bill_command = false; // отправляем в данный момент команду в приемник купюр

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

        public bool InitAllDevice()
        {
            bool res = true;

            if (!CheckSerialPort() && getCountSerialPort() < COUNT_DEVICE)
            {
                this.log.Write(LogMessageType.Error, "COM портов нет. Работа не возможна.");

                sendMessage(DeviceEvent.NoCOMPort);
                return false;
            }

            scaner = new ZebexScaner();
            WorkerScanerDriver = new SaleThread { ThreadName = "WorkerScanerDriver" };
            WorkerScanerDriver.Work += WorkerScanerDriver_Work;
            WorkerScanerDriver.Complete += WorkerScanerDriver_Complete;

            CCNETDriver = new CCRSProtocol();
            WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
            WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
            WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;

            printer = new PrinterESC();
            control = new ControlDevice();

            if (scaner.getNumberComPort().Contains("NULL") || CCNETDriver.getNumberComPort().Contains("NULL") || printer.getNamePrinter().Contains("NULL"))
            {
                // необходима настройка приложения
                this.log.Write(LogMessageType.Error, "Необходима настройка приложения");

                sendMessage(DeviceEvent.NeedSettingProgram);
                return false;
            }

            this.log.Write(LogMessageType.Information, "Настройка сканера.");

            // настроим драйвер сканера

            if (scaner.openPort(scaner.getNumberComPort()))
            {
                // запустим задачу ожидания сообщений от сканера
                WorkerScanerDriver.Run();
            }
            else
            {
                // неудача
                this.log.Write(LogMessageType.Error, "Сканер не верно настроен. Порт не доступен.");
                sendMessage(DeviceEvent.NeedSettingProgram);

                return false;
            }

            this.log.Write(LogMessageType.Information, "Настройка купюроприемникa.");

            // настроим купюроприемник
            if (CCNETDriver.openPort(CCNETDriver.getNumberComPort()))
            {
                if(restartBill().Contains("СБОЙ"))
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "Купюроприемник не работает или не подключен.");
                    sendMessage(DeviceEvent.NeedSettingProgram);

                    return false;
                }

                // запустим задачу опроса купюроприемника
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

                return false;
            }

            this.log.Write(LogMessageType.Information, "Настройка принтера.");

            // настроим принтер
            if(printer.OpenPrint("Citizen PPU-700"))
            {
                
            }
            else
            {
                // неудача
                this.log.Write(LogMessageType.Error, "Принтер не верно настроен. Порт не доступен.");
                sendMessage(DeviceEvent.NeedSettingProgram);

                return false;
            }

            // настроим управляющее устройство
            this.log.Write(LogMessageType.Information, "Настройка управлящего устройства.");

            if (control.openPort(control.getNumberComPort()))
            {
                
                
            }
            else
            {
                // неудача
                this.log.Write(LogMessageType.Error, "Управляющее устройство не верно настроен. Порт не доступен.");
                sendMessage(DeviceEvent.NeedSettingProgram);

                return false;
            }

            return res;
        }

        public void ManualInitDevice()
        {
            scaner = new ZebexScaner();
            WorkerScanerDriver = new SaleThread { ThreadName = "WorkerScanerDriver" };
            WorkerScanerDriver.Work += WorkerScanerDriver_Work;
            WorkerScanerDriver.Complete += WorkerScanerDriver_Complete;

            CCNETDriver = new CCRSProtocol();
            WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
            WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
            WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;

            printer = new PrinterESC();
            control = new ControlDevice();
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
            if(SerialPortHelper.GetSerialPorts().Length == 0)
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
        /// Ожидание ввода купюры
        /// </summary>
        /// <returns></returns>
        public string WaitBill()
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

            return result;
        }

        /// <summary>
        /// Забрать купюру
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
            GetBillTable();

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

                        // получили купюру
                        if (CCNETDriver.PollResults.Z1 == 0x81)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorCredit;
                            message.Content = bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

                            ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
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
                catch
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
