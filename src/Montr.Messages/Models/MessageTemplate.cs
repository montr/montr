using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Messages.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class MessageTemplate : Classifier
	{
		public new static readonly string TypeCode = "message_template";

		public MessageTemplate()
		{
			Type = TypeCode;
		}

		private string DebuggerDisplay => $"{Code}";

		public string Subject { get; set; }

		public string Body { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new()
			{
				Item = new ClassifierType
				{
					Code = TypeCode,
					Name = "Message templates",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Code", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Name", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 5 } },
					new TextField { Key = "subject", Name = "Subject", DisplayOrder = 30, System = true },
					// todo: make markdown/html editor control
					new TextAreaField { Key = "body", Name = "Body", DisplayOrder = 40, System = true, Props = new TextAreaField.Properties { Rows = 40 } }
				}
			};
		}
	}
}
