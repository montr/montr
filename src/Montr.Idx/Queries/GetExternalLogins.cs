using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Montr.Idx.Queries
{
	public class GetExternalLogins : IRequest<IList<UserLoginInfo>>
	{
		public ClaimsPrincipal User { get; set; }
	}
}
