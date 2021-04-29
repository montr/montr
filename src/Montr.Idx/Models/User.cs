using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class User : Classifier
	{
		public new static readonly string TypeCode = nameof(User).ToLower();

		private string DebuggerDisplay => $"{UserName}";

		public User()
		{
			Type = TypeCode;
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string ConcurrencyStamp { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new()
			{
				Item = new ClassifierType
				{
					Code = TypeCode,
					Name = "Users",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
					new TextField { Key = "userName", Name = "Username", Required = true, DisplayOrder = 30, System = true },
					new TextField { Key = "firstName", Name = "First Name", Required = true, DisplayOrder = 40, System = true },
					new TextField { Key = "lastName", Name = "Last Name", Required = true, DisplayOrder = 50, System = true },
					new TextField { Key = "email", Name = "Email", Required = true, DisplayOrder = 60, System = true },
					new TextField { Key = "phoneNumber", Name = "Phone", Required = true, DisplayOrder = 70, System = true }
				}
			};
		}
	}
}
