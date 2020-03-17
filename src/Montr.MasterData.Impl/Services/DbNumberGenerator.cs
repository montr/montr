﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbNumberGenerator : INumberGenerator
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly NumberPatternParser _patternParser;
		private readonly IEnumerable<INumberTagProvider> _tagProviders;

		public DbNumberGenerator(IDbContextFactory dbContextFactory,
			NumberPatternParser patternParser, IEnumerable<INumberTagProvider> tagProviders)
		{
			_dbContextFactory = dbContextFactory;
			_patternParser = patternParser;
			_tagProviders = tagProviders;
		}

		public async Task<string> GenerateNumber(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken)
		{
			// todo: add distributed lock
			using (var db = _dbContextFactory.Create())
			{
				// todo: join two queries
				var dbNumeratorEntity = await db.GetTable<DbNumeratorEntity>()
					.Where(x => x.EntityUid == enityUid)
					.FirstOrDefaultAsync(cancellationToken);

				if (dbNumeratorEntity == null) return null;

				var numeratorUid = dbNumeratorEntity.NumeratorUid;

				var dbNumerator = await db
					.GetTable<DbNumerator>()
					.Where(x => x.Uid == numeratorUid)
					.FirstAsync(cancellationToken);


				var tagComparer = StringComparer.OrdinalIgnoreCase;
				var tagComparison = StringComparison.OrdinalIgnoreCase;

				var numberTag = "{Number}";

				var tags = _patternParser.Parse(dbNumerator.Pattern);
				var values = tags.ToDictionary(x => x, x => "00", tagComparer);

				foreach (var tagProvider in _tagProviders)
				{
					if (tagProvider.Supports(entityTypeCode, out var supportedTags))
					{
						var tagsToResolve = tags.Intersect(supportedTags, tagComparer);

						await tagProvider.Resolve(entityTypeCode, enityUid, tagsToResolve, values, cancellationToken);
					}
				}

				// todo: check periodicity && include in key only tags unique in periodicity
				var keyBuilder = new StringBuilder();

				foreach (var tag in tags
					.Where(x => string.Equals(x, numberTag, tagComparison) == false)
					.OrderBy(x => x, tagComparer))
				{
					var value = values[tag];

					if (keyBuilder.Length > 0) keyBuilder.Append("_");

					keyBuilder.Append(tag).Append("/").Append(value);
				}

				var key = keyBuilder.ToString().ToLowerInvariant();

				var dbNumeratorCounter = await db
					.GetTable<DbNumeratorCounter>()
					.Where(x => x.NumeratorUid == numeratorUid && x.Key == key)
					.FirstOrDefaultAsync(cancellationToken);

				long counter;

				if (dbNumeratorCounter == null)
				{
					counter = 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Value(x => x.NumeratorUid, numeratorUid)
						.Value(x => x.Key, key)
						.Value(x => x.Value, counter)
						.InsertAsync(cancellationToken);
				}
				else
				{
					counter = dbNumeratorCounter.Value + 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Where(x => x.NumeratorUid == numeratorUid && x.Key == key)
						.Set(x => x.Value, counter)
						.UpdateAsync(cancellationToken);
				}

				var pattern = dbNumerator.Pattern;

				pattern = pattern.Replace(numberTag, counter.ToString("D5"), tagComparison);

				foreach (var tag in tags)
				{
					pattern = pattern.Replace(tag, values[tag], tagComparison);
				}

				return pattern;
			}
		}
	}
}
