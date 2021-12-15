using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries;

public class GetMenu : IRequest<Menu>
{
	public ClaimsPrincipal Principal { get; set; }

	public string MenuId { get; set; }
}
