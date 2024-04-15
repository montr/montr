using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Kompany.Registration.Queries;

namespace Montr.Kompany.Registration.Services.QueryHandlers
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
			ICollection<Document> result = null;

			if (request?.UserUid != null)
			{
				var documents = await _documentRepository.Search(new DocumentSearchRequest
				{
					FilterByUser = true,
					UserUid = request.UserUid,
					SkipPaging = true
				}, cancellationToken);

				result = documents.Rows;
			}

			return result;
		}
	}
}
