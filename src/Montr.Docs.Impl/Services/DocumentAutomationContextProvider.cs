﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Impl.Services
{
	public class DocumentAutomationContextProvider : IAutomationContextProvider
	{
		private readonly IRepository<Document> _repository;

		public DocumentAutomationContextProvider(IRepository<Document> repository)
		{
			_repository = repository;
		}

		public async Task<object> GetEntity(AutomationContext context, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentSearchRequest
			{
				Uid = context.EntityUid,
				IncludeFields = true
			}, cancellationToken);

			return result.Rows.Single();
		}

		public async Task<IList<FieldMetadata>> GetFields(AutomationContext context, CancellationToken cancellationToken)
		{
			// todo: combine document fields + fields from document questionaire
			var entityType = typeof(Document);

			var fields = entityType
				.GetProperties()
				.Select(x => new TextField { Key = x.Name, Name = x.Name })
				.Cast<FieldMetadata>()
				.ToList();

			return await Task.FromResult(fields);
		}
	}
}
