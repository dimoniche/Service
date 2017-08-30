using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AirVitamin
{
    public class BuckUpControlServiceTask
    {
        private SaleThread Worker { get; set; }

        internal bool SystemDataIsLoaded { get; set; }

        Log log;

        public BuckUpControlServiceTask(Log log)
        {
            this.log = log;

            SystemDataIsLoaded = false;

            Worker = new SaleThread();
            Worker.ThreadName = "BuckUpTask";
            Worker.Priority = ThreadPriority.BelowNormal;
            Worker.Work += Worker_Work;

            Start();
        }

        internal void Start()
        {
            lock (this)
            {
                Worker.Run();
            }
        }

        internal void Stop()
        {
            lock (this)
            {
                Worker.Abort();
            }
        }

        public void BuckUpBase()
        {
            if (Directory.Exists(Globals.DbConfiguration.folderBuckUp))
            {
                try
                {
                    GlobalDb.GlobalBase.Backup(Globals.DbConfiguration.folderBuckUp);

                    log.Write(LogMessageType.Information, "BUCKUP: Прошло резервирование базы.");
                }
                catch (Exception e)
                {
                    log.Write(LogMessageType.Error, "BUCKUP: Не получилось провести резервирование базы.", e);
                }
            }
            else
            {
                log.Write(LogMessageType.Error, "BUCKUP: Папка не доступна.");
            }
        }

        void Worker_Work(object sender, ThreadWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                try
                {
                    if(Globals.DbConfiguration.AutomaticBuckUp == 1)
                    {
                        BuckUpBase();
                    }
                }
                catch
                {
                }

                if (!e.Cancel)
                {
                    if (Globals.DbConfiguration.PeriodBuckUp < 1) Globals.DbConfiguration.PeriodBuckUp = 1;

                    SaleThread.Sleep(3600000 * Globals.DbConfiguration.PeriodBuckUp);
                }
            }
        }
    }
}
