using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class LogoutHandler : IRequestHandler<Logout, ApiResult>
	{
		private readonly ILogger<ConfirmEmailHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public LogoutHandler(
			ILogger<ConfirmEmailHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(Logout request, CancellationToken cancellationToken)
		{
			await _signInManager.SignOutAsync();

			_logger.LogInformation("User logged out.");

			return new ApiResult();
		}
	}
}
