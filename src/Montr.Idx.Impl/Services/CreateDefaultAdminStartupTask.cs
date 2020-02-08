using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.Services
{
	public class CreateDefaultAdminStartupTask : IStartupTask
	{
		private readonly ILogger<CreateDefaultAdminStartupTask> _logger;
		private readonly IOptionsMonitor<Options> _optionsAccessor;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly UserManager<DbUser> _userManager;

		public CreateDefaultAdminStartupTask(
			ILogger<CreateDefaultAdminStartupTask> logger,
			IOptionsMonitor<Options> optionsAccessor,
			IDbContextFactory dbContextFactory,
			UserManager<DbUser> userManager)
		{
			_logger = logger;
			_optionsAccessor = optionsAccessor;
			_dbContextFactory = dbContextFactory;
			_userManager = userManager;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			var options = _optionsAccessor.CurrentValue;

			if (options?.DefaultAdminEmail == null || options.DefaultAdminPassword == null)
			{
				_logger.LogDebug("Default administrator credentials is not specified in startup options");

				return;
			}

			if (await _dbContextFactory.HasData<DbUser>(cancellationToken))
			{
				_logger.LogWarning("Database already contains users, consider to remove default administrator credentials from startup options");

				return;
			}

			_logger.LogInformation("Creating default administrator {adminEmail}", options.DefaultAdminEmail);

			var user = new DbUser
			{
				Id = Guid.NewGuid(),
				UserName = options.DefaultAdminEmail,
				Email = options.DefaultAdminEmail
			};

			var identityResult = await _userManager.CreateAsync(user, options.DefaultAdminPassword);

			identityResult.ToApiResult().AssertSuccess(() => "Failed to create default administrator");
		}
	}
}
