using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.Idx.Impl.Services
{
	public class RegisterClassifierTypeStartupTask : IStartupTask
	{
		private readonly IClassifierTypeRegistrator _classifierTypeRegistrator;

		public RegisterClassifierTypeStartupTask(IClassifierTypeRegistrator classifierTypeRegistrator)
		{
			_classifierTypeRegistrator = classifierTypeRegistrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var command in GetCommands())
			{
				await _classifierTypeRegistrator.Register(command.Item, command.Fields, cancellationToken);
			}
		}

		protected  static IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = Role.TypeCode,
					Name = "Роли",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return GetUserType();
		}

		public static RegisterClassifierType GetUserType()
		{
			return new()
			{
				Item = new ClassifierType
				{
					Code = User.TypeCode,
					Name = "Пользователи",
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
