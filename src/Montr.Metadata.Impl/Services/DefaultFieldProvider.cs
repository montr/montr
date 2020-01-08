using System;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	public class DefaultFieldProvider<TFieldType, TClrType> : IFieldProvider
	{
		public Type FieldType => typeof(TFieldType);

		public object Read(string value)
		{
			return value != null ? ReadInternal(value) : (object) null;
		}

		public virtual TClrType ReadInternal(string value)
		{
			return (TClrType)Convert.ChangeType(value, typeof(TClrType));
		}

		public string Write(object value)
		{
			return Convert.ToString(value);
		}
	}

	public class DateFieldProvider : DefaultFieldProvider<DateField, DateTime>
	{
	}

	public class TimeFieldProvider : DefaultFieldProvider<TimeField, TimeSpan>
	{
		public override TimeSpan ReadInternal(string value)
		{
			if (DateTime.TryParse(value, out var dateTime))
			{
				return new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
			}

			return base.ReadInternal(value);
		}
	}
}
