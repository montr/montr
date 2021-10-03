using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.Tasks.Services
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
			foreach (var classifierType in GetClassifierTypes())
			{
				await _classifierTypeRegistrator.Register(classifierType.Item, classifierType.Fields, cancellationToken);
			}
		}

		protected static IEnumerable<RegisterClassifierType> GetClassifierTypes()
		{
			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.TaskType,
					Name = "Task types",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
				}
			};
		}
	}
}
