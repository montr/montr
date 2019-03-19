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

			FixCyclicDependencies(item);

			return item;
		}

		private static void FixCyclicDependencies(OktmoItem item)
		{
			// Cyclic dependency found: 01612400 -> 87628405 -> 34606416136 -> 54604410141 -> 11650426101 -> 34606416241 -> 54604410141
			// п Самодед
			if (item.Code == "11650426101" && item.ParentCode == "34606416241") item.ParentCode = "11650426";

			// Cyclic dependency found: 71811151 -> 22628416596 -> 79703000111 -> 89607448000 -> 92604410121 -> 32610428000 -> 89613425101 -> 92604410106 -> 32610428000
			// д Алексеевка
			if (item.Code == "92604410106" && item.ParentCode == "32610428000") item.ParentCode = "92604410";

			// Cyclic dependency found: 33636424136 -> 53723000111 -> 86636411121 -> 86636411126 -> 86636411126
			// д Машезеро
			if (item.Code == "86636411126" && item.ParentCode == "86636411126") item.ParentCode = "86636411";
		}
	}
}
