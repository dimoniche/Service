using System;

namespace ServiceSaleMachine
{
	public class ThreadWorkEventArgs : EventArgs
	{
		public SaleThread Thread { get; private set; }

		/// <summary>
		/// Указывает нужно ли прервать запущенную операцию
		/// </summary>
		public bool Cancel { get; internal set; }

		/// <summary>
		/// Переданный при запуске параметр
		/// </summary>
		public object Argument { get; internal set; }

		/// <summary>
		/// Возвращает или задает пользовательское значение результат фоновой операции.
		/// </summary>
		public object Result { get; set; }

		internal bool Abort { get; set; }

		public ThreadWorkEventArgs(SaleThread thread)
		{
			Thread = thread;
		}

		internal void Clear()
		{
			Cancel = false;
			Abort = false;
			Argument = null;
			Result = null;
		}
	}
}
