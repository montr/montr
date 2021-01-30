using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;

namespace Montr.Idx.Services
{
	public interface IOidcServer
	{
		Task<IActionResult> Authorize(OidcAuthorize request, CancellationToken cancellationToken);

		Task<IActionResult> Logout(OidcLogout request, CancellationToken cancellationToken);
	}
}
