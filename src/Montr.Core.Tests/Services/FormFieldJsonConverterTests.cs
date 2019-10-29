using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Metadata.Tests.Services
{
	[TestClass]
	public class FormFieldJsonConverterTests
	{
		[TestMethod]
		public void Write_FieldsArrayWithConverter_ShouldSerializeAllProperties()
		{
			// arrange
			var options = new JsonSerializerOptions
			{
				Converters = { new FormFieldJsonConverter() }
			};

			var fields = GetTestFields();

			// act
			var result = JsonSerializer.Serialize(fields, options);

			// assert
			Assert.IsNotNull(result);

			AssertAllPropertiesSerialized(result, fields);
		}

		[TestMethod]
		public void Write_FieldsArrayWithoutConverter_ShouldThrow()
		{
			// arrange
			var options = new JsonSerializerOptions();

			var fields = GetTestFields();

			// act
			var result = JsonSerializer.Serialize(fields, options);

			// assert
			Assert.IsNotNull(result);

			Assert.ThrowsException<AssertFailedException>(() => AssertAllPropertiesSerialized(result, fields));
		}

		private static void AssertAllPropertiesSerialized(string result, IReadOnlyList<FormField> fields)
		{
			var json = JsonDocument.Parse(result);

			Assert.AreEqual(fields.Count, json.RootElement.GetArrayLength());

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
						else
						{
							Assert.AreEqual(expected, JsonSerializer.Deserialize(actual, property.PropertyType));
						}
					}
					else
					{
						Assert.Fail($"Property {propertyName} of field type {field.GetType()} should be serialized.");
					}
				}
			}
		}

		private static FormField[] GetTestFields()
		{
			return new FormField[]
			{
				new StringField
				{
					Key = "Key 1",
					Name = "Name 1",
					Description = "Description 1",
					Placeholder = "Placeholder 1",
					Multiple = true,
					Readonly = true,
					Required = true,
					Autosize = true
				},

				new TextAreaField
				{
					Key = "Key 2",
					Name = "Name 2",
					Description = "Description 3",
					Placeholder = "Placeholder 3",
					Multiple = true,
					Readonly = true,
					Required = true,
					Autosize = true,
					Rows = 15
				},

				new PasswordField
				{
					Key = "Key 3",
					Name = "Name 3",
					Description = "Description 3",
					Placeholder = "Placeholder 3",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				new NumberField
				{
					Key = "Key 4",
					Name = "Name 4",
					Description = "Description 4",
					Placeholder = "Placeholder 4",
					Multiple = true,
					Readonly = true,
					Required = true,
					Min = 12,
					Max = 42
				},

				new DecimalField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Min = 2,
					Max = 22,
					Precision = 4
				},

				new DateField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				new TimeField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				new DateTimeField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},

				/*new SelectField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true
				},*/

				new SelectField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					Options = new []
					{
						new SelectFieldOption { Name = "Option 1", Value = "val_1" }, 
						new SelectFieldOption { Name = "Option 2", Value = "val_2" }, 
						new SelectFieldOption { Name = "Option 3", Value = "val_3" }, 
					}
				},

				new ClassifierGroupField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					TypeCode = "TypeCode 1",
					TreeCode = "TreeCode 1"
				},

				new ClassifierField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true,
					TypeCode = "TypeCode 2",
				},

				new FileField
				{
					Key = "Key 5",
					Name = "Name 5",
					Description = "Description 5",
					Placeholder = "Placeholder 5",
					Multiple = true,
					Readonly = true,
					Required = true
				}
			};
		}
	}
}
