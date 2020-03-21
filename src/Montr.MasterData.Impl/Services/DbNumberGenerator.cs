using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
		public static readonly StringComparer TagComparer = StringComparer.OrdinalIgnoreCase;

		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly PatternParser _patternParser = new PatternParser();
		private readonly IEnumerable<INumberTagProvider> _tagProviders;

		public DbNumberGenerator(
			IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider,
			IEnumerable<INumberTagProvider> tagProviders)
		{
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
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

				var keyBuilder = new KeyBuilder();

				if (numerator.Periodicity != NumeratorPeriodicity.None && date.HasValue)
				{
					var periodStart = GetPeriodStart(date.Value, numerator.Periodicity);

					keyBuilder.Append("Period", periodStart.ToString("yyyy-MM-dd"));
				}

				// include in key only tags selected for independent numbers
				if (numerator.KeyTags != null)
				{
					foreach (var tag in tags.Where(x =>
						numerator.KeyTags.Contains(x, TagComparer)).OrderBy(x => x, TagComparer))
					{
						keyBuilder.Append(tag, values[tag]);
					}
				}

				var key = keyBuilder.Build();

				var counter = await IncrementCounter(numerator, key, cancellationToken);

				AddKnownTags(values, counter, date);

				var result = numerator.Pattern;

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

				var dbNumerator = await db
					.GetTable<DbNumerator>()
					.Where(x => x.EntityTypeCode == entityTypeCode && x.Uid == dbNumeratorEntity.NumeratorUid)
					.FirstAsync(cancellationToken);

				return new Numerator
				{
					Uid = dbNumerator.Uid,
					Name = dbNumerator.Name,
					Periodicity = Enum.Parse<NumeratorPeriodicity>(dbNumerator.Periodicity),
					Pattern = dbNumerator.Pattern,
					KeyTags = dbNumerator.KeyTags?.Split(DbNumerator.KeyTagsSeparator, StringSplitOptions.RemoveEmptyEntries),
					IsActive = dbNumerator.IsActive,
					IsSystem = dbNumerator.IsSystem
				};
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

		private static void AddKnownTags(IDictionary<string, string> values, long counter, DateTime? date)
		{
			// todo: parse number digits count
			values[NumeratorKnownTags.Number] = counter.ToString("D5");

			if (date.HasValue)
			{
				var quarter = (date.Value.Month - 1) / 3 + 1;
				var year2 = date.Value.Year - (int)Math.Floor(date.Value.Year / 100M) * 100;
				
				values[NumeratorKnownTags.Day] = date.Value.Day.ToString("D2");
				values[NumeratorKnownTags.Month] = date.Value.Month.ToString("D2");
				values[NumeratorKnownTags.Quarter] = quarter.ToString();
				values[NumeratorKnownTags.Year2] = year2.ToString("D2");
				values[NumeratorKnownTags.Year4] = date.Value.Year.ToString();
			}
		}

		public class PatternParser
		{
			private static readonly Regex NumberPatternRegex = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);

			public ISet<string> Parse(string pattern)
			{
				if (pattern == null) throw new ArgumentNullException(nameof(pattern));

				var matches = NumberPatternRegex.Matches(pattern);

				return matches.Select(x => x.Value).ToHashSet();
			}
		}

		public class Tag
		{
			public string Original { get; set; }

			public string Name { get; set; }

			public string Args { get; set; }
		}

		private class KeyBuilder
		{
			private readonly StringBuilder _builder = new StringBuilder();

			public void Append(string tag, string value)
			{
				if (_builder.Length > 0) _builder.Append("/");

				_builder.Append("[").Append(tag).Append(":").Append(value).Append("]");
			}

			public string Build()
			{
				return _builder.ToString().ToLowerInvariant();
			}
		}
	}
}
