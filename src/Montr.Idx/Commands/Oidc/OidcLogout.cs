using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands.Oidc
{
	public class OidcLogout : IRequest<IActionResult>
	{
	}
}
