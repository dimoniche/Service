using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AirVitamin
{
	public class Log : IDisposable
	{
		private LockObject<object> Locker;
		private bool disposed = false;

		private FileStream file;
		private StreamWriter fileWriter;
		private string DirPath;
		private string FileName;
		private DateTime CurrentDate;

		public LogMessageTypeEnum MinMessageType { get; set; }
		public bool AllowWriteToConsole { get; set; }
		public bool AllowWriteThreadId { get; set; }
		public bool AllowWriteThread { get; set; }

		public Log()
		{
			Init(Globals.GetPath(PathEnum.Logs), Globals.GetProcessFileName(false), null);
		}

		private void Init(string dirPath, string fileName, string fileNamePostFix)
		{
			Locker = new LockObject<object>();

			MinMessageType = LogMessageTypeEnum.None;

			DirPath = dirPath;
			FileName = fileName + fileNamePostFix + "." + Globals.LogExtention;
		}

		public void Close()
		{
			using (Locker.Write())
			{
				if (fileWriter != null)
				{
					fileWriter.Dispose();
					fileWriter = null;
				}

				if (file != null)
				{
					file.Dispose();
					file = null;
				}
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				Close();
			}
		}

		public void Write(LogMessageType messageType, string message)
		{
			if (messageType.MessageType > MinMessageType) return;

			using (Locker.Write())
			{
				WriteInternal(messageType, message, null);
			}
		}

		public void Write(LogMessageType messageType, string message, string content)
		{
			if (messageType.MessageType > MinMessageType) return;

			using (Locker.Write())
			{
				WriteInternal(messageType, message, content);
			}
		}

		public void Write(LogMessageType messageType, string message, Exception error)
		{
			if (messageType.MessageType > MinMessageType) return;

			using (Locker.Write())
			{
				string content = error.Message;

				WriteInternal(messageType, message, content);
			}
		}

		private void WriteInternal(LogMessageType messageType, string message, string content)
		{
			DateTime nowTime = DateTime.Now;

			// Выводим в консоль, если разрешен вывод
			if (AllowWriteToConsole)
			{
				// Выводим сообщение
				Console.WriteLine(nowTime.ToString("HH:mm:ss.fff") + " " + message);
				// Выводим контент
				if (!string.IsNullOrWhiteSpace(content)) Console.WriteLine(content);
				Console.WriteLine("");
			}

			// Формируем строку сообщения для вывода
			StringBuilder builder = new StringBuilder();
			builder.Append(messageType.SaveName);
			builder.Append(nowTime.ToString("HH:mm:ss.fff"));
			builder.Append(" ");
			builder.Append(message);

			// Записываем в файл
			try
			{
				if (CurrentDate != DateTime.MinValue && CurrentDate != nowTime.Date)
				{
					// Изменилась текущая дата, закрываем файл
					Close();
				}

				if (fileWriter == null)
				{
					if (!Directory.Exists(DirPath))
						Directory.CreateDirectory(DirPath);

					// Открываем файл
					string filePath = DirPath + "\\" + nowTime.Date.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture) + " " + FileName;
					file = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
					fileWriter = new StreamWriter(file, Encoding.UTF8);
					CurrentDate = nowTime.Date;
				}

				// Записываем сообщение
				fileWriter.Write(builder.ToString());
				fileWriter.Write(Environment.NewLine);

				// Записываем контент
				if (!string.IsNullOrWhiteSpace(content))
				{
					fileWriter.Write(content);
					fileWriter.Write(Environment.NewLine);
				}
				fileWriter.Flush();
			}
			catch
			{
				Close();
			}

			builder.Clear();
		}
	}
}
