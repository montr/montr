using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Services.QueryHandlers
{
	public class GetTaskSearchCriteriaHandler : IRequestHandler<GetTaskSearchMetadata, DataView>
	{
		private readonly IDataAnnotationMetadataProvider _dataAnnotationMetadataProvider;

		public GetTaskSearchCriteriaHandler(IDataAnnotationMetadataProvider dataAnnotationMetadataProvider)
		{
			_dataAnnotationMetadataProvider = dataAnnotationMetadataProvider;
		}

		public async Task<DataView> Handle(GetTaskSearchMetadata request, CancellationToken cancellationToken)
		{
			var fields = await _dataAnnotationMetadataProvider.GetMetadata(typeof(TaskSearchRequest), cancellationToken);

			var columns = new List<DataColumn>
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

			var result = new DataView
			{
				Fields = fields,
				Columns = columns
			};
			
			return result;
		}
	}
}
