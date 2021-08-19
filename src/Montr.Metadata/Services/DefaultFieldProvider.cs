using System;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Services;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	// todo: rewrite all this sh*t
	public class DefaultFieldProvider<TFieldType, TClrType> : IFieldProvider
	{
		// ReSharper disable once StaticMemberInGenericType
		protected static readonly string PropsPrefix = ExpressionHelper.GetMemberName<TextAreaField>(x => x.Props).ToLowerInvariant();

		public virtual FieldPurpose Purpose => FieldPurpose.Content;

		public Type FieldType => typeof(TFieldType);

		public virtual IList<FieldMetadata> GetMetadata()
		{
			var result = new List<FieldMetadata>
			{
				new NumberField { Key = "displayOrder", Name = "#", Required = true, Props = { Min = 0, Max = 256 } },
				new TextField { Key = "key", Name = "Код", Required = true },
				new TextField { Key = "name", Name = "Наименование", Required = true },
				new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 2 } }
			};

			if (Purpose == FieldPurpose.Content)
			{
				result.AddRange(new List<FieldMetadata>
				{
					new TextField { Key = "placeholder", Name = "Placeholder" },
					new TextField { Key = "icon", Name = "Icon" },
					// new BooleanField { Key = "readonly", Name = "Readonly" },
					new BooleanField { Key = "required", Name = "Required" }
				});
			}

			return result;
		}

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
			return value != null ? ReadInternal(value) : null;
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

	public enum FieldPurpose
	{
		Information,
		Content
	}

	public class SectionFieldProvider : DefaultFieldProvider<SectionField, string>
	{
		public override FieldPurpose Purpose => FieldPurpose.Information;
	}

	public class TextAreaFieldProvider : DefaultFieldProvider<TextAreaField, string>
	{
		public override IList<FieldMetadata> GetMetadata()
		{
			var baseFields = base.GetMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new NumberField { Key = PropsPrefix + ".rows", Name = "Количество строк", Props = { Min = 1, Max = byte.MaxValue } }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}

	public class NumberFieldProvider : DefaultFieldProvider<NumberField, long>
	{
		public override IList<FieldMetadata> GetMetadata()
		{
			var baseFields = base.GetMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new NumberField { Key = PropsPrefix + ".min", Name = "Минимум", Props = { Min = long.MinValue, Max = long.MaxValue } },
				new NumberField { Key = PropsPrefix + ".max", Name = "Максимум", Props = { Min = long.MinValue, Max = long.MaxValue } }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}

	public class DecimalFieldProvider : DefaultFieldProvider<DecimalField, decimal>
	{
		public override IList<FieldMetadata> GetMetadata()
		{
			var baseFields = base.GetMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new NumberField { Key = PropsPrefix + ".min", Name = "Минимум", Props = { Min = decimal.MinValue, Max = decimal.MaxValue } },
				new NumberField { Key = PropsPrefix + ".max", Name = "Максимум", Props = { Min = decimal.MinValue, Max = decimal.MaxValue } },
				new NumberField { Key = PropsPrefix + ".precision", Name = "Точность", Description = "Количество знаков после запятой", Props = { Min = 0, Max = 5 } }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}

	// todo: add support of utc and localization
	public class DateFieldProvider : DefaultFieldProvider<DateField, DateTime>
	{
		public override IList<FieldMetadata> GetMetadata()
		{
			var baseFields = base.GetMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new BooleanField { Key = PropsPrefix + ".includeTime", Name = "Include Time" }
			};

			return baseFields.Union(additionalFields).ToList();
		}

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

	public class SelectFieldProvider : DefaultFieldProvider<SelectField, string>
	{
		public override IList<FieldMetadata> GetMetadata()
		{
			var baseFields = base.GetMetadata();

			var additionalFields = new List<FieldMetadata>
			{
				new DesignSelectOptionsField { Key = PropsPrefix + ".options", Required = true, Name = "Options" }
			};

			return baseFields.Union(additionalFields).ToList();
		}
	}
}
