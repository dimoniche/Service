using System;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class ZebexScaner
    {
        SerialPort serialPort;

        // Это буфер для накопления ответа
        public MemoryStream InputStream = new MemoryStream();
        internal readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        int currentByte = 0;

        public ZebexScaner()
        {

        }

        public void closePort()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.DataReceived -= SerialPortDataRecevied;
                    serialPort.ErrorReceived -= SerialPortErrorRecived;
                    serialPort.PinChanged -= SerialPortPinChanged;

                    serialPort.Close();
                }
                catch
                {

                }

                serialPort = null;
            }
        }

        public string getNumberComPort()
        {
            return Globals.ClientConfiguration.Settings.comPortScanner;
        }

        public void setNumberComPort(string str)
        {
            Globals.ClientConfiguration.Settings.comPortScanner = str;
            Globals.ClientConfiguration.Save();
        }

        public bool openPort(string com_port)
        {
            if (serialPort == null)
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
                serialPort.ReadTimeout = 3000;

                serialPort.DataReceived += SerialPortDataRecevied;
                serialPort.ErrorReceived += SerialPortErrorRecived;
                serialPort.PinChanged += SerialPortPinChanged;

                try
                {
                    serialPort.Open();
                }
                catch
                {
                    serialPort = null;
                    return false;
                }
            }

            return true;
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

                    if (bytes >= 12)    // штрих код больше 12 символов
                    {
                        // Прочитаем все из СОМ-порта в буфер
                        byte[] byteAnswer = new byte[bytes];
                        try
                        {
                            port.Read(byteAnswer, 0, bytes);
                        }
                        catch { }

                        //var str = System.Text.Encoding.Default.GetString(byteAnswer);
                        //string result = Regex.Replace(str, "[^0-9]+", "");

                        //if(result.Length < 12)
                        //{
                        //    // не все цифры получили, если сканер не будет работать - нужно организовать прием нескольких частей
                        //}

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
        public void Request(ZebexCommandEnum command)
        {
            if (serialPort == null) return;

            if (serialPort.IsOpen)
            {
                // Сначала действия перед отсылкой данных
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                byte[] buf = new byte[1];

                switch (command)
                {
                    case ZebexCommandEnum.disable:
                        buf[0] = (int)ZebexCommandCodeEnum.disable;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case ZebexCommandEnum.enable:
                        buf[0] = (int)ZebexCommandCodeEnum.enable;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case ZebexCommandEnum.powerUp:
                        buf[0] = (int)ZebexCommandCodeEnum.powerUp;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case ZebexCommandEnum.sleep:
                        buf[0] = (int)ZebexCommandCodeEnum.sleep;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                    case ZebexCommandEnum.wakeUp:
                        buf[0] = (int)ZebexCommandCodeEnum.wakeUp;
                        serialPort.Write(buf, 0, buf.Length);
                        break;
                }
            }
        }
    }
}
