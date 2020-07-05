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
			_registrator.Register("Automation/Grid", viewId => new DataView
			{
				Columns = new List<DataColumn>
				{
					new DataColumn { Key = "displayOrder", Name = "#", Width = 10, Sortable = true },
					new DataColumn { Key = "name", Name = "Name", Width = 450, Sortable = true },
					// new DataColumn { Key = "description", Name = "Description", Width = 150 },
					new DataColumn { Key = "active", Name = "Active", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
					new DataColumn { Key = "system", Name = "System", Width = 10, Sortable = true, Type = BooleanField.TypeCode },
				}
			});

			_registrator.Register("Automation/Edit", viewId => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new NumberField { Key = "displayOrder", Name = "#", Required = true, Props = { Min = 0, Max = 256 } },
					new TextField { Key = "name", Name = "Наименование", Required = true },
					new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 1 } },
					new AutomationConditionListField { Key = "conditions", Name = "Conditions" },
					new AutomationActionListField { Key = "actions", Name = "Actions" }
				}
			});

			return Task.CompletedTask;
		}
	}
}
