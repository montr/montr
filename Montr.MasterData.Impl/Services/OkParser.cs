using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LinqToDB.Extensions;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	// todo: move to ...Impl.GovRu project
	public abstract class OkParser<TItem> : IClassifierParser where TItem : OkItem, new()
	{
		public const string DefaultNsUri = "http://zakupki.gov.ru/223fz/types/1";
		public const string NsUri = "http://zakupki.gov.ru/223fz/reference/1";

		public const string DefaultNsPrefix = "ns";
		public const string NsPrefix = "ns2";

		protected readonly XmlNamespaceManager _nsManager;

		protected OkParser()
		{
			var nameTable = new NameTable();
			_nsManager = new XmlNamespaceManager(nameTable);
			_nsManager.AddNamespace(DefaultNsPrefix, DefaultNsUri);
			_nsManager.AddNamespace(NsPrefix, NsUri);
		}

		protected abstract string OkCode { get; }

		public async Task<ParseResult> Parse(Stream stream, CancellationToken cancellationToken)
		{
			var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

			var items = new List<TItem>();

			foreach (var element in xdoc.XPathSelectElements($"//{NsPrefix}:item", _nsManager))
			{
				var item = Parse(element);

				items.Add(item);
			}

			var result = Convert(
				items.Where(x => x.BusinessStatus == BusinessStatus.Included).ToList()
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
				Name = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:name")
			};
		}

		protected T Select<T>(XElement element, string xpath)
		{
			var child = element.XPathSelectElement(xpath, _nsManager);

			if (child != null)
			{
				var destinationType = typeof(T).ToNullableUnderlying();

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
			return new ParseResult
			{
				Items = items
					.Select(x => new Classifier
					{
						Code = x.Code,
						Name = x.Name
					}).ToList()
			};
		}
	}

	public class OkItem
	{
		public Guid? Uid { get; set; }

		public string BusinessStatus { get; set; }

		public DateTime? ChangeDateTime { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }
	}

	public class BusinessStatus
	{
		/// <summary>
		/// Включена
		/// </summary>
		public  const string Included = "801";

		/// <summary>
		/// Исключена
		/// </summary>
		public const string Excluded = "801";
	}

	public class OkeiParser : OkParser<OkeiItem>
	{
		protected override string OkCode => "nsiOkeiData";

		protected override OkeiItem Parse(XElement element)
		{
			var item = base.Parse(element);

			item.Symbol = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:symbol");
			item.SectionCode = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:section/{NsPrefix}:code");
			item.SectionName = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:section/{NsPrefix}:name");
			item.GroupCode = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:group/{NsPrefix}:code");
			item.GroupName = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:group/{NsPrefix}:name");

			return item;
		}

		protected override ParseResult Convert(IList<OkeiItem> items)
		{
			var result = base.Convert(items);

			var sections = new Dictionary<string, ClassifierGroup>();
			var groups = new Dictionary<string, ClassifierGroup>();

			var itemsInGroups = new List<ClassifierLink>();

			foreach (var item in items)
			{
				var sectionCode = item.SectionCode;
				var groupCode = item.SectionCode + "." + item.GroupCode;

				if (sections.TryGetValue(sectionCode, out _) == false)
				{
					sections[sectionCode] = new ClassifierGroup
					{
						Code = sectionCode,
						Name = item.SectionName
					};
				}

				if (groups.TryGetValue(groupCode, out _) == false)
				{
					sections[groupCode] = new ClassifierGroup
					{
						ParentCode = sectionCode,
						Code = groupCode,
						Name = item.GroupName
					};
				}

				itemsInGroups.Add(new ClassifierLink
				{
					GroupCode = groupCode,
					ItemCode = item.Code
				});
			}

			result.Groups = sections.Values.Union(groups.Values).ToList();
			result.Links = itemsInGroups;

			return result;
		}
	}

	public class OkeiItem : OkItem
	{
		public string Symbol { get; set; }

		public string SectionCode { get; set; }

		public string SectionName { get; set; }

		public string GroupCode { get; set; }

		public string GroupName { get; set; }
	}

	public class Okved2Parser : OkParser<Okved2Item>
	{
		protected override string OkCode => "nsiOkved2Data";

		protected override Okved2Item Parse(XElement element)
		{
			var item = base.Parse(element);

			item.ParentCode = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:parentCode");
			item.Section = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:section");

			return item;
		}

		protected override ParseResult Convert(IList<Okved2Item> items)
		{
			var result = base.Convert(items);

			foreach (var item in items)
			{
				if (item.ParentCode != null)
				{
					result.Items.Single(x => x.Code == item.Code).ParentCode = item.ParentCode;
				}
			}

			return result;
		}
	}
	
	public class Okved2Item : OkItem
	{
		public string ParentCode { get; set; }

		public string Section { get; set; }
	}
}
