using ServiceSaleMachine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ServiceSaleMachine
{
	/// <summary>
	/// Выполняет чистку старых файлов
	/// </summary>
	public class ClearFilesControlServiceTask
	{
		private SaleThread Worker { get; set; }

		internal bool SystemDataIsLoaded { get; set; }

        Log log;
 
		public ClearFilesControlServiceTask(Log log)
		{
            this.log = log;

			SystemDataIsLoaded = false;

			Worker = new SaleThread();
			Worker.ThreadName = "ClearOldFilesControl";
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

		/// <summary>
		/// Удаляет старые файлы в указанном каталоге, маске и максимальному возрасту в днях
		/// </summary>
		public void DeleteOldFiles(string path, string fileMask, int maxFileAge, SearchOption searchOption)
		{
			if (Directory.Exists(path))
			{
				try
				{
					// Каталог существует, получаем перечень интересующих нас файлов по маске
					IEnumerable<string> filePaths = Directory.EnumerateFiles(path, fileMask, searchOption);

					foreach (string filePath in filePaths)
					{
						DateTime fileCreationTime = File.GetCreationTime(filePath);

						if ((DateTime.Now - fileCreationTime).Days >= maxFileAge)
						{
							// Этот файл старый, удаляем
							if (FileHelper.TryDelete(filePath))
							{
                                log.Write(LogMessageType.Information, "Удален старый файл: " + filePath);
							}
						}
					}
				}
				catch (Exception)
				{
                    log.Write(LogMessageType.Debug, "Не удалось произвести чистку каталога от старых файлов");
				}
			}
		}

		void Worker_Work(object sender, ThreadWorkEventArgs e)
		{
			while (!e.Cancel)
			{
				try
				{
					DeleteOldFiles(Globals.GetPath(PathEnum.Logs), "*." + Globals.LogExtention, 30, SearchOption.TopDirectoryOnly);
				}
				catch
				{
				}

				// Период - 10 минут
				if (!e.Cancel)
				{
                    SaleThread.Sleep(600000);
				}
			}
		}
	}
}
