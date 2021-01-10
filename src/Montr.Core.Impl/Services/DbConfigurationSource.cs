using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Montr.Core.Events;
using Montr.Core.Impl.Entities;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbConfigurationSource : IConfigurationSource
	{
		public DbConfigurationSettingsChangedHandler SettingsChangedHandler { get; set; }

		public bool ReloadOnChange { get; set; }

		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			// todo: resolve services
			var dbContextFactory = new DefaultDbContextFactory();

			return new DbConfigurationProvider(this, dbContextFactory);
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
				ChangeToken.OnChange(() => source.SettingsChangedHandler.Watch(), Load);
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

	public static class ConfigurationBuilderExtensions
	{
		public static IConfigurationBuilder AddDbSettings(this IConfigurationBuilder builder,
			DbConfigurationSettingsChangedHandler settingsChangedHandler, bool reloadOnChange = true)
		{
			return builder.Add(new DbConfigurationSource
			{
				SettingsChangedHandler = settingsChangedHandler,
				ReloadOnChange = reloadOnChange
			});
		}
	}

	public class DbConfigurationSettingsChangedHandler : INotificationHandler<SettingsChanged>
	{
		private ConfigurationReloadToken _reloadToken;

		public bool Enabled { get; set; }

		public DbConfigurationSettingsChangedHandler()
		{
			_reloadToken = new ConfigurationReloadToken();
		}

		public IChangeToken Watch()
		{
			return _reloadToken;
		}

		public Task Handle(SettingsChanged notification, CancellationToken cancellationToken)
		{
			if (Enabled)
			{
				var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());

				previousToken.OnReload();
			}

			return Task.CompletedTask;
		}
	}
}
