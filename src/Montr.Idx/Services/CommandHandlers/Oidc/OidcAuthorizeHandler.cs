using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;

namespace Montr.Idx.Services.CommandHandlers.Oidc
{
	public class OidcAuthorizeHandler : IRequestHandler<OidcAuthorize, IActionResult>
	{
		private readonly IOidcServer _oidcServer;

		public OidcAuthorizeHandler(IOidcServer oidcServer)
		{
			_oidcServer = oidcServer;
		}

		public async Task<IActionResult> Handle(OidcAuthorize request, CancellationToken cancellationToken)
		{
			return await _oidcServer.Authorize(request, cancellationToken);
		}
	}
}
