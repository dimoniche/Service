using System;

namespace AirVitamin
{
	public class ThreadCompleteEventArgs
	{
		///// <summary>
		///// Указывает следует ли перезапустить задание
		///// </summary>
		//public bool Restart { get; set; }
		/// <summary>
		/// Указывает была ли прервана фоновая операция вызовом метода Cancel или Abort.
		/// </summary>
		public bool IsCanceled { get; internal set; }
		/// <summary>
		/// Указывает был ли вызван метод Abort явно или в результате длительного Cancel.
		/// </summary>
		public bool IsAborted { get; internal set; }
		/// <summary>
		/// Указывает на успешность завершения операции.
		/// Значение true обозначает успешность завершения, false иначе.
		/// </summary>
		public bool IsCompleted
		{
			get
			{
				return (!IsCanceled && !IsAborted);
			}
		}
		/// <summary>
		/// Возвращает пользовательское значение результат фоновой операции.
		/// </summary>
		public object Result { get; internal set; }
		public object Argument { get; internal set; }
		public Exception Error { get; internal set; }
	}
}
