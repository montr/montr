using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	// todo: move to ...Impl.GovRu project
	public abstract class OkParser : IClassifierParser
	{
		public const string NsUri = "http://zakupki.gov.ru/223fz/reference/1";

		public const string NsPrefix = "ns2";

		protected readonly XmlNamespaceManager _nsManager;

		protected OkParser()
		{
			var nameTable = new NameTable();
			_nsManager = new XmlNamespaceManager(nameTable);
			_nsManager.AddNamespace(NsPrefix, NsUri);
		}

		protected abstract string OkCode { get; }

		protected abstract string ConfigCode { get; }

		public async Task<ICollection<Classifier>> Parse(Stream stream, CancellationToken cancellationToken)
		{
			var result = new List<Classifier>();

			var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

			foreach (var element in xdoc.XPathSelectElements($"//{NsPrefix}:item", _nsManager))
			{
				var item = Parse(element);

				result.Add(item);
			}

			return result;
		}

		protected virtual Classifier Parse(XElement element)
		{
			return new Classifier
			{
				TypeCode = ConfigCode,
				Code = element.XPathSelectElement($"{NsPrefix}:{OkCode}/{NsPrefix}:code", _nsManager).Value,
				Name = element.XPathSelectElement($"{NsPrefix}:{OkCode}/{NsPrefix}:name", _nsManager).Value
			};
		}
	}

	public class OkeiParser : OkParser
	{
		protected override string OkCode => "nsiOkeiData";

		protected override string ConfigCode => "okei";
	}

	public class Okved2Parser : OkParser
	{
		protected override string OkCode => "nsiOkved2Data";

		protected override string ConfigCode => "okved2";
	}
}
