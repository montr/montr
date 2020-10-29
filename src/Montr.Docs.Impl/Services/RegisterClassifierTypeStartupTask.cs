using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.Docs.Impl.Services
{
	public class RegisterClassifierTypeStartupTask : AbstractRegisterClassifierTypeStartupTask
	{
		public RegisterClassifierTypeStartupTask(ILogger<RegisterClassifierTypeStartupTask> logger, IMediator mediator) : base(logger, mediator)
		{
		}

		protected override IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "questionnaire",
					Name = "Questionnaire",
					Description = "Анкеты",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

			yield return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = "process",
					Name = "Process",
					Description = "Процессы",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 10 } },
				}
			};

		}
	}
}
