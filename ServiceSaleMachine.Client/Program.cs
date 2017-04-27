using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AirVitamin.Client
{
    static class Program
    {
        internal static Log Log { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (!Globals.RegistrySettings.Load())
            {
                MessageBox.Show(Globals.ErrorMessageRegistryDontRead, "test", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach(string arg in args)
            {
                switch (arg)
                {
                    case "/admin":
                        {

                            Globals.admin = true;
                            break;
                        }
                }

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
            else
            {
                Log.MinMessageType = LogMessageTypeEnum.Information;
            }

            FormManager.CatchError += FormManager_CatchError;

            string fileName = Globals.GetPath(PathEnum.Image) + "\\";

            if (!Globals.DesignConfiguration.Load())
            { }

            if (!Globals.DbConfiguration.Load())
            {
                Globals.DbConfiguration.Save();
                Globals.DbConfiguration.Load();
            }

            if (!Globals.CheckConfiguration.Load())
            {
                Globals.CheckConfiguration.Save();
                Globals.CheckConfiguration.Load();
            }

            if (!Globals.ClientConfiguration.Load())
            {
                Globals.ClientConfiguration.Settings.comPortScanner = "NULL";
                Globals.ClientConfiguration.Settings.comPortBill = "NULL";
                Globals.ClientConfiguration.Settings.adressBill = "NULL";
                Globals.ClientConfiguration.Settings.comPortPrinter = "NULL";
                Globals.ClientConfiguration.Settings.NamePrinter = "NULL";
                Globals.ClientConfiguration.Settings.comPortControl = "NULL";

                // пока 3 сервиса
                Globals.ClientConfiguration.Settings.services = new List<Service>();
                
                Globals.ClientConfiguration.Settings.services.Add(new Service(1,"Сервис 1", fileName + "sol.png"));
                Globals.ClientConfiguration.Settings.services.Add(new Service(2,"Сервис 2", fileName + "sol.png"));
                Globals.ClientConfiguration.Settings.services.Add(new Service(3,"Сервис 3", fileName + "sol.png"));

                Globals.ClientConfiguration.Save();
                Globals.ClientConfiguration.Load();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WorkException(e.ExceptionObject as Exception);
        }

        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WorkException(e.Exception);
        }

        private static void FormManager_CatchError(object sender, FormErrorEventArgs e)
        {
            WorkException(e.Error);
        }

        private static void WorkException(Exception ex)
        {
            // На всякий случай
            try
            {
                Form parentForm = null;
                if (FormManager.MainForm != null && !FormManager.MainForm.IsDisposed)
                    parentForm = FormManager.MainForm;

                if (ex.IsAssignableTo(typeof(UserException)))
                {
                    MessageBox.Show(parentForm, ex.Message, FormManager.AppCaptionName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (Log != null) Log.Write(LogMessageType.Error, ex.GetDebugInformation());
                    FormError.TryShow(parentForm, ex);
                }
            }
            catch { }
        }
    }
}
