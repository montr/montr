using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Automate.Services
{
	public class RegisterMetadataStartupTask : IStartupTask
	{
		private readonly IMetadataRegistrator _registrator;

		public RegisterMetadataStartupTask(IMetadataRegistrator registrator)
		{
			_registrator = registrator;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_registrator.Register("Automation/Edit", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new NumberField { Key = "displayOrder", Name = "#", Required = true, Props = { Min = 0, Max = 256 } },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 1 } },
					new AutomationConditionListField { Key = "conditions", Name = "Conditions" },
					new AutomationActionListField { Key = "actions", Name = "Actions" },
				}
			});

			return Task.CompletedTask;
		}
	}
}
