﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Idx.Entities;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Services.QueryHandlers
{
	public class GetAuthSchemesHandler : IRequestHandler<GetAuthSchemes, IList<AuthScheme>>
	{
		private readonly SignInManager<DbUser> _signInManager;

		public GetAuthSchemesHandler(SignInManager<DbUser> signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<IList<AuthScheme>> Handle(GetAuthSchemes request, CancellationToken cancellationToken)
		{
			var schemes = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			return schemes.Select(x => new AuthScheme
			{
				Name = x.Name,
				DisplayName = x.DisplayName,
				Icon = x.Name.ToLower() == "microsoft" ? "windows" : x.Name.ToLower()
			}).ToList();
		}
	}
}
