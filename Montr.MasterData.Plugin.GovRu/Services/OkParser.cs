using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Montr.MasterData.Models;
using Montr.MasterData.Plugin.GovRu.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public abstract class OkParser<TItem> : IClassifierParser where TItem : OkItem, new()
	{
		public const string DefaultNsUri = "http://zakupki.gov.ru/223fz/types/1";
		public const string NsUri = "http://zakupki.gov.ru/223fz/reference/1";

		public const string DefaultNsPrefix = "ns";
		public const string NsPrefix = "ns2";

		protected readonly XmlNamespaceManager _nsManager;

		private readonly List<TItem> _items;

		protected OkParser()
		{
			var nameTable = new NameTable();
			_nsManager = new XmlNamespaceManager(nameTable);
			_nsManager.AddNamespace(DefaultNsPrefix, DefaultNsUri);
			_nsManager.AddNamespace(NsPrefix, NsUri);

			_items = new List<TItem>();
		}

		protected abstract string OkCode { get; }

		public void Reset()
		{
			_items.Clear();
		}

		public async Task Parse(Stream stream, CancellationToken cancellationToken)
		{
			var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

			foreach (var element in xdoc.XPathSelectElements($"//{NsPrefix}:item", _nsManager))
			{
				var item = Parse(element);

				_items.Add(item);
			}
		}

		public ParseResult GetResult()
		{
			var result = Convert(
				_items // .Where(x => x.BusinessStatus == BusinessStatus.Included).ToList()
			);

			return result;
		}

		protected virtual TItem Parse(XElement element)
		{
			return new TItem
			{
				Uid = Select<Guid?>(element, $"{DefaultNsPrefix}:guid"),
				BusinessStatus = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:businessStatus"),
				ChangeDateTime = Select<DateTime?>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:changeDateTime"),
				Code = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:code"),
				Name = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:name"),
				ParentCode = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:parentCode")
			};
		}

		protected T Select<T>(XElement element, string xpath)
		{
			var child = element.XPathSelectElement(xpath, _nsManager);

			if (child != null)
			{
				var destinationType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

				if (destinationType == typeof(Guid))
				{
					return (T)(object)Guid.Parse(child.Value);
				}

				return (T)System.Convert.ChangeType(child.Value, destinationType);
			}

			return default(T);
		}

		protected virtual ParseResult Convert(IList<TItem> items)
		{
			var result = new ParseResult
			{
				Items = new List<Classifier>()
			};

			// take last modified item if multiple items with same code exists
			var dict = items.ToLookup(x => x.Code)
				.ToDictionary(x => x.Key,
					g => g.OrderBy(i => i.BusinessStatus == BusinessStatus.Included ? 0 : 1)
						.ThenByDescending(i => i.ChangeDateTime)
						.First());

			foreach (var item in dict.Values)
			{
				var @class = new Classifier
				{
					Code = item.Code,
					Name = item.Name,
					StatusCode = item.BusinessStatus == BusinessStatus.Included
						? ClassifierStatusCode.Active
						: ClassifierStatusCode.Inactive,
					ParentCode = item.ParentCode
				};

				result.Items.Add(@class);
			}

			return result;
		}
	}
}
