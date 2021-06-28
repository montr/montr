using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Kompany.Registration.Queries;

namespace Montr.Kompany.Registration.Impl.QueryHandlers
{
	public class GetCompanyRegistrationRequestListHandler : IRequestHandler<GetCompanyRegistrationRequestList, ICollection<Document>>
	{
		private readonly IDocumentService _documentService;

		public GetCompanyRegistrationRequestListHandler(IDocumentService documentService)
		{
			_documentService = documentService;
		}

		public async Task<ICollection<Document>> Handle(GetCompanyRegistrationRequestList request, CancellationToken cancellationToken)
		{
			var documents = await _documentService.Search(new DocumentSearchRequest
			{
				UserUid = request.UserUid,
				SkipPaging = true
			}, cancellationToken);

			return documents.Rows;
		}
	}
}
