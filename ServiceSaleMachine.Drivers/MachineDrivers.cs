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
        public ZebexScaner scaner;
        public CCRSProtocol CCNETDriver;
        
        public delegate void ServiceClientResponseEventHandler(object sender, ServiceClientResponseEventArgs e);

        // событие обновления данных
        public event ServiceClientResponseEventHandler ReceivedResponse;

        // массив номиналов купюр
        public _BillRecord[] bill_record = new _BillRecord[24];

        public bool hold_bill = false;  // удерживать купюру

        public MachineDrivers()
        {
            // настроим драйвер сканера
            scaner = new ZebexScaner();

            // запустим задачу ожидания сообщений от сканера
            WorkerScanerDriver = new SaleThread { ThreadName = "WorkerScanerDriver" };
            WorkerScanerDriver.Work += WorkerScanerDriver_Work;
            WorkerScanerDriver.Complete += WorkerScanerDriver_Complete;

            WorkerScanerDriver.Run();

            // настроим купюроприемник
            CCNETDriver = new CCRSProtocol();

            WorkerBillPollDriver = new SaleThread { ThreadName = "WorkerBillPollDriver" };
            WorkerBillPollDriver.Work += WorkerBillPollDriver_Work;
            WorkerBillPollDriver.Complete += WorkerBillPollDriver_Complete;

            for (int i = 0; i < 24; i++)
            {
                bill_record[i] = new _BillRecord();
            }
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

                if (CCNETDriver.Cmd(CCNETCommandEnum.BillType, (byte)CCNETDriver.BillAdr, (long)0x00, (long)0x00) == true)
                {

                }
            }
        }

        private void WorkerBillPollDriver_Complete(object sender, ThreadCompleteEventArgs e)
        {
            
        }

        private void WorkerBillPollDriver_Work(object sender, ThreadWorkEventArgs e)
        {
            int stepCount = 0;

            CCNETDriver.Cmd(CCNETCommandEnum.Information, (byte)CCNETDriver.BillAdr);

            while (!e.Cancel)
            {
                bool hasErrors = false;

                try
                {
                    if (hold_bill && (stepCount%10) == 0)
                    {
                        CCNETDriver.Cmd(CCNETCommandEnum.Hold, (byte)CCNETDriver.BillAdr);
                    }

                    CCNETDriver.Cmd(CCNETCommandEnum.Poll, (byte)CCNETDriver.BillAdr);

                    if(CCNETDriver.PollResults.Z1 != 0 && CCNETDriver.PollResults.Z2 != 0)
                    {
                        Message message = new Message();

                        message.Recipient = MessageEndPoint.BillAcceptor;
                        message.Content = CCNETDriver.pollStatus();

                        ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                    }

                    if (CCNETDriver.PollResults.Z1 == 0x81)
                    {
                        Message message = new Message();

                        message.Recipient = MessageEndPoint.BillAcceptorCredit;
                        message.Content = bill_record[CCNETDriver.PollResults.Z2].Denomination.ToString();

                        ReceivedResponse(this, new ServiceClientResponseEventArgs(message));
                    }
                }
                catch
                {
                    hasErrors = true;
                }
                finally
                {
                    if (hasErrors)
                    {
                        
                    }

                    if (!e.Cancel)
                    {
                        // Нормальная работа
                        ScanerEvent.WaitOne(200);

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

            message.Recipient = MessageEndPoint.Scaner;
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
