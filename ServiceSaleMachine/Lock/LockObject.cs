using System;
using System.Threading;

namespace AirVitamin
{
	public class LockObject<T> where T : class
	{
		internal ReaderWriterLockSlim Locker;
		internal T Object;

		public LockObject()
		{
			Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		}

		public LockObject(T obj)
		{
			Locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			Object = obj;
		}

		public LockTicket<T> Lock(LockTypeEnum lockType)
		{
			return LockTicket<T>.Create(this, lockType, TimeSpan.FromMilliseconds(-1));
		}

		public LockTicket<T> TryLock(LockTypeEnum lockType, int millisecondsTimeout)
		{
			return LockTicket<T>.Create(this, lockType, TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		public LockTicket<T> TryLock(LockTypeEnum lockType, TimeSpan timeout)
		{
			return LockTicket<T>.Create(this, lockType, timeout);
		}

		public LockTicket<T> Read()
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Read, TimeSpan.FromMilliseconds(-1));
		}

		public LockTicket<T> TryRead(int millisecondsTimeout)
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Read, TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		public LockTicket<T> TryRead(TimeSpan timeout)
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Read, timeout);
		}

		public LockTicket<T> Write()
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Write, TimeSpan.FromMilliseconds(-1));
		}

		public LockTicket<T> TryWrite(int millisecondsTimeout)
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Write, TimeSpan.FromMilliseconds(millisecondsTimeout));
		}

		public LockTicket<T> TryWrite(TimeSpan timeout)
		{
			return LockTicket<T>.Create(this, LockTypeEnum.Write, timeout);
		}
	}
}
