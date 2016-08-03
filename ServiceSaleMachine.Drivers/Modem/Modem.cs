using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class Modem
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

        public bool openPort(string com_port, int speed = 9600)
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

        public bool Send(byte[] Data)
        {
            try
            {
                serialPort.Write(Data, 0, Data.Length);
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

        private void SerialPortPinChanged(object sender, SerialPinChangedEventArgs e)
        {

        }

        private void SerialPortErrorRecived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        private void SerialPortDataRecevied(object sender, SerialDataReceivedEventArgs e)
        {

        }

        public bool SendSMS(string sms)
        {
            byte[] buf = CommonHelper.GetBytes("++++");
            byte[] CR = { 0x0D };
            byte[] cntrlZ = { 0x1A };
            byte[] BufIn = new byte[20];

            // в командный режим
            Send(buf);

            buf = CommonHelper.GetBytes("AT+CMGF=1");

            Send(buf); 
            Send(CR);

            int val = 0;
            if (Recieve(BufIn, 2, out val) == false)
            {
                return false;
            }

            if(!System.Text.Encoding.UTF8.GetString(BufIn).Contains("OK"))
            {
                return false;
            }

            buf = CommonHelper.GetBytes("AT+CMGS=" + Globals.ClientConfiguration.Settings.numberTelephoneSMS);
            Send(buf);
            Send(CR);

            if (Recieve(BufIn, 5, out val) == false)
            {
                return false;
            }

            if (!System.Text.Encoding.UTF8.GetString(BufIn).Contains("OK"))
            {
                return false;
            }

            buf = CommonHelper.GetBytes(sms);

            if (Recieve(BufIn, 5, out val) == false)
            {
                return false;
            }

            if (!System.Text.Encoding.UTF8.GetString(BufIn).Contains("OK"))
            {
                return false;
            }

            return true;
        }
    }
}
