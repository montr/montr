using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class LinkLoginCallback : IRequest<ApiResult>
	{
		public ClaimsPrincipal User { get; set; }
	}
}
