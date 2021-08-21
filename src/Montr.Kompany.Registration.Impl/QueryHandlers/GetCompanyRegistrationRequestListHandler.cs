using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Kompany.Registration.Queries;

namespace Montr.Kompany.Registration.Impl.QueryHandlers
{
	public class GetCompanyRegistrationRequestListHandler : IRequestHandler<GetCompanyRegistrationRequestList, ICollection<Document>>
	{
		private readonly IRepository<Document> _documentRepository;

		public GetCompanyRegistrationRequestListHandler(IRepository<Document> documentRepository)
		{
			_documentRepository = documentRepository;
		}

		public async Task<ICollection<Document>> Handle(GetCompanyRegistrationRequestList request, CancellationToken cancellationToken)
		{
			var documents = await _documentRepository.Search(new DocumentSearchRequest
			{
				UserUid = request.UserUid,
				SkipPaging = true
			}, cancellationToken);

			return documents.Rows;
		}
	}
}
