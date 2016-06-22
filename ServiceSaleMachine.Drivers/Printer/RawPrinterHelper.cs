using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.IO;

namespace ServiceSaleMachine.Drivers
{
    public class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOC_INFO_1
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, Int32 pDefault);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true,CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi,ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level,
                                                  [In, MarshalAs(UnmanagedType.LPStruct)] DOC_INFO_1 di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true,CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true,CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true,CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        IntPtr hPrinter = new IntPtr(0);
        bool PrinterOpen = false;
        DOC_INFO_1 di = new DOC_INFO_1();

        public bool PrinterIsOpen { get { return PrinterOpen; } set { } }

        public bool OpenPrint(string name)
        {
            if(PrinterOpen == false)
            {
                di.pDocName = ".NET RAW Document";
                di.pDataType = "RAW";

                if(OpenPrinter(name.Normalize(), out hPrinter, (int)IntPtr.Zero))
                {
                    if(StartDocPrinter(hPrinter, 1, di))
                    {
                        if(StartPagePrinter(hPrinter))
                        {
                            PrinterOpen = true;
                        }
                    }
                }
            }
            return PrinterOpen;
        }

        public void ClosePrint()
        {
            if (PrinterOpen)
            {
                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
                ClosePrinter(hPrinter);
                PrinterOpen = false;
            }
        }

        public bool SendStringToPrinter(string szPrinterName, string szString)
        {
            bool res = false;

            if (PrinterOpen)
            {
                IntPtr pBytes;
                Int32 dwCount;
                Int32 dwWritten;

                dwCount = szString.Length;
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                res = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                Marshal.FreeCoTaskMem(pBytes);
            }
            else
            {
                res = false;
            }

            return res;
        }
    }
}
