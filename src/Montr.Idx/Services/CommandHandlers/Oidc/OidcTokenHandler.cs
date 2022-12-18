using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;

namespace Montr.Idx.Services.CommandHandlers.Oidc
{
	public class OidcTokenHandler : IRequestHandler<OidcToken, IActionResult>
	{
		private readonly IOidcServer _oidcServer;

		public OidcTokenHandler(IOidcServer oidcServer)
		{
			_oidcServer = oidcServer;
		}

		public async Task<IActionResult> Handle(OidcToken request, CancellationToken cancellationToken)
		{
			return await _oidcServer.Token(request, cancellationToken);
		}
	}
}
