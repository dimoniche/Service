using System;
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

        /// <summary>
        /// Драйвер сканера
        /// </summary>
        public ZebexScaner scaner;

        /// <summary>
        /// Драйвер купюроприемника
        /// </summary>
        public CCRSProtocol CCNETDriver;

        /// <summary>
        /// Драйвер принтера
        /// </summary>
        public PrinterESC printer;

        /// <summary>
        /// Драйвер управляющего устройства
        /// </summary>
        public ControlDevice control;

        /// <summary>
        /// Драйвер модема
        /// </summary>
        public Modem modem;

        public delegate void ServiceClientResponseEventHandler(object sender, ServiceClientResponseEventArgs e);

        // событие обновления данных
        public event ServiceClientResponseEventHandler ReceivedResponse;

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
                        if (CCNETDriver.restartBill().Contains("СБОЙ"))
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
                            CCNETDriver.bill_record[i] = new _BillRecord();
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

                CCNETDriver.send_bill_command = true;

                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, (long)0x00, (long)0x00) == true)
                {

                }
                CCNETDriver.send_bill_command = false;
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
        /// Обработчик опроса состояния приемника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerBillPollDriver_Work(object sender, ThreadWorkEventArgs e)
        {
            int stepCount = 0;

            // сначала загрузим массив принимаемых купюр
            while (CCNETDriver.bill_recordIsEmpty())
            {
                CCNETDriver.GetBillTable();
            }

            CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr);

            while (!e.Cancel)
            {
                bool hasErrors = false;

                try
                {
                    if (!CCNETDriver.send_bill_command)
                    {
                        CCNETDriver.send_bill_command = true;
                        if (CCNETDriver.hold_bill && (stepCount % 10) == 0)
                        {
                            CCNETDriver.Cmd(CCNETCommandEnum.Hold, (byte)CCNETDriver.BillAdr);
                        }

                        CCNETDriver.Cmd(CCNETCommandEnum.Poll, (byte)CCNETDriver.BillAdr);
                        CCNETDriver.send_bill_command = false;

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
                            CCNETDriver.hold_bill = true;

                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorEscrow;

                            BillNominal nominal = new BillNominal();

                            nominal.record = CCNETDriver.bill_record[CCNETDriver.PollResults.Z2];
                            nominal.nominalNumber = CCNETDriver.PollResults.Z2;
                            nominal.Denomination = CCNETDriver.bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

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

                            nominal.record = CCNETDriver.bill_record[CCNETDriver.PollResults.Z2];
                            nominal.nominalNumber = CCNETDriver.PollResults.Z2;
                            nominal.Denomination = CCNETDriver.bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

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
                    CCNETDriver.send_bill_command = false;
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
