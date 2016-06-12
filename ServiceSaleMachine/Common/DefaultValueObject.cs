using System;

namespace ServiceSaleMachine
{
	public class DefaultValueObject<T>
	{
		public T DefaultValue { get; private set; }
		public T Value { get; set; }

		public DefaultValueObject()
		{
		}

		public DefaultValueObject(T defaultValue)
		{
			Value = DefaultValue = defaultValue;
		}

		public DefaultValueObject(T defaultValue, T value)
		{
			DefaultValue = defaultValue;
			Value = value;
		}

		public bool HasDifferent
		{
			get
			{
				return !object.Equals(Value, DefaultValue);
			}
		}

		public void Reset()
		{
			Value = DefaultValue;
		}
	}
}
