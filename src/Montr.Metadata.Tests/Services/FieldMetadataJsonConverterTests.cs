using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Montr.Core.Services.Implementations;
using Montr.Metadata.Models;
using NUnit.Framework;

namespace Montr.Metadata.Tests.Services
{
	[TestFixture]
	public class FieldMetadataJsonConverterTests
	{
		[Test]
		public void Write_FieldsArrayWithConverter_ShouldSerializeAllProperties()
		{
			// arrange
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				WriteIndented = false,
				// Converters = { new FieldMetadataJsonConverter() }
				Converters = { new PolymorphicWriteOnlyJsonConverter<FieldMetadata>() }
			};

			var fields = GetTestFields();

			// act
			var result = JsonSerializer.Serialize(fields, options);

			// assert
			Assert.That(result, Is.Not.Null);

			AssertAllPropertiesSerialized(result, fields);
		}

		[Test]
		public void Write_FieldsArrayWithoutConverter_ShouldThrow()
		{
			// arrange
			var options = new JsonSerializerOptions();

			var fields = GetTestFields();

			// act
			var result = JsonSerializer.Serialize(fields, options);

			// assert
			Assert.That(result, Is.Not.Null);

			Assert.Throws<AssertionException>(() => AssertAllPropertiesSerialized(result, fields));
		}

		private static void AssertAllPropertiesSerialized(string result, IReadOnlyList<FieldMetadata> fields)
		{
			var json = JsonDocument.Parse(result);

			Assert.That(json.RootElement.GetArrayLength(), Is.EqualTo(fields.Count));

			for (var i = 0; i < fields.Count; i++)
			{
				var field = fields[i];

				var jsonField = json.RootElement[i];

				foreach (var property in field.GetType().GetProperties())
				{
					var propertyName = property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);

					if (jsonField.TryGetProperty(propertyName, out var jsonProperty))
					{
						var expected = property.GetValue(field);
						var actual = jsonProperty.GetRawText();

						Console.WriteLine($"{propertyName} \t {expected} \t {actual}");

						if (jsonProperty.ValueKind == JsonValueKind.Array)
						{
							// todo: assert arrays
						}
						else if (jsonProperty.ValueKind == JsonValueKind.Object)
						{
							// todo: assert objects
						}
						else
						{
							Assert.That(JsonSerializer.Deserialize(actual, property.PropertyType), Is.EqualTo(expected));
						}
					}
					else
					{
						Assert.Fail($"Property {propertyName} of field type {field.GetType()} should be serialized.");
					}
				}
			}
		}

		private static FieldMetadata[] GetTestFields()
		{
			return new FieldMetadata[]
			{
				new TextField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 1",
					Name = "Name 1",
					Description = "Description 1",
					Placeholder = "Placeholder 1",
					Tooltip = "Tooltip 1",
					Icon = "Icon 1",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				new TextAreaField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 2",
					Name = "Name 2",
					Description = "Description 2",
					Placeholder = "Placeholder 2",
					Tooltip = "Tooltip 2",
					Icon = "Icon 2",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props = new TextAreaField.Properties { Rows = 15 }
				},

				new PasswordField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 3",
					Name = "Name 3",
					Description = "Description 3",
					Placeholder = "Placeholder 3",
					Tooltip = "Tooltip 3",
					Icon = "Icon 3",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				new NumberField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 4",
					Name = "Name 4",
					Description = "Description 4",
					Placeholder = "Placeholder 4",
					Tooltip = "Tooltip 4",
					Icon = "Icon 4",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						Min = 12,
						Max = 42
					}
				},

				new DecimalField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Tooltip = "Tooltip 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						Min = 2,
						Max = 22,
						Precision = 4
					}
				},

				new DateField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Tooltip = "Tooltip 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						IncludeTime = true
					}
				},

				new TimeField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Tooltip = "Tooltip 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				/*new SelectField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},*/

				new SelectField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Tooltip = "Tooltip 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						Options = new []
						{
							new SelectFieldOption { Name = "Option 1", Value = "val_1" },
							new SelectFieldOption { Name = "Option 2", Value = "val_2" },
							new SelectFieldOption { Name = "Option 3", Value = "val_3" },
						}
					}
				},

				/*new ClassifierGroupField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						TypeCode = "TypeCode 1",
						TreeCode = "TreeCode 1"
					}
				},

				new ClassifierField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Props =
					{
						TypeCode = "TypeCode 2"
					}
				},*/

				new FileField
				{
					Uid = Guid.NewGuid(),
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Tooltip = "Tooltip 5",
					Icon = "Icon 5",
					Multiple = true,
					Readonly = true,
					Required = true
				}
			};
		}
	}
}
