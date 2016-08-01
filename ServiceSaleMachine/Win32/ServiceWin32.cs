using System;
using System.Runtime.InteropServices;

namespace ServiceSaleMachine
{
    public static class ServiceWin32
    {
        #region SHChangeNotify
        private const uint SHCNE_ASSOCCHANGED = 0x8000000U;
        private const uint SHCNF_IDLIST = 0x0U;
        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "SHChangeNotify")]
        private static extern void Win32_SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static void SHChangeNotify()
        {
            try
            {
                Win32_SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch
            {
            }
        }
        #endregion SHChangeNotify

        #region SHChangeNotify
        private const Int32 SC_CLOSE = 0xF060;
        private const Int32 MF_BYCOMMAND = 0x0;
        private const Int32 MF_GRAYED = 0x1;
        private const Int32 MF_ENABLED = 0x0;
        [DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "GetSystemMenu")]
        private extern static Int32 Win32_GetSystemMenu(Int32 hwnd, Int32 revert);
        [DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "EnableMenuItem")]
        private extern static Int32 Win32_EnableMenuItem(Int32 menu, Int32 ideEnableItem, Int32 enable);

        public static void HideCloseButton(IntPtr windowHandle)
        {
            try
            {
                Win32_EnableMenuItem(Win32_GetSystemMenu(windowHandle.ToInt32(), 0), SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
            catch
            {
            }
        }
        #endregion SHChangeNotify

        #region GetSpecialFolderLocation

        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, CSIDL nFolder, ref IntPtr ppidl);

        [DllImport("shell32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] System.Text.StringBuilder pszPath);

        public static string GetSpecialFolderLocation(CSIDL folderCode)
        {
            IntPtr pidl = Marshal.AllocHGlobal(1024);
            SHGetSpecialFolderLocation(pidl, folderCode, ref pidl);
            System.Text.StringBuilder path = new System.Text.StringBuilder(260);
            bool result = SHGetPathFromIDListW(pidl, path);
            Marshal.FreeHGlobal(pidl);
            if (result == false)
            {
                throw new ArgumentException();
            }
            return path.ToString().TrimEnd(char.MinValue);
        }

        #endregion GetSpecialFolderLocation

        #region RefreshTrayArea

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        public static void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", null);
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }

        #endregion RefreshTrayArea

        [DllImport("USER32.dll")]
        public static extern bool MessageBeep(uint uType);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int showWindowCommand);
    }

    public enum CSIDL
    {
        CSIDL_DESKTOP = 0x0000,    // <desktop>
        CSIDL_INTERNET = 0x0001,    // Internet Explorer (icon on desktop)
        CSIDL_PROGRAMS = 0x0002,    // Start Menu\Programs
        CSIDL_CONTROLS = 0x0003,    // My Computer\Control Panel
        CSIDL_PRINTERS = 0x0004,    // My Computer\Printers
        CSIDL_PERSONAL = 0x0005,    // My Documents
        CSIDL_FAVORITES = 0x0006,    // <user name>\Favorites
        CSIDL_STARTUP = 0x0007,    // Start Menu\Programs\Startup
        CSIDL_RECENT = 0x0008,    // <user name>\Recent
        CSIDL_SENDTO = 0x0009,    // <user name>\SendTo
        CSIDL_BITBUCKET = 0x000a,    // <desktop>\Recycle Bin
        CSIDL_STARTMENU = 0x000b,    // <user name>\Start Menu
        CSIDL_MYDOCUMENTS = 0x000c,    // logical "My Documents" desktop icon
        CSIDL_MYMUSIC = 0x000d,    // "My Music" folder
        CSIDL_MYVIDEO = 0x000e,    // "My Videos" folder
        CSIDL_DESKTOPDIRECTORY = 0x0010,    // <user name>\Desktop
        CSIDL_DRIVES = 0x0011,    // My Computer
        CSIDL_NETWORK = 0x0012,    // Network Neighborhood (My Network Places)
        CSIDL_NETHOOD = 0x0013,    // <user name>\nethood
        CSIDL_FONTS = 0x0014,    // windows\fonts
        CSIDL_TEMPLATES = 0x0015,
        CSIDL_COMMON_STARTMENU = 0x0016,    // All Users\Start Menu
        CSIDL_COMMON_PROGRAMS = 0X0017,    // All Users\Start Menu\Programs
        CSIDL_COMMON_STARTUP = 0x0018,    // All Users\Startup
        CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,    // All Users\Desktop
        CSIDL_APPDATA = 0x001a,    // <user name>\Application Data
        CSIDL_PRINTHOOD = 0x001b,    // <user name>\PrintHood
        CSIDL_LOCAL_APPDATA = 0x001c,    // <user name>\Local Settings\Applicaiton Data (non roaming)
        CSIDL_ALTSTARTUP = 0x001d,    // non localized startup
        CSIDL_COMMON_ALTSTARTUP = 0x001e,    // non localized common startup
        CSIDL_COMMON_FAVORITES = 0x001f,
        CSIDL_INTERNET_CACHE = 0x0020,
        CSIDL_COOKIES = 0x0021,
        CSIDL_HISTORY = 0x0022,
        CSIDL_COMMON_APPDATA = 0x0023,    // All Users\Application Data
        CSIDL_WINDOWS = 0x0024,    // GetWindowsDirectory()
        CSIDL_SYSTEM = 0x0025,    // GetSystemDirectory()
        CSIDL_PROGRAM_FILES = 0x0026,    // C:\Program Files
        CSIDL_MYPICTURES = 0x0027,    // C:\Program Files\My Pictures
        CSIDL_PROFILE = 0x0028,    // USERPROFILE
        CSIDL_SYSTEMX86 = 0x0029,    // x86 system directory on RISC
        CSIDL_PROGRAM_FILESX86 = 0x002a,    // x86 C:\Program Files on RISC
        CSIDL_PROGRAM_FILES_COMMON = 0x002b,    // C:\Program Files\Common
        CSIDL_PROGRAM_FILES_COMMONX86 = 0x002c,    // x86 Program Files\Common on RISC
        CSIDL_COMMON_TEMPLATES = 0x002d,    // All Users\Templates
        CSIDL_COMMON_DOCUMENTS = 0x002e,    // All Users\Documents
        CSIDL_COMMON_ADMINTOOLS = 0x002f,    // All Users\Start Menu\Programs\Administrative Tools
        CSIDL_ADMINTOOLS = 0x0030,    // <user name>\Start Menu\Programs\Administrative Tools
        CSIDL_CONNECTIONS = 0x0031,    // Network and Dial-up Connections
        CSIDL_COMMON_MUSIC = 0x0035,    // All Users\My Music
        CSIDL_COMMON_PICTURES = 0x0036,    // All Users\My Pictures
        CSIDL_COMMON_VIDEO = 0x0037,    // All Users\My Video
        CSIDL_CDBURN_AREA = 0x003b    // USERPROFILE\Local Settings\Application Data\Microsoft\CD Burning
    }
}
