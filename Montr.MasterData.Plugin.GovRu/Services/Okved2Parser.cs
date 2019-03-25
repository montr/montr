using System.Xml.Linq;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class Okved2Parser : XmlOkParser<Okved2Item>
	{
		protected override string OkCode => "nsiOkved2Data";

		protected override Okved2Item Parse(XElement element)
		{
			var item = base.Parse(element);

			item.Section = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:section");

			return item;
		}
	}
}
