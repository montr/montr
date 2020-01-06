using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Montr.Core.Models;

namespace Montr.Metadata.Impl.Services
{
	/// <summary>
	/// https://github.com/dotnet/corefx/issues/38650
	/// https://github.com/dotnet/corefx/issues/39031
	/// https://github.com/dotnet/corefx/issues/37787
	/// https://github.com/dotnet/corefx/blob/fbb8b5c4b9566ba81596aa35f42b71bbda601528/src/System.Text.Json/tests/Serialization/CustomConverterTests.Polymorphic.cs#L11-L147
	/// </summary>
	public class FieldMetadataJsonConverter /*WithTypeDiscriminator*/ : JsonConverter<FieldMetadata>
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return typeof(FieldMetadata).IsAssignableFrom(typeToConvert);
		}

		public override FieldMetadata Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}

		public override void Write(Utf8JsonWriter writer, FieldMetadata value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();

			writer.WriteString("type", value.Type);
			writer.WriteString("key", value.Key);
			writer.WriteString("name", value.Name);
			writer.WriteString("description", value.Description);
			writer.WriteString("placeholder", value.Placeholder);
			writer.WriteBoolean("multiple", value.Multiple);
			writer.WriteBoolean("readonly", value.Readonly);
			writer.WriteBoolean("required", value.Required);

			/*if (value is TextField stringField)
			{
				// writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.TextField);
			}

			if (value is TextAreaField textAreaField)
			{
				if (textAreaField.Rows.HasValue) writer.WriteNumber("rows", textAreaField.Rows.Value);
			}

			if (value is NumberField numberField)
			{
				if (numberField.Min.HasValue) writer.WriteNumber("min", numberField.Min.Value);
				if (numberField.Max.HasValue) writer.WriteNumber("max", numberField.Max.Value);
			}

			if (value is DecimalField decimalField)
			{
				if (decimalField.Min.HasValue) writer.WriteNumber("min", decimalField.Min.Value);
				if (decimalField.Max.HasValue) writer.WriteNumber("max", decimalField.Max.Value);
				if (decimalField.Precision.HasValue) writer.WriteNumber("precision", decimalField.Precision.Value);
			}

			if (value is SelectField selectField)
			{
				if (selectField.Options != null)
				{
					writer.WriteStartArray("options");

					foreach (var option in selectField.Options)
					{
						writer.WriteStartObject();

						writer.WriteString("name", option.Name);
						writer.WriteString("value", option.Value);

						writer.WriteEndObject();
					}

					writer.WriteEndArray();
				}
			}

			if (value is ClassifierGroupField classifiergroupField)
			{
				writer.WriteString("typeCode", classifiergroupField.TypeCode);
				writer.WriteString("treeCode", classifiergroupField.TreeCode);
			}

			if (value is ClassifierField classifierField)
			{
				writer.WriteString("typeCode", classifierField.TypeCode);
			}*/

			writer.WriteEndObject();
		}
	}
}
