using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetAuthSchemesQueryHandler : IRequestHandler<GetAuthSchemesQuery, IList<AuthScheme>>
	{
		private readonly ILogger<GetAuthSchemesQueryHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public GetAuthSchemesQueryHandler(
			ILogger<GetAuthSchemesQueryHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<IList<AuthScheme>> Handle(GetAuthSchemesQuery request, CancellationToken cancellationToken)
		{
			var schemes = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			return schemes.Select(x => new AuthScheme { Name = x.Name, DisplayName = x.DisplayName }).ToList();
		}
	}
}
