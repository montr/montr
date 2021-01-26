using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands
{
	public class OidcAuthorize : IRequest<IActionResult>
	{
		public Controller Controller { get; set; }
	}
}
