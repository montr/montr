using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Logging;
using Montr.Data.Linq2Db;
using Montr.Tools.DbMigrator.Entities;

namespace Montr.Tools.DbMigrator.Services
{
	public class MigrationRunner
	{
		private readonly ILogger<MigrationRunner> _logger;
		private readonly DefaultDbContextFactory _dbContextFactory;
		private readonly IHashProvider _hashProvider;

		public MigrationRunner(ILogger<MigrationRunner> logger, IHashProvider hashProvider, DefaultDbContextFactory dbContextFactory)
		{
			_logger = logger;
			_dbContextFactory = dbContextFactory;
			_hashProvider = hashProvider;
		}

		public async Task Run(Options options, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Running migrations from {MigrationPath}", options.MigrationPath);

			var watch = new Stopwatch();

			watch.Start();

			var migrations = GetMigrations(options);

			_logger.LogInformation("Found {Count} migrations", migrations.Count);

			using (var db = _dbContextFactory.Create())
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

		private async Task Run(DbContext db, Migration migration, Options options, CancellationToken cancellationToken)
		{
			var migrationByHash = await db.GetTable<DbMigration>().SingleOrDefaultAsync(x => x.Hash == migration.Hash, cancellationToken);

			if (migrationByHash != null)
			{
				_logger.LogInformation("Skipping migration {FileName}", migration.FileName);

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
						$"Failed to migrate: {migration.FileName} - the file was already migrated in the past and changed after migration. To force migration use the --force command");
				}
			}

			var watch = new Stopwatch();

			try
			{
				watch.Start();

				using (var transaction = db.BeginTransaction())
				{
					await db.ExecuteAsync(migration.Sql, cancellationToken);

					await transaction.CommitAsync(cancellationToken);
				}

				watch.Stop();

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
			}
			catch (Exception ex)
			{
				throw new ApplicationException($"Failed to run migration: {migration.FileName} {ex.Message}", ex);
			}
		}

		private IList<Migration> GetMigrations(Options options)
		{
			var migrationPath = options?.MigrationPath ?? Environment.CurrentDirectory;

			var files = Directory.EnumerateFiles(migrationPath, "*.sql", SearchOption.AllDirectories);

			return files
				.Select(file => new FileInfo(file))
				.OrderBy(fileInfo => fileInfo.Name)
				.Select(fileInfo =>
				{
					var sql = File.ReadAllText(fileInfo.FullName);
					var hash = _hashProvider.GetHash(sql);

					return new Migration { FileName = fileInfo.Name, Sql = sql, Hash = hash};
				}).ToList();
		}

		private static async Task Bootstrap(DbContext db, CancellationToken cancellationToken)
		{
			var sql = await LoadEmbeddedResource(typeof(Options), "Resources.bootstrap.sql");

			await db.ExecuteAsync(sql, cancellationToken);
		}

		private static async Task<string> LoadEmbeddedResource(Type type, string name)
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(type, name))
			{
				using (var reader = new StreamReader(stream ?? throw new ApplicationException($"Resource \"{name}\" is not found.")))
				{
					return await reader.ReadToEndAsync();
				}
			}
		}

		private class Migration
		{
			public string FileName { get; set; }

			public string Sql { get; set; }

			public string Hash { get; set; }
		}
	}
}
