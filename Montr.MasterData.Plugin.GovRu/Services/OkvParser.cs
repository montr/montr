using System;
using System.Xml.Linq;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class OkvParser : XmlOkParser<OkvItem>
	{
		protected override string OkCode => "nsiOkvData";

		protected override OkvItem Parse(XElement element)
		{
			var item = base.Parse(element);

			item.StartDateActive = Select<DateTime?>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:startDateActive");
			item.EndDateActive = Select<DateTime?>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:endDateActive");
			item.DigitalCode = Select<int>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:digitalCode");
			item.ShortName = Select<string>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:shortName");

			return item;
		}
	}
}
