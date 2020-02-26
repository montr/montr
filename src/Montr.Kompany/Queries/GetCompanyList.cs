using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Queries
{
	public class GetCompanyList : CompanySearchRequest, IRequest<SearchResult<Company>>
	{
	}
}
