using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers.Oidc
{
	public class OidcLogoutHandler : IRequestHandler<OidcLogout, IActionResult>
	{
		private readonly IOidcServer _oidcServer;

		public OidcLogoutHandler(IOidcServer oidcServer)
		{
			_oidcServer = oidcServer;
		}

		public async Task<IActionResult> Handle(OidcLogout request, CancellationToken cancellationToken)
		{
			return await _oidcServer.Logout(request, cancellationToken);
		}
	}
}
