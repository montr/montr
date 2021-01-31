using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers.Oidc
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
