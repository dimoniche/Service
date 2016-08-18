using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class ControlDevice
    {
        SerialPort serialPort;

        // Это буфер для накопления ответа
        public MemoryStream InputStream = new MemoryStream();
        internal readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public string getNumberComPort()
        {
            return Globals.ClientConfiguration.Settings.comPortControl;
        }

        public void setNumberComPort(string str)
        {
            Globals.ClientConfiguration.Settings.comPortControl = str;
            Globals.ClientConfiguration.Save();
        }

        public int getComPortSpeed()
        {
            return Globals.ClientConfiguration.Settings.comPortControlSpeed;
        }

        public void setComPortSpeed(int val)
        {
            Globals.ClientConfiguration.Settings.comPortControlSpeed = val;
            Globals.ClientConfiguration.Save();
        }

        public bool openPort(string com_port,int speed = 9600)
        {
            if (serialPort == null)
            {
                // настроим ком порт
                serialPort = new SerialPort();
                serialPort.PortName = com_port;
                serialPort.BaudRate = speed;        // 9600
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.None;
                serialPort.NewLine = "\r\n"; // Пусть пока будет "\r\n".
                serialPort.DtrEnable = true;
                serialPort.ReadTimeout = 3000;
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

        public void closePort()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Close();
                    }
                    catch
                    {

                    }

                    serialPort = null;
                }
            }
        }

        public bool Send(byte[] Data, int Number)
        {
            try
            {
                serialPort.Write(Data, 0, Number);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Recieve(byte[] Buffer, int Length, out int count)
        {
            try
            {
                count = serialPort.Read(Buffer, 0, Length);
                return true;
            }
            catch
            {
                count = 0;
                return false;
            }
        }

        public bool Recieve(byte[] Buffer, int offset, int Length, out int count)
        {
            try
            {
                count = serialPort.Read(Buffer, offset, Length);
                return true;
            }
            catch
            {
                count = 0;
                return false;
            }
        }

        /// <summary>
        /// Команда открыть
        /// </summary>
        /// <returns></returns>
        public void SendOpenControl(int controlNumber)
        {
            if (Globals.ClientConfiguration.Settings.offControl == 1) return;

            byte[] buf = new byte[2];

            buf[0] = (byte)(controlNumber * 2 - 1);
            buf[1] = (byte)(0xFF - buf[0]);
            this.Send(buf, 2);
        }

        /// <summary>
        /// Команда закрыть
        /// </summary>
        /// <returns></returns>
        public void SendCloseControl(int controlNumber)
        {
            if (Globals.ClientConfiguration.Settings.offControl == 1) return;

            byte[] buf = new byte[2];

            buf[0] = (byte)(controlNumber * 2);
            buf[1] = (byte)(0xFF - buf[0]);
            this.Send(buf, 2);
        }

        /// <summary>
        /// Пролучение статуса устройств
        /// </summary>
        /// <returns></returns>
        public byte[] GetStatusControl(Log log = null)
        {
            if (Globals.ClientConfiguration.Settings.offControl == 1) return null;

            byte[] res = new byte[1];
            byte[] buf = new byte[2];
            byte[] BufIn = new byte[10];

            buf[0] = (byte)0x0F;
            buf[1] = (byte)(0xFF - buf[0]);
            this.Send(buf, 2);

            if (log != null)
            {
                log.Write(LogMessageType.Error, "CONTROL: Transmit: " + buf[0].ToString("X") + " " + buf[1].ToString("X"));
            }

            int val = 0;
            if (this.Recieve(BufIn, 3, out val) == false)
            {
                return null;
            }

            if(log != null)
            {
                log.Write(LogMessageType.Error, "CONTROL: Data: " + BufIn[0].ToString("X") + " " + BufIn[1].ToString("X") + " " + BufIn[2].ToString("X") + " ");
            }

            // состояние 
            res[0] = BufIn[1];

            return res;
        }

        private void SerialPortPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            
        }

        private void SerialPortErrorRecived(object sender, SerialErrorReceivedEventArgs e)
        {
            
        }

        private void SerialPortDataRecevied(object sender, SerialDataReceivedEventArgs e)
        {
            
        }
    }
}
