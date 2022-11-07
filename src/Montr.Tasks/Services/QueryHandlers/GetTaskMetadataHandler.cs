using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Tasks.Models;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Services.QueryHandlers
{
	public class GetTaskMetadataHandler : IRequestHandler<GetTaskMetadata, DataView>
	{
		private readonly IRepository<TaskModel> _taskRepository;
		private readonly IConfigurationProvider _configurationProvider;

		public GetTaskMetadataHandler(IRepository<TaskModel> taskRepository, IConfigurationProvider configurationProvider)
		{
			_taskRepository = taskRepository;
			_configurationProvider = configurationProvider;
		}

		public async Task<DataView> Handle(GetTaskMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			if (request.ViewId == ViewCode.TaskList)
			{
				result.Columns = new List<DataColumn>
				{
					new() { Key = "number", Name = "Number", Width = 50, Sortable = true, UrlProperty = "url" },
					new() { Key = "name", Name = "Name", Width = 250, Sortable = true, UrlProperty = "url" },
					new() { Key = "statusCode", Template = "status", Name = "Status", Sortable = true, Width = 30, UrlProperty = "url" },
					new() { Key = "taskTypeName", Name = "Type", Width = 200 },
					new() { Key = "assigneeName", Name = "Assignee", Width = 150 },
					new() { Key = "startDateUtc", Name = "Start Date", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "dueDateUtc", Name = "Due Date", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "createdAtUtc", Name = "Created", Type = "datetime", Sortable = true, Width = 100 },
					new() { Key = "modifiedAtUtc", Name = "Modified", Type = "datetime", Sortable = true, Width = 100 }
				};
			}

			if (request.ViewId == ViewCode.TaskForm)
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextField { Key = "number", Name = "Number", Required = true },
					new ClassifierField { Key = "taskTypeUid", Name = "Task type", Required = true, Props = { TypeCode = ClassifierTypeCode.TaskType } },
					new ClassifierField { Key = "assigneeUid", Name = "Assignee", Props = { TypeCode = Idx.ClassifierTypeCode.User } },
					new TextField { Key = "name", Name = "Name", Required = true },
					new TextAreaField { Key = "description", Name = "Description", Placeholder = "Description", Props = new TextAreaField.Properties { Rows = 2 } }
				};
			}

			if (request.ViewId == ViewCode.TaskPage)
			{
				var task = (await _taskRepository.Search(new TaskSearchRequest
				{
					Uid = request.TaskUid,
					SkipPaging = true
				}, cancellationToken)).Rows.SingleOrDefault();

				result.Toolbar = await _configurationProvider.GetItems<TaskModel, Button>(task, request.Principal);
				result.Panes = await _configurationProvider.GetItems<TaskModel, DataPane>(task, request.Principal);
				result.Panels = await _configurationProvider.GetItems<TaskModel, DataPanel>(task, request.Principal);
			}

			return result;
		}
	}
}
