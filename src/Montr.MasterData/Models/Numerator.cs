using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Montr.MasterData.Commands;
using Montr.Metadata.Models;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Numerator : Classifier
	{
		private string DebuggerDisplay => $"{Name} - {Pattern}";

		public static readonly StringComparer TagComparer = StringComparer.OrdinalIgnoreCase;

		public static readonly string DefaultPattern = "{Number}";

		public Numerator()
		{
			Type = ClassifierTypeCode.Numerator;
		}

		/// <summary>
		/// DocumentType or ClassifierType
		/// </summary>
		public string EntityTypeCode { get; set; }

		public string Pattern { get; set; }

		/// <summary>
        /// Tags used to build unique numerator keys (unique numbers in scope of these tags)
        /// todo: display in UI as checkboxes (?) - only for documents (?)
        /// </summary>
        public string[] KeyTags { get; set; }

		public NumeratorPeriodicity Periodicity { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.Numerator,
					Name = "Numerators",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					// todo: remove "Numerator/Grid" and "Numerator/Form" from RegisterClassifierMetadataStartupTask
					new TextField
					{
						Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true
					},
					new TextAreaField
					{
						Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true,
						Props = new TextAreaField.Properties { Rows = 2 }
					},
					new SelectField
					{
						Key = "entityTypeCode", Name = "EntityTypeCode", Required = true, DisplayOrder = 30, System = true,
						Props =
						{
							Options = Core.Models.EntityTypeCode
								.GetRegisteredEntityTypeCodes()
								.Select(x => new SelectFieldOption { Value = x, Name = x })
								.ToArray()
						}
					},
					new SelectField
					{
						Key = "periodicity", Name = "Periodicity", Required = true, DisplayOrder = 40, System = true,
						Props =
						{
							Options = new[]
							{
								new SelectFieldOption { Value = "None", Name = "None" },
								new SelectFieldOption { Value = "Day", Name = "Day" },
								new SelectFieldOption { Value = "Month", Name = "Month" },
								new SelectFieldOption { Value = "Quarter", Name = "Quarter" },
								new SelectFieldOption { Value = "Year", Name = "Year" },
							}
						}
					},
					new TextAreaField
					{
						Key = "pattern", Name = "Формат номера", Required = true, DisplayOrder = 50, System = true,
						Props = new TextAreaField.Properties { Rows = 4 },
						Description = "Укажите шаблон с использованием возможных подстановок, например: {DocumentType}-{Number}/{Year2}"
					}
				}
			};
		}
	}

	public enum NumeratorPeriodicity : byte
	{
		None,
		Day,
		// Week, // requires settings for start of week - Monday or Sunday
		Month,
		Quarter,
		Year
	}

	public abstract class Token
	{
	}

	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class TextToken : Token
	{
		private string DebuggerDisplay => $"Text: {Content}";

		public string Content { get; set; }
	}

	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class TagToken : Token
	{
		private string DebuggerDisplay => $"Tag: {{{Name}{(Args != null && Args.Length > 0 ? ':' + string.Join(':', Args) : string.Empty)}}}";

		public string Name { get; set; }

		public string[] Args { get; set; }
	}

	public enum TokenType : byte
	{
		Text,
		TagBegin,
		TagName,
		TagArgSeparator,
		TagArg,
		TagEnd,
		End
	}
}
