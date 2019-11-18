using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetExternalLogins : IRequest<IList<ExternalLoginModel>>
	{
		public ClaimsPrincipal User { get; set; }
	}
}
