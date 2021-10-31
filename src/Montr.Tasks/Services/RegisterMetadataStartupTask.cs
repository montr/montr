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
					new() { Key = "code", Name = "Code", Width = 450, Sortable = true },
					new() { Key = "name", Name = "Name", Width = 450, Sortable = true },
					new() { Key = "statusCode", Template = "status", Name = "Status", Sortable = true, Width = 30 },
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
