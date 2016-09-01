using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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

        static string eSelectRusCodePage = (char)(27) + "t" + (char)(7);// + (char)(27) + "R" + (char)(0) + (char)(27) + "G" + (char)(1);

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

        // Запрос состояния датчика бумаги
        static string PaperStatus = "" + (char)(0x10) + (char)(0x04) + (char)(4);

        public RawPrinterHelper prn = new RawPrinterHelper();
        string PrinterName = "CITIZEN PPU-700";

        // статус принтера
        public printerStatus status;

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
            status = new printerStatus();
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

            if (index == PrinterSettings.InstalledPrinters.Count) return -1;

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


        /// <summary>
        /// Печать чека инкассации
        /// </summary>
        public void PrintCheckСollection(MoneyStatistic statistic)
        {
            Print(eInit + "" + eSelectRusCodePage + "" + eCentre, false, false);
            Print(TransformCode(Globals.CheckConfiguration.Settings.firmsname), true);
            Print(eLeft);

            Print(eLeft + DateTime.Now.ToString("yyyy-MM-dd") + "                          " + DateTime.Now.ToString("HH:mm"));
            Print(eLeft);

            PrintDashes();
            Print(TransformCode("Чек инкассации"), true);
            Print(eLeft);
            PrintDashes();

            Print(eLeft, false, false);
            Print(TransformCode("Общая сумма   "), true);
            Print(" ".PadRight(42 - 20 - statistic.AllMoneySumm.ToString().Length, ' ') + statistic.AllMoneySumm.ToString("00000.00") + TransformCode(" руб"), true);
            Print(eLeft);

            Print(eLeft, false, false);
            Print(TransformCode("Сумма на акк. "), true);
            Print(" ".PadRight(42 - 20 - statistic.AccountMoneySumm.ToString().Length, ' ') + statistic.AccountMoneySumm.ToString("00000.00") + TransformCode(" руб"), true);
            Print(eLeft);

            Print(eLeft, false, false);
            Print(TransformCode("Сумма на чеках"), true);
            Print(" ".PadRight(42 - 20 - statistic.BarCodeMoneySumm.ToString().Length, ' ') + statistic.BarCodeMoneySumm.ToString("00000.00") + TransformCode(" руб"), true);
            Print(eLeft);

            Print(eLeft, false, false);
            Print(TransformCode("Оказано услуг "), true);
            Print(" ".PadRight(42 - 20 - statistic.ServiceMoneySumm.ToString().Length, ' ') + statistic.ServiceMoneySumm.ToString("00000.00") + TransformCode(" руб"), true);
            Print(eLeft);

            Print("");
            Print(vbLf + vbLf + vbLf + eCut);

            prn.ClosePrint();
        }

        public void PrintHeader()
        {
            Print(eInit + "" + eSelectRusCodePage + "" + eCentre,false,false);
            Print(TransformCode(Globals.CheckConfiguration.Settings.firmsname),true);
            Print(eLeft);

            Print(TransformCode(Globals.CheckConfiguration.Settings.secondfirmsname),true);
            Print(eLeft);

            // номер чека
            Print(TransformCode("Чек N: ") + GlobalDb.GlobalBase.GetCurrentNumberCheck().ToString("00000"), true);
            Print(eLeft);

            Print(eLeft + DateTime.Now.ToString("yyyy-MM-dd") + "                          " + DateTime.Now.ToString("HH:mm"));
            Print(eLeft);
            PrintDashes();
        }

        public void PrintBody(Service serv)
        {
            Print(TransformCode(Globals.CheckConfiguration.Settings.PreviouslyService) + " " + TransformCode(serv.caption),true);
            Print(eLeft);
            Print(" ".PadRight(42 - 3 - serv.price.ToString().Length, ' ') + serv.price.ToString("#.00"));

            PrintDashes();

            Print(eLeft,false,false);
            Print(TransformCode("ИТОГ"),true);
            Print(" ".PadRight(42 - 7 - serv.price.ToString().Length, ' ') + serv.price.ToString("#.00"));

            Print(eLeft, false, false);
            Print(TransformCode("Налич"),true);
            Print(" ".PadRight(42 - 8 - serv.price.ToString().Length, ' ') + serv.price.ToString("#.00"));

            Print(eLeft, false, false);
            Print(TransformCode("Сдача"),true);
            Print(" ".PadRight(42 - 8 - 1, ' ') + "0,00");
        }

        public void PrintFooter()
        {
            Print(eCentre,false,false);
            Print(TransformCode("СПАСИБО"),true);
            Print(eLeft);

            Print("");
            if (Globals.CheckConfiguration.Settings.advert1.Length > 1)
            {
                Print(TransformCode(Globals.CheckConfiguration.Settings.advert1), true);
                Print(eLeft);
            }
            if (Globals.CheckConfiguration.Settings.advert2.Length > 1)
            {
                Print(TransformCode(Globals.CheckConfiguration.Settings.advert2), true);
                Print(eLeft);
            }
            if (Globals.CheckConfiguration.Settings.advert3.Length > 1)
            {
                Print(TransformCode(Globals.CheckConfiguration.Settings.advert3), true);
                Print(eLeft);
            }
            if (Globals.CheckConfiguration.Settings.advert4.Length > 1)
            {
                Print(TransformCode(Globals.CheckConfiguration.Settings.advert4), true);
                Print(eLeft);
            }

            Print(vbLf + vbLf + vbLf + eCut);
        }

        public void Print(String Line, bool uni = false,bool end = true)
        {
            if (uni)
            {
                prn.SendStringToPrinterUni(PrinterName, Line);
            }
            else
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

                NewStr += tmpStr;
                i++;
            }

            return NewStr;
        }

 
        // печать картинки
        public string GetLogo(string file)
        {
            string logo = "";
            if (!File.Exists(Globals.GetPath(PathEnum.Image) + "\\" + file)) return null;

            BitmapData data = GetBitmapData(Globals.GetPath(PathEnum.Image) + "\\" + file);
            BitArray dots = data.Dots;

            byte[] width = BitConverter.GetBytes(data.Width);

            int offset = 0;
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write((char)0x1B);
            bw.Write('@');

            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)24);

            while (offset < data.Height)
            {
                bw.Write((char)0x1B);
                bw.Write('*');         // bit-image mode
                bw.Write((byte)33);    // 24-dot double-density
                bw.Write(width[0]);  // width low byte
                bw.Write(width[1]);  // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;
                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        bw.Write(slice);
                    }
                }
                offset += 24;
                bw.Write((char)0x0A);
            }
            // Restore the line spacing to the default of 30 dots.
            bw.Write((char)0x1B);
            bw.Write('3');
            bw.Write((byte)30);

            bw.Flush();
            byte[] bytes = stream.ToArray();
            return logo + Encoding.Default.GetString(bytes);
        }

        public BitmapData GetBitmapData(string bmpFileName)
        {
            using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
            {
                var threshold = 127;
                var index = 0;
                double multiplier = 504; // this depends on your printer model. for Beiyang you should use 1000
                double scale = (double)(multiplier / (double)bitmap.Width);
                int xheight = (int)(bitmap.Height * scale);
                int xwidth = (int)(bitmap.Width * scale);
                var dimensions = xwidth * xheight;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < xheight; y++)
                {
                    for (var x = 0; x < xwidth; x++)
                    {
                        var _x = (int)(x / scale);
                        var _y = (int)(y / scale);
                        var color = bitmap.GetPixel(_x, _y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = (int)(bitmap.Height * scale),
                    Width = (int)(bitmap.Width * scale)
                };
            }
        }

        public class BitmapData
        {
            public BitArray Dots
            {
                get;
                set;
            }

            public int Height
            {
                get;
                set;
            }

            public int Width
            {
                get;
                set;
            }
        }

        public void PrintBitMap(string file)
        {
            Print(GetLogo(file));
        }

        public void PrintBitMapHeader()
        {
            Print(eInit + "" + eSelectRusCodePage);
            PrintBitMap("Header.bmp");
            Print(eLeft + DateTime.Now.ToString("yyyy-MM-dd") + "                          " + DateTime.Now.ToString("HH:mm"));
            Print(eLeft);
            PrintDashes();
        }

        public void PrintBitMapBody(Service service)
        {
            PrintBitMap("BodyNameService.bmp");
            PrintDashes();
            PrintBitMap("BodyMoney.bmp");
        }

        public void PrintBitMapFooter()
        {
            PrintBitMap("Footer.bmp");
            Print("");
            Print(vbLf + vbLf + vbLf + eCut);
        }
    }
}
