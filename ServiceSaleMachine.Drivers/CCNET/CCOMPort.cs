using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServiceSaleMachine.Drivers
{
    public class CCOMPort
    {
        int iCOM;                               //!< COM port number 
        SerialPort COMPort;                     //!< File handle associated with the COM port
        long[] EnablePorts = new long[16];		//!< An array containing port enumeration results (error codes) for firs 16 COM ports

        public long IsEnable(int iPort)
        {
            return EnablePorts[iPort];
        }

        public bool OpenCOM(int COMi)
        {
            if (COMPort == null)
            {
                iCOM = COMi;

                string com_port = "COM" + COMi;

                // настроим ком порт для прослушки
                COMPort = new SerialPort();
                COMPort.PortName = com_port;
                COMPort.BaudRate = 9600;        // 9600
                COMPort.Parity = Parity.None;
                COMPort.StopBits = StopBits.One;
                COMPort.DataBits = 8;
                COMPort.Handshake = Handshake.None;
                COMPort.NewLine = "\r\n"; // Пусть пока будет "\r\n".
                COMPort.DtrEnable = true;

                /*COMPort.DataReceived += SerialPortDataRecevied;
                COMPort.ErrorReceived += SerialPortErrorRecived;
                COMPort.PinChanged += SerialPortPinChanged;*/

                COMPort.Open();
            }

            return true;
        }

        public void CloseCOM()
        {
            if (COMPort.IsOpen)
            {
                try
                {
                    //COMPort.DataReceived -= SerialPortDataRecevied;
                    //COMPort.ErrorReceived -= SerialPortErrorRecived;
                    //COMPort.PinChanged -= SerialPortPinChanged;

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

        public bool Recieve(byte[] Buffer, int Length)
        {
            try
            {
                COMPort.Read(Buffer, 0, Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Recieve(byte[] Buffer, int offset, int Length)
        {
            try
            {
                COMPort.Read(Buffer, offset, Length);
                return true;
            }
            catch
            {
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
