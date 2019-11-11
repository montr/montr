using System.Security.Claims;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class ChangePassword : ChangePasswordModel, IRequest<ApiResult>
	{
		public ClaimsPrincipal User { get; set; }
	}
}
