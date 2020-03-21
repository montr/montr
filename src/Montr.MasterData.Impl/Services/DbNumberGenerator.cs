using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbNumberGenerator : INumberGenerator
	{
		public static readonly string NumberTag = "{Number}";
		public static readonly string IndiTagsSeparator = ",";

		public static readonly StringComparer TagComparer = StringComparer.OrdinalIgnoreCase;

		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly NumberPatternParser _patternParser;
		private readonly IEnumerable<INumberTagProvider> _tagProviders;

		public DbNumberGenerator(IDbContextFactory dbContextFactory, IDateTimeProvider dateTimeProvider,
			NumberPatternParser patternParser, IEnumerable<INumberTagProvider> tagProviders)
		{
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_patternParser = patternParser;
			_tagProviders = tagProviders;
		}

		public async Task<string> GenerateNumber(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken)
		{
			// todo: add distributed lock
			{
				var numerator = await GetNumerator(entityTypeCode, enityUid, cancellationToken);

				if (numerator == null) return null;

				var tags = _patternParser.Parse(numerator.Pattern);

				DateTime? date = null;
				var values = tags.ToDictionary(x => x, x => "00", TagComparer);

				foreach (var tagProvider in _tagProviders)
				{
					if (tagProvider.Supports(entityTypeCode, out var supportedTags))
					{
						var tagsToResolve = tags.Intersect(supportedTags, TagComparer);

						await tagProvider.Resolve(entityTypeCode, enityUid, out var providerDate, tagsToResolve, values, cancellationToken);

						date = providerDate;
					}
				}

				var keyBuilder = new StringBuilder();

				if (numerator.Periodicity != NumeratorPeriodicity.None && date.HasValue)
				{
					var periodStart = GetPeriodStart(date.Value, numerator.Periodicity);

					values["{Year4}"] = periodStart.Year.ToString();

					keyBuilder.Append("{Period}").Append("/").Append(periodStart.ToString("yyyy-MM-dd"));
				}

				// include in key only tags selected for independent numbers
				if (numerator.KeyTags != null)
				{
					foreach (var tag in tags.Where(x =>
						numerator.KeyTags.Contains(x, TagComparer)).OrderBy(x => x, TagComparer))
					{
						if (keyBuilder.Length > 0) keyBuilder.Append("_");

						keyBuilder.Append(tag).Append("/").Append(values[tag]);
					}
				}

				var key = keyBuilder.ToString().ToLowerInvariant();

				var counter = await IncrementCounter(numerator, key, cancellationToken);

				var result = numerator.Pattern;

				// todo: parse number digits count
				result = result.Replace(NumberTag, counter.ToString("D5"), StringComparison.OrdinalIgnoreCase);

				foreach (var tag in tags)
				{
					result = result.Replace(tag, values[tag], StringComparison.OrdinalIgnoreCase);
				}

				return result;
			}
		}

		// todo: get numerator from cache
		private async Task<Numerator> GetNumerator(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken)
		{
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

				var numerator = new Numerator
				{
					Uid = dbNumerator.Uid,
					Name = dbNumerator.Name,
					Periodicity = Enum.Parse<NumeratorPeriodicity>(dbNumerator.Periodicity),
					Pattern = dbNumerator.Pattern,
					KeyTags = dbNumerator.KeyTags?.Split(IndiTagsSeparator, StringSplitOptions.RemoveEmptyEntries),
					IsActive = dbNumerator.IsActive,
					IsSystem = dbNumerator.IsSystem
				};

				return numerator;
			}
		}

		private async Task<long> IncrementCounter(Numerator numerator, string key, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				var dbNumeratorCounter = await db
					.GetTable<DbNumeratorCounter>()
					.Where(x => x.NumeratorUid == numerator.Uid && x.Key == key)
					.FirstOrDefaultAsync(cancellationToken);

				long counter;

				if (dbNumeratorCounter == null)
				{
					counter = 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Value(x => x.NumeratorUid, numerator.Uid)
						.Value(x => x.Key, key)
						.Value(x => x.Value, counter)
						.Value(x => x.GeneratedAtUtc, _dateTimeProvider.GetUtcNow())
						.InsertAsync(cancellationToken);
				}
				else
				{
					counter = dbNumeratorCounter.Value + 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Where(x => x.NumeratorUid == numerator.Uid && x.Key == key)
						.Set(x => x.Value, counter)
						.Set(x => x.GeneratedAtUtc, _dateTimeProvider.GetUtcNow())
						.UpdateAsync(cancellationToken);
				}

				return counter;
			}
		}

		private static DateTime GetPeriodStart(DateTime date, NumeratorPeriodicity periodicity)
		{
			switch (periodicity)
			{
				case NumeratorPeriodicity.Year:
					return new DateTime(date.Year, 1, 1);

				case NumeratorPeriodicity.Quarter:
					return new DateTime(date.Year, (date.Month - 1) / 3 * 3 + 1, 1);

				case NumeratorPeriodicity.Month:
					return new DateTime(date.Year, date.Month, 1);

				case NumeratorPeriodicity.Day:
					return date;

				default:
					throw new ArgumentException($"Periodicity {periodicity} is not supported", nameof(periodicity));
			}
		}
	}
}
