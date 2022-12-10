using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands.Oidc
{
	public class OidcToken : IRequest<IActionResult>
	{
	}
}
