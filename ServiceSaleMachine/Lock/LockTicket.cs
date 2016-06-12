using System;
using System.Threading;

namespace ServiceSaleMachine
{
	public sealed class LockTicket<T> : LockTicket where T : class
	{
		private LockObject<T> LockObject;

		public T Object
		{
			get
			{
				if (!LockObject.Locker.IsReadLockHeld && !LockObject.Locker.IsWriteLockHeld) throw new SynchronizationLockException();
				return LockObject.Object;
			}

			set
			{
				if (!LockObject.Locker.IsWriteLockHeld) throw new SynchronizationLockException();
				LockObject.Object = value;
			}
		}

		private LockTicket(LockObject<T> lockObject, LockTypeEnum lockType)
			: base(lockObject.Locker, lockType)
		{
			LockObject = lockObject;
		}

		internal static LockTicket<T> Create(LockObject<T> lockObject, LockTypeEnum lockType)
		{
			return TryLock(lockObject.Locker, lockType, ref TimeSpanInfinite) ? new LockTicket<T>(lockObject, lockType) : null;
		}

		internal static LockTicket<T> Create(LockObject<T> lockObject, LockTypeEnum lockType, TimeSpan timeout)
		{
			return TryLock(lockObject.Locker, lockType, ref timeout) ? new LockTicket<T>(lockObject, lockType) : null;
		}
	}

	public class LockTicket : IDisposable
	{
		protected static TimeSpan TimeSpanInfinite = TimeSpan.FromMilliseconds(Timeout.Infinite);
		private bool isDisposed = false;
		private ReaderWriterLockSlim Locker;
		private LockTypeEnum LockType;

		protected LockTicket(ReaderWriterLockSlim locker, LockTypeEnum lockType)
		{
			Locker = locker;
			LockType = lockType;
		}

		public static LockTicket Create(ReaderWriterLockSlim locker, LockTypeEnum lockType)
		{
			return TryLock(locker, lockType, ref TimeSpanInfinite) ? new LockTicket(locker, lockType) : null;
		}

		public static LockTicket Create(ReaderWriterLockSlim locker, LockTypeEnum lockType, TimeSpan timeout)
		{
			return TryLock(locker, lockType, ref timeout) ? new LockTicket(locker, lockType) : null;
		}

		protected static bool TryLock(ReaderWriterLockSlim locker, LockTypeEnum lockType, ref TimeSpan timeout)
		{
			switch (lockType)
			{
				case LockTypeEnum.Read:
					return locker.TryEnterReadLock(timeout);

				case LockTypeEnum.Write:
					return locker.TryEnterWriteLock(timeout);

				default:
					return false;
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;

				switch (LockType)
				{
					case LockTypeEnum.Read:
						Locker.ExitReadLock();
						break;

					case LockTypeEnum.Write:
						Locker.ExitWriteLock();
						break;
				}
			}
		}
	}
}
