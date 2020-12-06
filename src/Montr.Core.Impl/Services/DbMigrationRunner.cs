using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Core.Impl.Entities;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	// todo: backup db before migration (when not applied migrations exists)
	public class DbMigrationRunner : IMigrationRunner
	{
		private readonly ILogger<DbMigrationRunner> _logger;
		private readonly IOptionsMonitor<MigrationOptions> _optionsAccessor;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly EmbeddedResourceProvider _resourceProvider;
		private readonly HashProvider _hashProvider;

		public DbMigrationRunner(ILogger<DbMigrationRunner> logger,
			IOptionsMonitor<MigrationOptions> optionsAccessor, IDbContextFactory dbContextFactory,
			EmbeddedResourceProvider resourceProvider)
		{
			_logger = logger;
			_optionsAccessor = optionsAccessor;
			_dbContextFactory = dbContextFactory;
			_resourceProvider = resourceProvider;

			_hashProvider = new HashProvider();
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			var options = _optionsAccessor.CurrentValue;

			if (options.MigrationPath == null)
			{
				_logger.LogWarning("Migrations path not specified, skipping migrations");

				return;
			}

			_logger.LogInformation("Running migrations from {MigrationPath}", options.MigrationPath);

			var watch = new Stopwatch();

			watch.Start();

			// todo: calc migrations that will be run
			var migrations = GetMigrations(options);

			_logger.LogInformation("Found {Count} migrations", migrations.Count);

			await using (var db = _dbContextFactory.Create(options.ConnectionName))
			{
				await Bootstrap(db, cancellationToken);

				foreach (var migration in migrations)
				{
					await Run(db, migration, options, cancellationToken);
				}
			}

			watch.Stop();

			_logger.LogInformation("Migrations completed in {Elapsed}", watch.Elapsed);
		}

		private async Task Run(DbContext db, Migration migration, MigrationOptions options, CancellationToken cancellationToken)
		{
			var migrationByHash = await db.GetTable<DbMigration>().SingleOrDefaultAsync(x => x.Hash == migration.Hash, cancellationToken);

			if (migrationByHash != null)
			{
				_logger.LogDebug("Skipping migration {FileName}", migration.FileName);

				if (migrationByHash.FileName != migration.FileName)
				{
					_logger.LogInformation(
						"Filename has changed in the database from {FromFileName} to {ToFileName}, updating",
						migration.FileName, migrationByHash.FileName);

					await db.GetTable<DbMigration>()
						.Where(x => x.Hash == migration.Hash)
						.Set(x => x.FileName, migration.FileName)
						.UpdateAsync(cancellationToken);
				}

				return;
			}

			_logger.LogInformation("Running migration {FileName}", migration.FileName);

			var dbMigration = await db.GetTable<DbMigration>()
				.SingleOrDefaultAsync(x => x.FileName == migration.FileName, cancellationToken);

			var exists = dbMigration != null;

			if (exists)
			{
				if (options.Force == false)
				{
					throw new ApplicationException(
						$"Failed to migrate: {migration.FileName} - the file was already migrated in the past and changed after migration. "
						+ "To force migration use the \"force\" option.");
				}
			}

			var watch = new Stopwatch();

			try
			{
				watch.Start();

				using (var transaction = await db.BeginTransactionAsync(cancellationToken))
				{
					await db.ExecuteAsync(migration.Sql, cancellationToken);

					if (exists)
					{
						await db.GetTable<DbMigration>()
							.Where(x => x.FileName == migration.FileName)
							.Set(x => x.FileName, migration.FileName)
							.Set(x => x.Hash, migration.Hash)
							.Set(x => x.ExecutedAtUtc, DateTime.UtcNow)
							.Set(x => x.DurationMs, watch.ElapsedMilliseconds)
							.UpdateAsync(cancellationToken);
					}
					else
					{
						await db.GetTable<DbMigration>()
							.Value(x => x.FileName, migration.FileName)
							.Value(x => x.Hash, migration.Hash)
							.Value(x => x.ExecutedAtUtc, DateTime.UtcNow)
							.Value(x => x.DurationMs, watch.ElapsedMilliseconds)
							.InsertAsync(cancellationToken);
					}

					await transaction.CommitAsync(cancellationToken);
				}

				watch.Stop();
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Failed to run migration: {migration.FileName} {ex.Message}", ex);
			}
		}

		private IList<Migration> GetMigrations(MigrationOptions options)
		{
			var files = Directory.EnumerateFiles(options.MigrationPath, "*.sql", SearchOption.AllDirectories);

			return files
				.Select(file => new FileInfo(file))
				.OrderBy(file => file.Name)
				.Select(file =>
				{
					var sql = File.ReadAllText(file.FullName);
					var hash = _hashProvider.GetHash(sql);

					return new Migration { FileName = file.Name, Sql = sql, Hash = hash};
				}).ToList();
		}

		private async Task Bootstrap(DbContext db, CancellationToken cancellationToken)
		{
			var sql = await _resourceProvider.LoadEmbeddedResource(typeof(Module), "Resources.bootstrap.sql");

			await db.ExecuteAsync(sql, cancellationToken);
		}

		private class HashProvider
		{
			private readonly MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

			public string GetHash(string value)
			{
				lock (_md5Provider)
				{
					var hash = _md5Provider.ComputeHash(Encoding.Unicode.GetBytes(value));

					return new Guid(hash).ToString();
				}
			}
		}

		private class Migration
		{
			public string FileName { get; init; }

			public string Sql { get; init; }

			public string Hash { get; init; }
		}
	}
}
