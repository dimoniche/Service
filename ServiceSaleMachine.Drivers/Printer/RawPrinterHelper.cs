using System;
using System.Runtime.InteropServices;

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

        [DllImport("winspool.Drv", EntryPoint = "AbortPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool AbortPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GetPrinter(
           IntPtr hPrinter,
           int dwLevel,
           IntPtr pPrinter,
           int dwBuf,
           out int dwNeeded
           );

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

        public void AbortPrint()
        {
            AbortPrinter(hPrinter);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PRINTER_INFO_6
        {
            public uint dwStatus;
        }

        public int GetPrinterStatusInt(string PrinterName)
        {
            int intRet = 0;
            IntPtr hPrinter;

            if (OpenPrinter(PrinterName, out hPrinter, (int)IntPtr.Zero))
            {
                int cbNeeded = 0;
                bool bolRet = GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out cbNeeded);
                if (cbNeeded > 0)
                {
                    IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded*4);
                    bolRet = GetPrinter(hPrinter, 2, pAddr, cbNeeded*4, out cbNeeded);
                    if (bolRet)
                    {
                        PRINTER_INFO_2 Info2 = new PRINTER_INFO_2();

                        Info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));

                        intRet = System.Convert.ToInt32(Info2.Status);
                    }
                    Marshal.FreeHGlobal(pAddr);
                }
                ClosePrinter(hPrinter);
            }

            return intRet;
        }
  
        public bool WriteByteArrayToPrinter(string PrinterName, byte[] array)
        {
            bool res = false;

            IntPtr hPrinter;

            if (OpenPrinter(PrinterName, out hPrinter, (int)IntPtr.Zero))
            {
                IntPtr pBytes = new IntPtr();
                Int32 dwWritten;

                Marshal.Copy(array,0,pBytes,array.Length);

                res = WritePrinter(hPrinter, pBytes, array.Length, out dwWritten);
                Marshal.FreeCoTaskMem(pBytes);

                ClosePrinter(hPrinter);
            }

            return res;
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

        public bool SendStringToPrinterUni(string szPrinterName, string szString)
        {
            bool res = false;

            if (PrinterOpen)
            {
                IntPtr pBytes;
                Int32 dwCount;
                Int32 dwWritten;

                dwCount = szString.Length * 2;
                pBytes = Marshal.StringToCoTaskMemUni(szString);
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
