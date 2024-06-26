﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Settings.Entities;
using Montr.Settings.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IConfigurationProvider = Microsoft.Extensions.Configuration.IConfigurationProvider;

namespace Montr.Settings.Services.Implementations
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
			if (ReloadOnChange
			    && notification.EntityTypeCode == Application.EntityTypeCode
			    && notification.EntityUid == Application.EntityUid)
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
				// only Application settings available in DbConfigurationProvider

				var dbData = db.GetTable<DbSettings>()
					.Where(x => x.EntityTypeCode == Application.EntityTypeCode &&
					            x.EntityUid == Application.EntityUid)
					.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

				var result = new Dictionary<string, string>();

				foreach (var pair in dbData)
				{
					// todo: store settings type in db to recognize arrays
					var valueIsArray = pair.Value?.Length > 0 && pair.Value[0] == '[' && pair.Value[^1] == ']';

					if (valueIsArray)
					{
						var array = (JArray)JsonConvert.DeserializeObject(pair.Value);

						for (var i = 0; i < array?.Count; i++)
						{
							result[pair.Key + ":" + i] = array[i].Value<string>();
						}
					}
					else
					{
						result[pair.Key] = pair.Value;
					}
				}

				Data = result;
			}

			OnReload();
		}
	}
}
