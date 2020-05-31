using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Docs.Impl.Services
{
	public class DocumentTypeNameResolver : IEntityNameResolver
	{
		private readonly IRepository<DocumentType> _repository;

		public DocumentTypeNameResolver(IRepository<DocumentType> repository)
		{
			_repository = repository;
		}

		public async Task<string> Resolve(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentTypeSearchRequest { Uid = entityUid }, cancellationToken);

			var documentType = result?.Rows.SingleOrDefault();

			return documentType?.Name;

			/*if (entityUid == Process.CompanyRegistrationRequest)
			{
				return "Процесс регистрации (по умолчанию)";
			}*/
		}
	}
}
