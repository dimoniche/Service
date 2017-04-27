using System;
using System.ComponentModel;
using System.Threading;

namespace AirVitamin
{
	public class SaleThread : Component
	{
		#region Закрытые члены
		private object locker = new object();
		private Thread thread;
		private ThreadWorkEventArgs workArgs;
		private EventWaitHandle startHandle;
		private EventWaitHandle stopHandle;
		private EventWaitHandle completeHandle;
		private bool disposed;
		#endregion Закрытые члены

		#region События
		public delegate void ThreadWorkHandler(object sender, ThreadWorkEventArgs e);
		public delegate void ThreadCompleteHandler(object sender, ThreadCompleteEventArgs e);
		public delegate void ThreadProgressChangedHandler(object sender, ThreadProgressChangedEventArgs e);
		public event ThreadWorkHandler Work;
		public event ThreadCompleteHandler Complete;
		public event ThreadProgressChangedHandler ProgressChanged;
		#endregion События

		#region Свойства
		[Browsable(false)]
		public bool IsWork { get; private set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public object Tag { get; set; }

		public ThreadPriority Priority { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsAvailableToRun
		{
			get
			{
				lock (locker)
				{
					return (thread == null && Work != null && Work.GetInvocationList().Length == 1);
				}
			}
		}

		//[Browsable(false)]
		//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ThreadName { get; set; }
		#endregion Свойства

		#region Конструкторы и деструкторы
		public SaleThread()
		{
			disposed = false;
			IsWork = false;
			workArgs = new ThreadWorkEventArgs(this);
			Priority = ThreadPriority.Normal;
		}

		public SaleThread(IContainer container)
			: this()
		{
			container.Add(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && IsWork && !disposed)
			{
				disposed = true;
				Abort();
				Work = null;
				Complete = null;
			}
			base.Dispose(disposing);
		}
		#endregion Конструкторы и деструкторы

		#region Открытые методы
		/// <summary>
		/// Начинает выполнение фоновой операции.
		/// </summary>
		public void Run()
		{
			Run(null, false);
		}

		/// <summary>
		/// Начинает выполнение фоновой операции.
		/// </summary>
		public void Run(object Argument)
		{
			Run(Argument, true);
		}

		/// <summary>
		/// Начинает выполнение фоновой операции.
		/// </summary>
		private void Run(object Argument, bool UseArgument)
		{
			lock (locker)
			{
				//if (thread != null || Work == null || Work.GetInvocationList().Length > 1) throw new InvalidOperationException();
				if (!IsAvailableToRun) throw new InvalidOperationException();

				startHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
				stopHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
				completeHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
				if (UseArgument)
				{
					thread = new Thread(new ParameterizedThreadStart(OnWork));
					if (ThreadName != null)
						thread.Name = ThreadName;
					thread.IsBackground = true;
					thread.Priority = Priority;
					thread.Start(Argument);
				}
				else
				{
					thread = new Thread(new ThreadStart(OnWork));
					if (ThreadName != null)
						thread.Name = ThreadName;
					thread.IsBackground = true;
					thread.Priority = Priority;
					thread.Start();
				}
			}
			startHandle.WaitOne();
		}

		public void Cancel()
		{
			Cancel(System.Threading.Timeout.Infinite);
		}

		/// <summary>
		/// Устанавливает значение WorkEventArgs.Cancel в true и ожидает завершение фоновой операции заданное время.
		/// Для "вечного" ожидания нужно указать значение System.Threading.Timeout.Infinite (-1).
		/// </summary>
		public void Cancel(int miliseconds, bool waitCompleted = true)
		{
			lock (locker)
			{
				if (thread != null && IsWork)
				{
					try
					{
						workArgs.Cancel = true;
						if (thread.ThreadState.HasFlag(ThreadState.WaitSleepJoin))
						{
							workArgs.Abort = true;
							thread.Interrupt();
						}
						if (!stopHandle.WaitOne(miliseconds))
						{
							workArgs.Abort = true;
							thread.Abort();
						}
					}
					catch (NullReferenceException)
					{
					}
				}
			}
			if (waitCompleted && completeHandle != null) completeHandle.WaitOne();
		}

		public void CancelAsync()
		{
			lock (locker)
			{
				if (thread != null)
				{
					workArgs.Cancel = true;
				}
			}
		}

		/// <summary>
		/// Немедленно завершает фоновую операцию.
		/// </summary>
		public void Abort(bool waitCompleted = true)
		{
			lock (locker)
			{
				bool doAbort;
				doAbort = (thread != null && IsWork);
				if (doAbort)
				{
					try
					{
						workArgs.Cancel = true;
						workArgs.Abort = true;
						if (thread.ThreadState.HasFlag(ThreadState.WaitSleepJoin))
						{
							thread.Interrupt();
						}
						else
						{
							thread.Abort();
						}
					}
					catch (NullReferenceException)
					{
					}
					stopHandle.WaitOne();
				}
			}
			if (waitCompleted && completeHandle != null) completeHandle.WaitOne();
		}

		/// <summary>
		/// Передает информацию подписчику события ProgressChanged о ходе выполнения фоновой операции.
		/// </summary>
		public void SetProgress(object userState)
		{
			ThreadProgressChangedEventArgs args = new ThreadProgressChangedEventArgs(null, userState);
			OnProgressChanged(args);
		}

		/// <summary>
		/// Передает информацию подписчику события ProgressChanged о ходе выполнения фоновой операции.
		/// </summary>
		public void SetProgress(int percent, object userState)
		{
			if (percent < 0 || percent > 100) throw new ArgumentOutOfRangeException("Percent");
			ThreadProgressChangedEventArgs args = new ThreadProgressChangedEventArgs(percent, userState);
			OnProgressChanged(args);
		}

		public static bool Sleep(int millisecondsTimeout)
		{
			EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			return waitHandle.WaitOne(millisecondsTimeout);
		}

		public static bool Sleep(TimeSpan timeout)
		{
			EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
			return waitHandle.WaitOne(timeout);
		}
		#endregion Открытые методы

		#region Закрытые методы
		private void OnWork()
		{
			OnWork(null);
		}

		private void OnWork(object Argument)
		{
			ThreadCompleteEventArgs completeArgs = new ThreadCompleteEventArgs();
			try
			{
				lock (locker)
				{
					workArgs.Clear();
					workArgs.Argument = Argument;
					completeArgs.Argument = Argument;
					IsWork = true;
					startHandle.Set();
				}
				Work(this, workArgs);
			}
			catch (ThreadAbortException)
			{
				Thread.ResetAbort();
			}
			catch (ThreadInterruptedException)
			{
			}
			catch (Exception ex)
			{
				completeArgs.Error = ex;
			}
			finally
			{
				IsWork = false;
				stopHandle.Set();

				//lock (locker)
				{
					completeArgs.IsCanceled = workArgs.Cancel;
					completeArgs.IsAborted = workArgs.Abort;
					completeArgs.Result = workArgs.Result;
				}

				completeHandle.Set();
				thread = null;

				OnComplete(completeArgs);

				//if (completeArgs.Restart)
				//{
				//    if (Argument != null)
				//        Run(Argument);
				//    else
				//        Run();
				//}
			}
		}

		private void SendEvent(Delegate[] delegates, object[] objs)
		{
			ISynchronizeInvoke invokeRequired;
			foreach (Delegate del in delegates)
			{
				invokeRequired = del.Target as ISynchronizeInvoke;
				if (invokeRequired != null)
				{
					//invokeRequired.Invoke(del, new object[] { this, args } );
					IAsyncResult asyncOperation = null;
					bool operationCompleted = false;
					while (!operationCompleted && !workArgs.Cancel && !workArgs.Abort)
					{
						if (asyncOperation == null) asyncOperation = invokeRequired.BeginInvoke(del, objs);
						operationCompleted = asyncOperation.AsyncWaitHandle.WaitOne(50);
					}

					////IAsyncResult asyncOperation = invokeRequired.BeginInvoke(del, objs);
					////bool operationCompleted;
					////do
					////{
					////    operationCompleted = asyncOperation.AsyncWaitHandle.WaitOne(50);
					////}
					////while (!operationCompleted && !workArgs.Cancel && !workArgs.Abort);
				}
				else
				{
					del.Method.Invoke(del.Target, objs);
				}
			}
		}

		private void OnComplete(ThreadCompleteEventArgs args)
		{
			if (Complete == null) return;
			try
			{
				SendEvent(Complete.GetInvocationList(), new object[] { this, args });
			}
			catch
			{
			}
		}

		private void OnProgressChanged(ThreadProgressChangedEventArgs args)
		{
			if (ProgressChanged == null) return;
			try
			{
				SendEvent(ProgressChanged.GetInvocationList(), new object[] { this, args });
			}
			catch
			{
			}
		}
		#endregion Закрытые методы

	}
}
