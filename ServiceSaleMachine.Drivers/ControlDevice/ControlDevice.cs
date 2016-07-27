using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
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

        public bool openPort(string com_port)
        {
            if (serialPort == null)
            {
                // настроим ком порт
                serialPort = new SerialPort();
                serialPort.PortName = com_port;
                serialPort.BaudRate = 9600;        // 9600
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
