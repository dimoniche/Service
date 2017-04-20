using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace AirVitamin.Drivers
{
    public class MachineDrivers
    {
        private SaleThread WorkerBillPollDriver { get; set; }

        /// <summary>
        /// событие необходимости обработки данных купюроприемника
        /// </summary>
        public static AutoResetEvent BillAcceptorEvent = new AutoResetEvent(false);

        // Драйвера устройств

        /// <summary>
        /// Драйвер купюроприемника
        /// </summary>
        public CCRSProtocol CCNETDriver;

        /// <summary>
        /// Драйвер управляющего устройства
        /// </summary>
        public ControlDevice control;

        /// <summary>
        /// Драйвер модема
        /// </summary>
        public Modem modem;

        public delegate void ServiceClientResponseEventHandler(object sender, ServiceClientResponseEventArgs e);

        /// <summary>
        /// событие обновления данных
        /// </summary>
        public event ServiceClientResponseEventHandler ReceivedResponse;

        public bool gettingBill = false;        // грузим купюру
        public bool gettingEscrowBill = false;  // задержали купюру

        /// <summary>
        /// логгер
        /// </summary>
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

            this.log.Write(LogMessageType.Information, "DRIVERS: Старт драйверов. Версия " + Globals.ProductVersion);

            // создаем все объекты драйверов
            if (CCNETDriver == null) { CCNETDriver = new CCRSProtocol(); }
            if (control == null) { control = new ControlDevice(); }
            if (modem == null) { modem = new Modem(); }
        }

        /// <summary>
        /// Останов всех устройств
        /// </summary>
        /// <returns></returns>
        public bool StopAllDevice()
        {
            if (Globals.ClientConfiguration.Settings.offHardware != 1)
            {
                if (WorkerBillPollDriver != null) WorkerBillPollDriver.Abort();
                if (CCNETDriver != null) CCNETDriver.closePort();

                if (Globals.ClientConfiguration.Settings.offControl != 1)
                {
                    if (control != null) control.closePort();
                }

                if (Globals.ClientConfiguration.Settings.offModem != 1)
                {
                    if (modem != null) modem.closePort();
                }
            }

            return true;
        }

        public void InitAllTask()
        {
            if (Globals.ClientConfiguration.Settings.offBill != 1)
            {
                if (WorkerBillPollDriver == null)
                {
                    WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
                    WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
                    WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;
                }
            }
        }

        public WorkerStateStage InitAllDevice()
        {
            WorkerStateStage res = WorkerStateStage.None;

            if (!CheckSerialPort() && getCountSerialPort() < COUNT_DEVICE)
            {
                this.log.Write(LogMessageType.Error, "INIT: COM портов нет. Работа не возможна.");
                return WorkerStateStage.NoCOMPort;
            }

            if (Globals.ClientConfiguration.Settings.offBill != 1 && CCNETDriver.getNumberComPort().Contains("нет"))
            {
                // необходима настройка приложения
                this.log.Write(LogMessageType.Error, "INIT: Необходима настройка приложения");
                res = WorkerStateStage.NeedSettingProgram;
            }

            this.log.Write(LogMessageType.Information, "BILL: Настройка купюроприемникa.");

            if (Globals.ClientConfiguration.Settings.offBill != 1 && !CCNETDriver.getNumberComPort().Contains("нет"))
            {
                try
                {
                    // настроим купюроприемник
                    if (CCNETDriver.openPort(CCNETDriver.getNumberComPort()))
                    {
                        for (int i = 0; i < 24; i++)
                        {
                            CCNETDriver.bill_record[i] = new _BillRecord();
                        }

                        if (CCNETDriver.restartBill().Contains("СБОЙ"))
                        {
                            // неудача
                            this.log.Write(LogMessageType.Error, "BILL: Купюроприемник не работает или не подключен.");
                            res = WorkerStateStage.NeedSettingProgram;
                        }

                        // запустим задачу опроса купюроприемника
                        if (!WorkerBillPollDriver.IsWork)
                        {
                            WorkerBillPollDriver.Run();
                            this.log.Write(LogMessageType.Information, "BILL: Задачу Купюроприемника запустили.");
                        }
                    }
                    else
                    {
                        // неудача
                        this.log.Write(LogMessageType.Error, "BILL: Купюроприемник не верно настроен. Порт не доступен.");
                        res = WorkerStateStage.NeedSettingProgram;
                    }
                }
                catch (Exception exp)
                {
                    log.Write(LogMessageType.Error, "BILL INIT ALL: " + exp.GetDebugInformation());
                }
            }
            else
            {
                this.log.Write(LogMessageType.Error, "BILL: Купюроприемник не настроен.");
            }

            this.log.Write(LogMessageType.Information, "PRINTER: Настройка принтера.");

            // настроим управляющее устройство
            this.log.Write(LogMessageType.Information, "CONTROL: Настройка управлящего устройства.");

            if (Globals.ClientConfiguration.Settings.offControl != 1 && !control.getNumberComPort().Contains("нет"))
            {
                if (control.openPort(control.getNumberComPort()))
                {


                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "CONTROL: Управляющее устройство не верно настроено. Порт не доступен.");
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Error, "CONTROL: Управляющее устройство не настроенo.");
            }

            // настроим драйвер модема
            this.log.Write(LogMessageType.Information, "MODEM: Настройка модема.");

            if (Globals.ClientConfiguration.Settings.offModem != 1 && !modem.getNumberComPort().Contains("нет"))
            {
                this.log.Write(LogMessageType.Information, "MODEM: Настройка модема.");

                if (modem.openPort(modem.getNumberComPort(), modem.getComPortSpeed()))
                {

                }
                else
                {
                    // неудача
                    this.log.Write(LogMessageType.Error, "MODEM: Модем не верно настроен. Порт не доступен.");
                    res = WorkerStateStage.NeedSettingProgram;
                }
            }
            else
            {
                this.log.Write(LogMessageType.Information, "MODEM: Модем отключен.");
            }

            this.log.Write(LogMessageType.Information, "INIT: Оборудование полностью настроили.");

            return res;
        }

        public void ManualInitDevice()
        {
            if (Globals.ClientConfiguration.Settings.offBill != 1)
            {
                CCNETDriver = new CCRSProtocol();
                WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
                WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
                WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;
            }

            if (Globals.ClientConfiguration.Settings.offControl != 1)
            {
                control = new ControlDevice();
            }

            if (Globals.ClientConfiguration.Settings.offModem != 1)
            {
                modem = new Modem();
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
            int timeout = 100;
            bool result = true;
            bool connectOff = false;
            bool errorBill = false;

            this.log.Write(LogMessageType.Information, "BILL TASK: Запускаем задачу приемника.");

            try
            {
                // сначала загрузим массив принимаемых купюр
                while (CCNETDriver.bill_recordIsEmpty())
                {
                    if (CCNETDriver.GetBillTable().Contains("СБОЙ"))
                    {
                        //result = false;
                    }

                    BillAcceptorEvent.WaitOne(500);
                }

                CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr);
            }
            catch (Exception exp)
            {
                this.log.Write(LogMessageType.Error, "BILL TASK: Ошибка: " + exp.GetDebugInformation());
            }

            this.log.Write(LogMessageType.Information, "BILL TASK: Задачу приемника проинициализировали.");

            while (!e.Cancel)
            {
                try
                {
                    //this.log.Write(LogMessageType.Information, "BILL TASK: Шаг обработчика.");
                    if(CCNETDriver.getNumberComPort().Contains("нет"))
                    {
                        // нет ком порта - у не работаем
                    }
                    else if (!CCNETDriver.send_bill_command)
                    {
                        //this.log.Write(LogMessageType.Information, "BILL TASK: Запуск комманды опроса.");

                        CCNETDriver.send_bill_command = true;
                        if (CCNETDriver.hold_bill && (stepCount % 10) == 0)
                        {
                            result = CCNETDriver.Cmd(CCNETCommandEnum.Hold, (byte)CCNETDriver.BillAdr);
                        }

                        result = CCNETDriver.Cmd(CCNETCommandEnum.Poll, (byte)CCNETDriver.BillAdr);
                        CCNETDriver.send_bill_command = false;

                        if(result == false)
                        {
                            if (connectOff == false)    // первый раз - пошлем событие об этом
                            {
                                Message message = new Message();

                                message.Event = DeviceEvent.ConnectBillError;
                                message.Content = "";

                                log.Write(LogMessageType.Information, "BILL TASK: Нет связи с купюроприемником");

                                ReceivedResponse(this, new ServiceClientResponseEventArgs(message));

                                CCNETDriver.PollResults.Z1 = 0;
                                CCNETDriver.PollResults.Z2 = 0;

                                // если нет связи - опрашиваем реже
                                timeout = 2000;
                            }

                            connectOff = true;
                            CCNETDriver.NoConnectBill = true;
                        }
                        else
                        {
                            if (connectOff == true || (errorBill == true && (CCNETDriver.PollResults.Z1 != 0x47 
                                                                          && CCNETDriver.PollResults.Z1 != 0x44 
                                                                          && CCNETDriver.PollResults.Z1 != 0x45)))
                            {
                                // была пропажа питания или отсутствие связи
                                Message message = new Message();

                                message.Event = DeviceEvent.ConnectBillErrorEnd;
                                message.Content = "";

                                log.Write(LogMessageType.Information, "BILL TASK: Cвязь с купюроприемником появилась");

                                ReceivedResponse(this, new ServiceClientResponseEventArgs(message));

                                connectOff = false;
                                errorBill = false;

                                CCNETDriver.NoConnectBill = false;

                                try
                                {
                                    // перезагружаем
                                    CCNETDriver.restartBill();

                                    // сначала загрузим массив принимаемых купюр
                                    while (CCNETDriver.bill_recordIsEmpty())
                                    {
                                        if (CCNETDriver.GetBillTable().Contains("СБОЙ"))
                                        {
                                            //result = false;
                                        }
                                    }

                                    CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr);
                                }
                                catch (Exception exp)
                                {
                                    this.log.Write(LogMessageType.Debug, "BILL TASK: Ошибка: " + exp.GetDebugInformation());
                                }
                            }

                            // со связью все ок
                            timeout = 100;
                        }

                        // все события купюроприемника
                        if (CCNETDriver.PollResults.Z1 != 0 && CCNETDriver.PollResults.Z2 != 0)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptor;
                            message.Content = CCNETDriver.pollStatus();

                            log.Write(LogMessageType.Debug, "BILL TASK: " + message.Content);

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

                        // Drop Cassette Jammed
                        if (CCNETDriver.PollResults.Z1 == 0x44)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.DropCassetteJammed;
                            message.Content = null;

                            // один раз событие посылаем
                            if (errorBill == false) ReceivedResponse(this, new ServiceClientResponseEventArgs(message));

                            errorBill = true;
                        }

                        // Cheated
                        if (CCNETDriver.PollResults.Z1 == 0x45)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillCheated;
                            message.Content = null;

                            // один раз событие посылаем
                            if (errorBill == false) ReceivedResponse(this, new ServiceClientResponseEventArgs(message));

                            errorBill = true;
                        }

                        // ошибки приемника
                        if (CCNETDriver.PollResults.Z1 == 0x47)
                        {
                            Message message = new Message();

                            message.Event = DeviceEvent.BillAcceptorError;
                            message.Content = CCNETDriver.PollResults.Z2;

                            // один раз событие посылаем
                            if (errorBill == false) ReceivedResponse(this, new ServiceClientResponseEventArgs(message));

                            errorBill = true;
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
                    log.Write(LogMessageType.Error, "TASK BILL: " + exp.GetDebugInformation());
                    CCNETDriver.send_bill_command = false;
                }
                finally
                {
                    if (!e.Cancel)
                    {
                        // Нормальная работа
                        BillAcceptorEvent.WaitOne(timeout);

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
}
