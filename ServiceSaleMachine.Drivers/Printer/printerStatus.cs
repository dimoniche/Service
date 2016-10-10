using ServiceSaleMachine;
using System;
using System.IO.Ports;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class printerStatus
    {
        SerialPort serialPort;
        Log log = null;

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
                catch (Exception e)
                {
                    serialPort = null;
                    if (log != null) log.Write(LogMessageType.Error, "PRINTER OPEN: " + e.ToString());
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
                    catch (Exception e)
                    {
                        if (log != null) log.Write(LogMessageType.Error, "PRINTER CLOSE: " + e.ToString());
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
            catch (Exception e)
            {
                if (log != null) log.Write(LogMessageType.Error, "PRINTER SEND: " + e.ToString());
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
            catch(Exception e)
            {
                if (log != null) log.Write(LogMessageType.Error, "PRINTER SEND: " + e.ToString());
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

                if (log != null) log.Write(LogMessageType.Error, "PRINTER RECIVE: " + e.ToString());

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
            catch (Exception e)
            {
                count = 0;

                if (log != null) log.Write(LogMessageType.Error, "PRINTER RECIVE: " + e.ToString());

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
        public PaperEnableEnum CheckPaper(Log Log = null)
        {
            this.log = Log;

            openPort(getNumberComPort());

            if (serialPort == null && serialPort.IsOpen == false) return PaperEnableEnum.PaperError;

            byte[] buf = new byte[3];
            byte[] BufIn = new byte[20];

            // считаем статус бумаги
            buf[0] = 0x10;
            buf[1] = 0x04;
            buf[2] = 0x04;

            if (Send(buf) == false)
            {
                if (log != null) log.Write(LogMessageType.Error, "PRINTER: не послали проверку статуса");
            }

            Thread.Sleep(50);

            int val = 0;
            if (Recieve(BufIn, 1, out val) == false)
            {
                if (log != null) log.Write(LogMessageType.Error, "PRINTER: не приняли проверку статуса");

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
