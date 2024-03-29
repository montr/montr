﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Idx.Commands.Oidc;

namespace Montr.Idx.Services.CommandHandlers.Oidc
{
	public class OidcUserInfoHandler : IRequestHandler<OidcUserInfo, IActionResult>
	{
		private readonly IOidcServer _oidcServer;

		public OidcUserInfoHandler(IOidcServer oidcServer)
		{
			_oidcServer = oidcServer;
		}

		public async Task<IActionResult> Handle(OidcUserInfo request, CancellationToken cancellationToken)
		{
			return await _oidcServer.UserInfo(request, cancellationToken);
		}
	}
}
