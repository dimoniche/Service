using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceSaleMachine.Drivers
{
    public class PrinterESC
    {
        // комманды принтера
        static string eClear = (char)27 + "@";
        static string eCentre = (char)27 + (char)(97) + "1";
        static string eLeft = (char)(27) + (char)(97) + "0";
        static string eRight = (char)(27) + (char)(97) + "2";
        static string eDrawer = eClear + (char)(27) + "p" + (char)(0) + ".}";
        static string eCut = (char)(27) + "i" + System.Environment.NewLine;
        static string eSmlText = (char)(27) + "!" + (char)(1);
        static string eNmlText = (char)(27) + "!" + (char)(0);
        static string eInit = eNmlText + (char)(13) + (char)(27) + "c6" + (char)(1) + (char)(27) + "R3" + System.Environment.NewLine;
        static string eBigCharOn = (char)(27) + "!" + (char)(56);
        static string eBigCharOff = (char)(27) + "!" + (char)(0);
        static string GS = ((char)0x1D).ToString();

        static string vbCr = "\r";
        static string vbLf = "\n";
        static string vbCrLf = "\r\n";

        RawPrinterHelper prn = new RawPrinterHelper();
        string PrinterName = "Citizen PPU-700";

        public PrinterESC()
        {

        }

        public PrinterESC(string name)
        {
            PrinterName = name;

        }

        public void StartPrint()
        {
            prn.OpenPrint(PrinterName);
        }

        public void PrintHeader()
        {
            Print(eInit + eCentre + "My Shop");
            Print("Tel:0123 456 7890");
            Print("Web: www.????.com");
            Print("sales@????.com");
            Print("VAT Reg No:123 4567 89" + eLeft);
            PrintDashes();
        }

        public void PrintBody()
        {
            Print(eSmlText + "Tea");
            PrintDashes();
            Print(eSmlText + "-------");
            Print("--------");
        }

        public void PrintFooter()
        {
            Print(eCentre + "Thank You For Your Support!" + eLeft);
            Print(vbLf + vbLf + vbLf + vbLf + vbLf + eCut + eDrawer);
        }

        public void Print(String Line)
        {
            prn.SendStringToPrinter(PrinterName, Line + vbLf);
        }

        public void PrintDashes()
        {
            Print(eLeft + eNmlText + "-".PadRight(42, '-'));
        }

        public void EndPrint()
        {
            prn.ClosePrint();
        }

        public void PrintBarCode(string msg)
        {
            Print(getBarcodeStr(msg));
        }

        public string getBarcodeStr(string msg)
        {
            string str = "";
            str = GS + "h" + (char)(80);                    // Barcode Width
            str += GS + "w" + (char)(1);                    // Font for HRI characters
            str += GS + "f" + (char)(0);                    // Position of HRI characters
            str += GS + "H" + (char)(2);                    // Print Barcode Smb 39
            str += GS + "k" + (char)(69) + (char)(20);      // Print Text Under
            str += msg + (char)(0) + vbCrLf;                //
            str += GS + "d" + (char)(3) + vbCrLf;           //
            str += GS + "@";

            return str;
        }
    }
}
