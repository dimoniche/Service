using Drivers.Zebex;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class ZebexScaner
    {
        SerialPort serialPort;

        // Это буфер для накопления ответа
        public MemoryStream InputStream = new MemoryStream();
        internal readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public ZebexScaner(string com_port)
        {
            // настроим ком порт для прослушки
            serialPort = new SerialPort();
            serialPort.PortName = com_port;
            serialPort.BaudRate = 9600;        // 9600
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.NewLine = "\r\n"; // Пусть пока будет "\r\n".
            serialPort.DtrEnable = true;

            serialPort.DataReceived += SerialPortDataRecevied;
            serialPort.ErrorReceived += SerialPortErrorRecived;
            serialPort.PinChanged += SerialPortPinChanged;

            serialPort.Open();
        }

        private void SerialPortPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            
        }

        private void SerialPortErrorRecived(object sender, SerialErrorReceivedEventArgs e)
        {
            
        }

        private void SerialPortDataRecevied(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort port = (SerialPort)sender;

                if (port.IsOpen)
                {
                    int bytes = port.BytesToRead;

                    if (bytes > 0)
                    {
                        // Прочитаем все из СОМ-порта в буфер
                        byte[] byteAnswer = new byte[bytes];
                        try
                        {
                            port.Read(byteAnswer, 0, bytes);
                        }
                        catch { }

                        using (var ticket = LockTicket.Create(Locker, LockTypeEnum.Write))
                        {
                            InputStream.SetLength(0);

                            // Наполняем поток полученными данными
                            InputStream.Write(byteAnswer, 0, bytes);

                            // Обрабатываем полученные данные
                            MachineDrivers.ScanerEvent.Set();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void WorkerZebexDriver_Complete(object sender, ThreadCompleteEventArgs e)
        {
            
        }

        private void WorkerZebexDriver_Work(object sender, ThreadWorkEventArgs e)
        {

        }

        /// <summary>
        /// Установка режима работы устройства
        /// </summary>
        /// <param name="command"></param>
        public void Request(int command)
        {
            if (serialPort.IsOpen)
            {
                byte[] buf = new byte[1];

                switch (command)
                {
                    case 0:
                        buf[0] = (int)ZebexCommandEnum.disable;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case 1:
                        buf[0] = (int)ZebexCommandEnum.enable;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case 2:
                        buf[0] = (int)ZebexCommandEnum.powerUp;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case 3:
                        buf[0] = (int)ZebexCommandEnum.sleep;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case 4:
                        buf[0] = (int)ZebexCommandEnum.wakeUp;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                }
            }
        }
    }
}
