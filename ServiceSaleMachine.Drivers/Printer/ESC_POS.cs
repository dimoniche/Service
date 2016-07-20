using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public class PrinterESC
    {
        // комманды принтера
        static string eClear = (char)(27) + "@";
        static string eCentre = (char)(27) + "" + (char)(97) + "1";
        static string eLeft = (char)(27) + "" + (char)(97) + "0";
        static string eRight = (char)(27) + "" + (char)(97) + "2";

        static string eDrawer = eClear + (char)(27) + "p" + (char)(0) + "" + (char)(60) + "" + (char)(120);
        static string eCut = (char)(29) + "V" + (char)(65);

        static string eSmlText = (char)(27) + "!" + (char)(1);
        static string eNmlText = (char)(27) + "!" + (char)(0);
        static string eInit = (char)(27) + "@" + "" + eNmlText;

        static string eBigCharOn = (char)(27) + "!" + (char)(56);
        static string eBigCharOff = (char)(27) + "!" + (char)(0);

        static string eSelectRusCodePage = (char)(27) + "t" + (char)(17);

        static string escUnerlineOn = (char)(27) + "" + (char)(45) + "" + (char)(1);    // Unerline On
        static string escUnerlineOnx2 = (char)(27) + "" + (char)(45) + "" + (char)(2);  // Unerline On x 2
        static string escUnerlineOff = (char)(27) + "" + (char)(45) + "" + (char)(0);   // Unerline Off
        static string escBoldOn = (char)(27) + "" + (char)(69) + "" + (char)(1);        // Bold On
        static string escBoldOff = (char)(27) + "" + (char)(69) + "" + (char)(0);       // Bold Off
        static string escNegativeOn = (char)(29) + "" + (char)(66) + "" + (char)(1);    // White On Black On'
        static string escNegativeOff = (char)(29) + "" + (char)(66) + "" + (char)(0);   // White On Black Off
        static string esc8CpiOn = (char)(29) + "" + (char)(33) + "" + (char)(16);       // Font Size x2 On
        static string esc8CpiOff = (char)(29) + "" + (char)(33) + "" + (char)(0);       // Font Size x2 Off

        static string GS = "" + (char)(0x1D);

        static string vbCr = "\r";
        static string vbLf = "\n";
        static string vbCrLf = "\r\n";

        static string SelectBarcodeHeght = (char)(29) + "h" + (char)(80);
        static string PrintBarcode = (char)(29) + "k" + (char)(69) + "" + (char)(13);

        public RawPrinterHelper prn = new RawPrinterHelper();
        string PrinterName = "CITIZEN PPU-700";

        byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public PrinterESC()
        {

        }

        public PrinterESC(string name)
        {
            PrinterName = name;

        }

        public int findPrinterIndex(string name)
        {
            int index = 0;
            string[] printers = new string[PrinterSettings.InstalledPrinters.Count];

            foreach (string strPrinter in PrinterSettings.InstalledPrinters)
            {
                if(name == strPrinter)
                {
                    break;
                }

                index++;
            }


            return index;
        }

        public string[] findAllPrinter()
        {
            string[] printers = new string[PrinterSettings.InstalledPrinters.Count];
            int i = 0;

            foreach (string strPrinter in PrinterSettings.InstalledPrinters)
            {
                printers[i] = strPrinter;
                i++;
            }

            return printers;
        }

        public void StartPrint(string str)
        {
            PrinterName = str;
            prn.OpenPrint(PrinterName);
        }

        public bool OpenPrint(string str)
        {
            PrinterName = str;
            return prn.OpenPrint(PrinterName);
        }

        public void ClosePrint()
        {
            prn.ClosePrint();
        }

        public string getNamePrinter()
        {
            return Globals.ClientConfiguration.Settings.NamePrinter;
        }

        public void setNamePrinter(string str)
        {
            Globals.ClientConfiguration.Settings.NamePrinter = str;
            Globals.ClientConfiguration.Save();
        }

        public void PrintHeader()
        {
            Print(eInit + "" + eSelectRusCodePage + "" + eCentre + "" + TransformCode(Globals.CheckConfiguration.Settings.firmsname));
            Print(TransformCode(Globals.CheckConfiguration.Settings.secondfirmsname));
            Print(eLeft + DateTime.Now.ToString("yyyy-MM-dd") + "                          " + DateTime.Now.ToString("HH:mm"));
            Print(eLeft);
            PrintDashes();
        }

        public void PrintBody(Service serv)
        {
            Print("" + TransformCode(serv.caption) + " ".PadRight(42 - serv.caption.Length - serv.price.ToString().Length, ' ') + serv.price.ToString());
            PrintDashes();
            Print(eLeft + TransformCode("ИТОГ") + " ".PadRight(42 - 4 - serv.price.ToString().Length, ' ') + serv.price.ToString());
            Print(eLeft + TransformCode("Налич") + " ".PadRight(42 - 5 - serv.price.ToString().Length, ' ') + serv.price.ToString());
            Print(eLeft + TransformCode("Сдача") + " ".PadRight(42 - 5 - 1, ' ') + "0");
        }

        public void PrintFooter()
        {
            Print(eCentre + "" + TransformCode("СПАСИБО") + eLeft);
            Print("");
            Print(Globals.CheckConfiguration.Settings.advert1);
            Print(Globals.CheckConfiguration.Settings.advert2);
            Print(Globals.CheckConfiguration.Settings.advert3);
            Print(Globals.CheckConfiguration.Settings.advert4);
            Print(vbLf + vbLf + vbLf + vbLf + vbLf + eCut);
        }

        public void Print(String Line, bool end = true)
        {
            if (end)
            {
                prn.SendStringToPrinter(PrinterName, Line + vbCrLf);
            }
            else
            {
                prn.SendStringToPrinter(PrinterName, Line);
            }
        }

        public void PrintDashes()
        {
            Print(eLeft.ToString() + eNmlText.ToString() + "-".PadRight(42, '-'));
        }

        public void EndPrint()
        {
            prn.ClosePrint();
        }

        public void PrintBarCode(string msg)
        {
            Print(eCentre + getBarcodeStr(msg) + vbCrLf);
        }

        public string getBarcodeStr(string msg)
        {
            string str = "";

            str = GS + "h" + (char)(160);                   // ДЛИНА
            str += GS + "w" + (char)(4);                    // Barcode Width 
            str += GS + "f" + (char)(0);                    // Font for HRI characters
            str += GS + "H" + (char)(2);                    // Position of HRI characters
            str += GS + "k" + (char)(2);                    // Print Barcode Smb 39
            str += msg + (char)(0) + "" + vbCrLf;           // Print Text Under

            return str;
        }

        public string TransformCode(string Str)
        {
            int i = 0;
            string NewStr = "", tmpStr;

            while (i < Str.Length)
            {
                switch (Str[i])
                {
                    case 'А': tmpStr = "" + (char)(0x80); break;
                    case 'Б': tmpStr = "" + (char)(0x81); break;
                    case 'В': tmpStr = "" + (char)(0x82); break;
                    case 'Г': tmpStr = "" + (char)(0x83); break;
                    case 'Д': tmpStr = "" + (char)(0x84); break;
                    case 'Е': tmpStr = "" + (char)(0x85); break;
                    case 'Ё': tmpStr = "" + (char)(0xF0); break;
                    case 'Ж': tmpStr = "" + (char)(0x86); break;
                    case 'З': tmpStr = "" + (char)(0x87); break;
                    case 'И': tmpStr = "" + (char)(0x88); break;
                    case 'Й': tmpStr = "" + (char)(0x89); break;
                    case 'К': tmpStr = "" + (char)(0x8A); break;
                    case 'Л': tmpStr = "" + (char)(0x8B); break;
                    case 'М': tmpStr = "" + (char)(0x8C); break;
                    case 'Н': tmpStr = "" + (char)(0x8D); break;
                    case 'О': tmpStr = "" + (char)(0x8E); break;
                    case 'П': tmpStr = "" + (char)(0x8F); break;
                    case 'Р': tmpStr = "" + (char)(0x90); break;
                    case 'С': tmpStr = "" + (char)(0x91); break;
                    case 'Т': tmpStr = "" + (char)(0x92); break;
                    case 'У': tmpStr = "" + (char)(0x93); break;
                    case 'Ф': tmpStr = "" + (char)(0x94); break;
                    case 'Х': tmpStr = "" + (char)(0x95); break;
                    case 'Ц': tmpStr = "" + (char)(0x96); break;
                    case 'Ч': tmpStr = "" + (char)(0x97); break;
                    case 'Ш': tmpStr = "" + (char)(0x98); break;
                    case 'Щ': tmpStr = "" + (char)(0x99); break;
                    case 'Ъ': tmpStr = "" + (char)(0x9A); break;
                    case 'Ы': tmpStr = "" + (char)(0x9B); break;
                    case 'Ь': tmpStr = "" + (char)(0x9C); break;
                    case 'Э': tmpStr = "" + (char)(0x9D); break;
                    case 'Ю': tmpStr = "" + (char)(0x9E); break;
                    case 'Я': tmpStr = "" + (char)(0x9F); break;

                    case 'а': tmpStr = "" + (char)(0xA0); break;
                    case 'б': tmpStr = "" + (char)(0xA1); break;
                    case 'в': tmpStr = "" + (char)(0xA2); break;
                    case 'г': tmpStr = "" + (char)(0xA3); break;
                    case 'д': tmpStr = "" + (char)(0xA4); break;
                    case 'е': tmpStr = "" + (char)(0xA5); break;
                    case 'ё': tmpStr = "" + (char)(0xF1); break;
                    case 'ж': tmpStr = "" + (char)(0xA6); break;
                    case 'з': tmpStr = "" + (char)(0xA7); break;
                    case 'и': tmpStr = "" + (char)(0xA8); break;
                    case 'й': tmpStr = "" + (char)(0xA9); break;
                    case 'к': tmpStr = "" + (char)(0xAA); break;
                    case 'л': tmpStr = "" + (char)(0xAB); break;
                    case 'м': tmpStr = "" + (char)(0xAC); break;
                    case 'н': tmpStr = "" + (char)(0xAD); break;
                    case 'о': tmpStr = "" + (char)(0xAE); break;
                    case 'п': tmpStr = "" + (char)(0xAF); break;
                    case 'р': tmpStr = "" + (char)(0xE0); break;
                    case 'с': tmpStr = "" + (char)(0xE1); break;
                    case 'т': tmpStr = "" + (char)(0xE2); break;
                    case 'у': tmpStr = "" + (char)(0xE3); break;
                    case 'ф': tmpStr = "" + (char)(0xE4); break;
                    case 'х': tmpStr = "" + (char)(0xE5); break;
                    case 'ц': tmpStr = "" + (char)(0xE6); break;
                    case 'ч': tmpStr = "" + (char)(0xE7); break;
                    case 'ш': tmpStr = "" + (char)(0xE8); break;
                    case 'щ': tmpStr = "" + (char)(0xE9); break;
                    case 'ъ': tmpStr = "" + (char)(0xEA); break;
                    case 'ы': tmpStr = "" + (char)(0xEB); break;
                    case 'ь': tmpStr = "" + (char)(0xEC); break;
                    case 'э': tmpStr = "" + (char)(0xED); break;
                    case 'ю': tmpStr = "" + (char)(0xEE); break;
                    case 'я': tmpStr = "" + (char)(0xEF); break;
                    default:  tmpStr = "" + Str[i]; break;
                }

                NewStr = NewStr + tmpStr;
                i = i + 1;
            }

            return NewStr;
        }
    }
}
