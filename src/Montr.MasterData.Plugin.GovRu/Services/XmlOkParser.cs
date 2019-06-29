using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public abstract class XmlOkParser<TItem> : OkParser<TItem> where TItem : OkItem, new()
	{
		public const string DefaultNsUri = "http://zakupki.gov.ru/223fz/types/1";
		public const string NsUri = "http://zakupki.gov.ru/223fz/reference/1";

		public const string DefaultNsPrefix = "ns";
		public const string NsPrefix = "ns2";

		protected readonly XmlNamespaceManager _nsManager;

		protected XmlOkParser()
		{
			var nameTable = new NameTable();
			_nsManager = new XmlNamespaceManager(nameTable);
			_nsManager.AddNamespace(DefaultNsPrefix, DefaultNsUri);
			_nsManager.AddNamespace(NsPrefix, NsUri);
		}

		protected abstract string OkCode { get; }

		public override async Task Parse(Stream stream, CancellationToken cancellationToken)
		{
			var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

			foreach (var element in xdoc.XPathSelectElements($"//{NsPrefix}:item", _nsManager))
			{
				var item = Parse(element);

				_items.Add(item);
			}
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
				return (T)Convert(child.Value, typeof(T));
			}

			return default(T);
		}
	}
}
