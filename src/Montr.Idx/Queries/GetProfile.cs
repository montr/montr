using System.Security.Claims;
using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetProfile : IRequest<Profile>
	{
		public ClaimsPrincipal User { get; set; }
	}
}
