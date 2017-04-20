using System;

namespace AirVitamin
{
	public class LogMessageType
	{
		public static LogMessageType Error { get; private set; }
		public static LogMessageType Warning { get; private set; }
		public static LogMessageType Information { get; private set; }
		public static LogMessageType Debug { get; private set; }

		public LogMessageTypeEnum MessageType { get; private set; }
		public string SaveName { get; private set; }

		static LogMessageType()
		{
			Error = new LogMessageType(LogMessageTypeEnum.Error, "[ERROR] ");
			Warning = new LogMessageType(LogMessageTypeEnum.Warning, "[WARN]  ");
			Information = new LogMessageType(LogMessageTypeEnum.Information, "[INFO]  ");
			Debug = new LogMessageType(LogMessageTypeEnum.Debug, "[DEBUG] ");
		}

		private LogMessageType(LogMessageTypeEnum messageType, string saveName)
		{
			MessageType = messageType;
			SaveName = saveName;
		}
	}
}
