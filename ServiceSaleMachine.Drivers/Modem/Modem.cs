using System;
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
            return Globals.ClientConfiguration.Settings.comPortModem;
        }

        public void setNumberComPort(string str)
        {
            Globals.ClientConfiguration.Settings.comPortModem = str;
            Globals.ClientConfiguration.Save();
        }

        public int getComPortSpeed()
        {
            return Globals.ClientConfiguration.Settings.comPortModemSpeed;
        }

        public void setComPortSpeed(int val)
        {
            Globals.ClientConfiguration.Settings.comPortModemSpeed = val;
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
                serialPort.ReadTimeout = 1000;

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

        private static string reversePhone(string phone)
        {
            string LTel = "";

            if (phone.Length % 2 == 1)
                phone = phone + 'F';

            for (int i = 0; i < phone.Length; i++)
            {
                if (i % 2 != 0)
                {
                    LTel = LTel + phone[i] + phone[i - 1];
                }
            }

            return LTel;
        }

        private static string StringToUSC2(string text)
        {       
            byte[] textOnBytes = System.Text.Encoding.GetEncoding("UTF-16").GetBytes(text);
            string textInUSC = "";

            for (int i = 0; i<textOnBytes.Length; i++)
            {
                string buff = ((int)textOnBytes[i]).ToString("X2");

                if (buff.Length % 2 == 1)
                {
                    textInUSC += "0";
                }
                textInUSC += buff;
            }

            string msgTextLength = ((int)textInUSC.Length / 2).ToString("X2");

            // Если длина нечётная - добавляем в начале 0       
            if (msgTextLength.Length % 2 == 1)
                msgTextLength = "0" + msgTextLength;

            return (msgTextLength + textInUSC).ToUpper(); 
        }

        public bool SendSMSRus(string sms)
        {
            if (Globals.ClientConfiguration.Settings.offModem == 1) return false;
            if (Globals.ClientConfiguration.Settings.numberTelephoneSMS.Length < 7) return false;

            string LMes;

            string LText = StringToUSC2(sms);

            // Длина и номер SMS центра. 0 - означает, что будет использоваться дефолтный номер.
            LMes = "00";
            // SMS-SUBMIT
            LMes = LMes + "11";
            // устанавливается устройством, изначально должно быть 00;
            LMes = LMes + "00";
            // Длина и номер отправителя
            LMes = LMes + "00";
            // Длина номера получателя
            LMes = LMes + Globals.ClientConfiguration.Settings.numberTelephoneSMS.Length.ToString("X2");
            // Тип-адреса. (91 указывает международный формат телефонного номера, 81 - местный формат).
            LMes = LMes + "91";
            // Телефонный номер получателя в международном формате.
            LMes = LMes + reversePhone(Globals.ClientConfiguration.Settings.numberTelephoneSMS);
            // Идентификатор протокола
            LMes = LMes + "00";
            // Старший полубайт означает сохранять SMS у получателя или нет (Flash SMS),  Младший полубайт - кодировка(0-латиница 8-кирилица).
            LMes = LMes + "08";
            // Срок доставки сообщения. С1 - неделя
            LMes = LMes + "C1";
            // Длина текста сообщения.
            LMes = LMes + (LText.Length / 2).ToString("X2");
            LMes = LMes + LText;

            byte[] buf;
            byte[] BufIn = new byte[20];

            buf = System.Text.Encoding.ASCII.GetBytes("AT+CMGF=0\x0d\x0a");

            Send(buf);

            int val = 0;
            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            string str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains("OK"))
            {
                return false;
            }

            buf = System.Text.Encoding.ASCII.GetBytes("AT+CMGS=" + (LMes.Length-2)/2 + "\x0d\x0a");
            Send(buf);

            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains(">") && !str.Contains("OK"))
            {
                return false;
            }

            buf = System.Text.Encoding.ASCII.GetBytes(LMes + "\x0d\x0a\x1A");
            Send(buf);

            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains(">") && !str.Contains("OK"))
            {
                return false;
            }

            return true;
        }

        public static string Translit(string str)
        {
            string[] lat_up = { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
            string[] lat_low = { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
            string[] rus_up = { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я" };
            string[] rus_low = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
            for (int i = 0; i <= 32; i++)
            {
                str = str.Replace(rus_up[i], lat_up[i]);
                str = str.Replace(rus_low[i], lat_low[i]);
            }
            return str;
        }

        public bool SendSMS(string sms)
        {
            if (Globals.ClientConfiguration.Settings.offModem == 1) return false;
            if (Globals.ClientConfiguration.Settings.numberTelephoneSMS.Length < 7) return false;

            byte[] buf;
            byte[] BufIn = new byte[20];

            sms = Translit(sms);

            buf = System.Text.Encoding.ASCII.GetBytes("AT+CMGF=1\x0d\x0a");

            Send(buf);

            Thread.Sleep(500);

            int val = 0;
            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            string str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains("OK"))
            {
                return false;
            }

            buf = System.Text.Encoding.ASCII.GetBytes("AT+CMGS=\"" + Globals.ClientConfiguration.Settings.numberTelephoneSMS + "\"\x0d\x0a");
            Send(buf);

            Thread.Sleep(500);

            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains(">") && !str.Contains("OK"))
            {
                return false;
            }

            buf = System.Text.Encoding.ASCII.GetBytes(sms + "\x0d\x0a\x1A");
            Send(buf);

            Thread.Sleep(1000);

            if (Recieve(BufIn, 20, out val) == false)
            {
                return false;
            }

            str = System.Text.Encoding.UTF8.GetString(BufIn);
            if (!str.Contains(">") && !str.Contains("OK"))
            {
                return false;
            }

            return true;
        }
    }
}
