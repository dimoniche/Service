using ServiceSaleMachine;
using System;
using System.IO.Ports;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class printerStatus
    {
        SerialPort serialPort;

        public string getNumberComPort()
        {
            return Globals.ClientConfiguration.Settings.comPortPrinter;
        }

        public void setNumberComPort(string str)
        {
            Globals.ClientConfiguration.Settings.comPortPrinter = str;
            Globals.ClientConfiguration.Save();
        }

        public bool openPort(string com_port, int speed = 19200)
        {
            if (serialPort == null)
            {
                // настроим ком порт
                serialPort = new SerialPort();
                serialPort.PortName = com_port;
                serialPort.BaudRate = speed;        // 19200
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.None;
                serialPort.NewLine = "\r\n"; // Пусть пока будет "\r\n".
                serialPort.DtrEnable = true;
                serialPort.ReadTimeout = 5000;

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
            catch (Exception e)
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

        /// <summary>
        /// проверка статуса бумаги
        /// </summary>
        /// <param name="sms"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public PaperEnableEnum CheckPaper(Log log = null)
        {
            openPort(getNumberComPort());

            byte[] buf = new byte[3];
            byte[] BufIn = new byte[20];

            // считаем статус бумаги
            buf[0] = 0x10;
            buf[1] = 0x04;
            buf[2] = 0x04;

            Send(buf);


            Thread.Sleep(5);

            int val = 0;
            if (Recieve(BufIn, 1, out val) == false)
            {
                if (log != null) log.Write(LogMessageType.Error, "PRINTER: не послали проверку статуса");

                closePort();
                return PaperEnableEnum.PaperError;
            }

            if ((BufIn[0] & 0x04) > 0) return PaperEnableEnum.PaperNearEnd;
            if ((BufIn[0] & 0x08) > 0) return PaperEnableEnum.PaperNearEnd;
            if ((BufIn[0] & 0x20) > 0) return PaperEnableEnum.PaperEnd;
            if ((BufIn[0] & 0x40) > 0) return PaperEnableEnum.PaperEnd;

            closePort();

            return PaperEnableEnum.PaperOk;
        }
    }
}
