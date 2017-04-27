using System;

namespace AirVitamin
{
	public class ThreadProgressChangedEventArgs : EventArgs
	{
		public int? Percent { get; private set; }
		public object UserState { get; private set; }

		public ThreadProgressChangedEventArgs(int? percent, object userState)
		{
			Percent = percent;
			UserState = userState;
		}
	}
}
