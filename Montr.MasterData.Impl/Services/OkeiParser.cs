using System;
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
	public class OkeiParser : IClassifierParser
	{
		const string namespaceUri = "http://zakupki.gov.ru/223fz/reference/1";

		public async Task<ICollection<Classifier>> Parse(Stream stream, CancellationToken cancellationToken)
		{
			var result = new List<Classifier>();

			var xdoc = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);

			var nameTable = new NameTable();
			var namespaceManager = new XmlNamespaceManager(nameTable);
			namespaceManager.AddNamespace("ns2", namespaceUri);

			foreach (var element in xdoc.XPathSelectElements("//ns2:item", namespaceManager))
			{
				var item = new Classifier();

				var okCode = "nsiOkeiData";
				okCode = "nsiOkved2Data";

				item.Code = element.XPathSelectElement($"ns2:{okCode}/ns2:code", namespaceManager).Value;
				item.Name = element.XPathSelectElement($"ns2:{okCode}/ns2:name", namespaceManager).Value;

				result.Add(item);
			}

			return result;
		}
	}
}
