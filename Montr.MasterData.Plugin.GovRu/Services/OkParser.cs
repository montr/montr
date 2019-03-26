using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;
using Montr.MasterData.Plugin.GovRu.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public abstract class OkParser<TItem> : IClassifierParser where TItem : OkItem, new()
	{
		protected readonly List<TItem> _items = new List<TItem>();

		public void Reset()
		{
			_items.Clear();
		}

		public abstract Task Parse(Stream stream, CancellationToken cancellationToken);

		public ParseResult GetResult()
		{
			return Convert(_items, out _);
		}
		
		protected virtual ParseResult Convert(IList<TItem> items, out IDictionary<string, TItem> itemMap)
		{
			// take last modified item if multiple items with same code exists
			// (only one item with the same code can exists in classifier)

			itemMap = items.ToLookup(x => x.Code)
				.ToDictionary(x => x.Key, g => g
					.OrderBy(i => i.BusinessStatus == BusinessStatus.Included ? 0 : 1)
					.ThenByDescending(i => i.ChangeDateTime)
					.First());

			var result = new ParseResult
			{
				Items = new List<Classifier>()
			};

			foreach (var item in itemMap.Values)
			{
				result.Items.Add(new Classifier
				{
					Code = item.Code,
					Name = item.Name,
					StatusCode = ToStatusCode(item.BusinessStatus),
					ParentCode = item.ParentCode
				});
			}

			return result;
		}

		protected string ToStatusCode(string businessStatus)
		{
			return businessStatus == null ||
					businessStatus == BusinessStatus.Included
				? ClassifierStatusCode.Active
				: ClassifierStatusCode.Inactive;
		}
		
		protected object Convert(string value, Type type)
		{
			var destinationType = Nullable.GetUnderlyingType(type) ?? type;

			if (destinationType == typeof(Guid))
			{
				return Guid.Parse(value);
			}

			return System.Convert.ChangeType(value, destinationType);
		}
	}
}