using System;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl.Services
{
	// todo: rewrite all this sh*t
	public class DefaultFieldProvider<TFieldType, TClrType> : IFieldProvider
	{
		public Type FieldType => typeof(TFieldType);

		public virtual bool Validate(object value, out object parsed, out string[] errors)
		{
			try
			{
				parsed = ValidateInternal(value);
				errors = null;

				return true;
			}
			catch (Exception e)
			{
				parsed = null;
				errors = new[] { e.Message };

				return false;
			}
		}

		public virtual object ValidateInternal(object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is string stringValue)
			{
				return ReadFromStorage(stringValue);
			}

			// todo: check clr type
			return (TClrType)value;
		}

		public object ReadFromStorage(string value)
		{
			return value != null ? ReadInternal(value) : (object)null;
		}

		public virtual TClrType ReadInternal(string value)
		{
			return (TClrType)Convert.ChangeType(value, typeof(TClrType));
		}

		public string WriteToStorage(object value)
		{
			return value != null ? WriteInternal((TClrType)value) : null;
		}

		public virtual string WriteInternal(TClrType value)
		{
			return Convert.ToString(value);
		}
	}

	// todo: add support of utc and localization
	public class DateFieldProvider : DefaultFieldProvider<DateField, DateTime>
	{
		public override string WriteInternal(DateTime value)
		{
			return value.ToString("o");
		}
	}

	// todo: add support of utc and localization
	public class TimeFieldProvider : DefaultFieldProvider<TimeField, TimeSpan>
	{
		public override object ValidateInternal(object value)
		{
			if (value is DateTime dateTime)
			{
				return dateTime.TimeOfDay;
			}

			return base.ValidateInternal(value);
		}

		public override TimeSpan ReadInternal(string value)
		{
			if (DateTime.TryParse(value, out var dateTime))
			{
				return dateTime.TimeOfDay;
			}

			return base.ReadInternal(value);
		}
	}
}
