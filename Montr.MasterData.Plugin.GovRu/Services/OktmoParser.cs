using System;
using System.Xml.Linq;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class OktmoParser : OkParser<OktmoItem>
	{
		protected override string OkCode => "nsiOktmoData";

		protected override OktmoItem Parse(XElement element)
		{
			var item = base.Parse(element);

			item.StartDateActive = Select<DateTime?>(element, $"{NsPrefix}:{OkCode}/{NsPrefix}:startDateActive");

			return item;
		}
	}
}
