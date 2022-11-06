using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Kompany.Registration.Services.Implementations
{
	public class CompanyRequestValidationHelper
	{
		private readonly IRepository<Document> _documentRepository;

		public CompanyRequestValidationHelper(IRepository<Document> documentRepository)
		{
			_documentRepository = documentRepository;
		}

		public async Task EnsureCreatedByCurrentUser(Guid documentUid, Guid? userUid, CancellationToken cancellationToken)
		{
			var searchRequest = new DocumentSearchRequest { Uid = documentUid };

			// todo: remove double loading document - in ChangeStatus it is loaded again
			var searchResult = await _documentRepository.Search(searchRequest, cancellationToken);

			var document = searchResult.Rows.Single();

			if (document.CreatedBy == null || document.CreatedBy != userUid)
			{
				throw new InvalidOperationException("Document is not created by current user.");
			}
		}
	}
}
