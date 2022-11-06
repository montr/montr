using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Montr.Core.Events;
using Montr.Data.Linq2Db;
using Montr.Settings.Entities;

namespace Montr.Settings.Services.Impl
{
	public static class DbSettingsExtensions
	{
		public static IWebHostBuilder UseDbSettings(this IWebHostBuilder builder, bool reloadOnChange = true)
		{
			var dbConfigurationSource = new DbConfigurationSource { ReloadOnChange = reloadOnChange };

			return builder
				.ConfigureAppConfiguration((_, config) =>
				{
					config.Add(dbConfigurationSource);
				})
				.ConfigureServices((_, services) =>
				{
					services.AddSingleton<INotificationHandler<SettingsChanged>>(dbConfigurationSource);
				});
		}
	}

	public class DbConfigurationSource : IConfigurationSource, INotificationHandler<SettingsChanged>
	{
		private ConfigurationReloadToken _reloadToken;

		public bool ReloadOnChange { get; set; }

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			if (ReloadOnChange)
			{
				_reloadToken = new ConfigurationReloadToken();
			}

			// todo: resolve services
			var dbContextFactory = new DefaultDbContextFactory();

			return new DbConfigurationProvider(this, dbContextFactory);
		}

		public IChangeToken Watch()
		{
			return _reloadToken;
		}

		public Task Handle(SettingsChanged notification, CancellationToken cancellationToken)
		{
			if (ReloadOnChange)
			{
				var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());

				// todo: notify other workers (Redis)
				previousToken.OnReload();
			}

			return Task.CompletedTask;
		}
	}

	public class DbConfigurationProvider : ConfigurationProvider
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbConfigurationProvider(DbConfigurationSource source, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;

			if (source.ReloadOnChange)
			{
				ChangeToken.OnChange(source.Watch, Load);
			}
		}

		public override void Load()
		{
			using (var db = _dbContextFactory.Create())
			{
				Data = db.GetTable<DbSettings>()
					.ToDictionary(x => x.Id, x => x.Value, StringComparer.OrdinalIgnoreCase);
			}

			OnReload();
		}
	}
}
