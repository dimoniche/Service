using System;
using System.IO.Ports;
using System.Threading;

namespace AirVitamin.Drivers
{
    public class CCOMPort
    {
        string COM;                             //!< COM port number 
        SerialPort COMPort;                     //!< File handle associated with the COM port
        long[] EnablePorts = new long[16];		//!< An array containing port enumeration results (error codes) for firs 16 COM ports

        public long IsEnable(int iPort)
        {
            return EnablePorts[iPort];
        }

        public bool OpenCOM(string strCOM)
        {
            if (COMPort == null)
            {
                COM = strCOM;

                // настроим ком порт для прослушки
                COMPort = new SerialPort();
                COMPort.PortName = strCOM;
                COMPort.BaudRate = 9600;        // 9600
                COMPort.Parity = Parity.None;
                COMPort.StopBits = StopBits.One;
                COMPort.DataBits = 8;
                COMPort.Handshake = Handshake.None;
                COMPort.NewLine = "\r\n"; // Пусть пока будет "\r\n".

                COMPort.RtsEnable = false;
                COMPort.DtrEnable = false;


                COMPort.ReadTimeout = 2000;

                /*COMPort.DataReceived += SerialPortDataRecevied;
                COMPort.ErrorReceived += SerialPortErrorRecived;
                COMPort.PinChanged += SerialPortPinChanged;*/

                try
                {
                    COMPort.Open();
                }
                catch
                {
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
            
        }

        public void CloseCOM()
        {
            if (COMPort.IsOpen)
            {
                try
                {
                    /*COMPort.DataReceived -= SerialPortDataRecevied;
                    COMPort.ErrorReceived -= SerialPortErrorRecived;
                    COMPort.PinChanged -= SerialPortPinChanged;*/

                    COMPort.Close();
                }
                catch
                {

                }

                COMPort = null;
            }
        }

        public bool Send(byte[] Data, int Number)
        {
            try
            {
                COMPort.Write(Data, 0, Number);
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
                count = COMPort.Read(Buffer, 0, Length);
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
                count = COMPort.Read(Buffer, offset, Length);
                return true;
            }
            catch
            {
                count = 0;
                return false;
            }
        }

        public SerialPort GetHandle()
        {
            return COMPort;
        }

        public void DTR(bool bDTR)
        {
            COMPort.DtrEnable = bDTR;
            Thread.Sleep(1);
        }

        public void RTS(bool bRTS)
        {
            COMPort.RtsEnable = bRTS;
            Thread.Sleep(1);
        }

    }
}
