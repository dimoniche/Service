using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServiceSaleMachine.Client
{
    static class Program
    {
        internal static Log Log { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Globals.RegistrySettings.Load())
            {
                MessageBox.Show(Globals.ErrorMessageRegistryDontRead, "test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Создадим журнал клиента
            Log = new Log { MinMessageType = LogMessageTypeEnum.Warning, AllowWriteToConsole = false };

            // В случае отладки будем сохранять максимум информации
            if (Globals.IsDebug)
            {
                Log.MinMessageType = LogMessageTypeEnum.Debug;
                Log.AllowWriteThreadId = true;
                Log.AllowWriteThread = true;
            }

            if (!Globals.ClientConfiguration.Load())
            {
                Globals.ClientConfiguration.Settings.comPortScanner = "NULL";
                Globals.ClientConfiguration.Settings.comPortBill = "NULL";
                Globals.ClientConfiguration.Settings.adressBill = "NULL";
                Globals.ClientConfiguration.Settings.comPortPrinter = "NULL";
                Globals.ClientConfiguration.Settings.NamePrinter = "NULL";
                Globals.ClientConfiguration.Settings.comPortControl = "NULL";

                Globals.ClientConfiguration.Save();
                Globals.ClientConfiguration.Load();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
