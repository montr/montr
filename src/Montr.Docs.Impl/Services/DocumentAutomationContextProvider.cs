using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Docs.Impl.Services
{
	public class DocumentAutomationContextProvider : IAutomationContextProvider
	{
		private readonly IRepository<Document> _documentRepository;
		private readonly IFieldProviderRegistry _fieldProviderRegistry;

		public DocumentAutomationContextProvider(
			IRepository<Document> documentRepository,
			IFieldProviderRegistry fieldProviderRegistry)
		{
			_documentRepository = documentRepository;
			_fieldProviderRegistry = fieldProviderRegistry;
		}

		public async Task<object> GetEntity(AutomationContext context, CancellationToken cancellationToken)
		{
			var result = await _documentRepository.Search(new DocumentSearchRequest
			{
				Uid = context.EntityUid,
				IncludeFields = true
			}, cancellationToken);

			return result.Rows.Single();
		}

		public async Task<IList<FieldMetadata>> GetFields(AutomationContext context, CancellationToken cancellationToken)
		{
			// todo: (?) combine document fields + fields from document questionnaire
			var entityType = typeof(Document);

			var result = new List<FieldMetadata>();

			foreach (var property in entityType.GetProperties())
			{
				var attribute = (FieldAttribute)property.GetCustomAttributes(typeof(FieldAttribute), false).FirstOrDefault();

				if (attribute != null)
				{
					var fieldTypeProvider = _fieldProviderRegistry.GetFieldTypeProvider(attribute.TypeCode);

					var fieldMetadata = (FieldMetadata) Activator.CreateInstance(fieldTypeProvider.FieldType);

					if (fieldMetadata != null)
					{
						fieldMetadata.Key = fieldMetadata.Name = property.Name;

						result.Add(fieldMetadata);
					}
				}
			}

			return await Task.FromResult(result);
		}
	}
}
