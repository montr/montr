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
	public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResult>
	{
		private readonly ILogger<ConfirmEmailCommandHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public LogoutCommandHandler(
			ILogger<ConfirmEmailCommandHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
		{
			await _signInManager.SignOutAsync();

			_logger.LogInformation("User logged out.");

			return new ApiResult();
		}
	}
}
