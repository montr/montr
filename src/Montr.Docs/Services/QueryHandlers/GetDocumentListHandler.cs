using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Services.QueryHandlers
{
	public class GetDocumentListHandler : IRequestHandler<GetDocumentList, SearchResult<Document>>
	{
		private readonly IAuthorizationService _authorizationService;
		private readonly IRepository<Document> _documentRepository;

		public GetDocumentListHandler(IAuthorizationService authorizationService, IRepository<Document> documentRepository)
		{
			_authorizationService = authorizationService;
			_documentRepository = documentRepository;
		}

		public async Task<SearchResult<Document>> Handle(GetDocumentList request, CancellationToken cancellationToken)
		{
			var allowViewAllDocuments = await _authorizationService.AuthorizePermission(request.Principal,
				Permission.GetCode(typeof(Permissions.ViewAllDocuments)));

			request.FilterByUser = allowViewAllDocuments.Succeeded == false;

			return await _documentRepository.Search(request, cancellationToken);
		}
	}
}
