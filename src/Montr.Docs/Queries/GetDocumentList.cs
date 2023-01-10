using System.Security.Claims;
using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;

namespace Montr.Docs.Queries
{
	public class GetDocumentList : DocumentSearchRequest, IRequest<SearchResult<Document>>
	{
		public ClaimsPrincipal Principal { get; set; }
	}
}
