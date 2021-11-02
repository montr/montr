using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Montr.Tasks.Models;

namespace Montr.Tasks.Services
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
			_registrator.Register(ViewCode.TaskList, _ => new DataView
			{
				Columns = new List<DataColumn>
				{
					new() { Key = "code", Name = "Code", Width = 50, Sortable = true, UrlProperty = "url" },
					new() { Key = "name", Name = "Name", Width = 250, Sortable = true, UrlProperty = "url" },
					new() { Key = "statusCode", Template = "status", Name = "Status", Sortable = true, Width = 30, UrlProperty = "url" },
					new() { Key = "taskTypeName", Name = "Type", Width = 200 },
					new() { Key = "assigneeName", Name = "Assignee", Width = 150 },
					new() { Key = "startDateUtc", Name = "Start Date", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "dueDateUtc", Name = "Due Date", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "createdAtUtc", Name = "Created", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "modifiedAtUtc", Name = "Modified", Type = "datetime", Sortable = true, Width = 100 }
				}
			});

			_registrator.Register(ViewCode.TaskForm, _ => new DataView
			{
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "number", Name = "Number", Required = true },
					new TextField { Key = "name", Name = "Name", Required = true },
				}
			});

			return Task.CompletedTask;
		}
	}
}
