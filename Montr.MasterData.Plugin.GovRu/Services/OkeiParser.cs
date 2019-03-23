using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Montr.MasterData.Models;
using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
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

		protected override ParseResult Convert(IList<OkeiItem> items, out IDictionary<string, OkeiItem> itemMap)
		{
			var result = base.Convert(items, out itemMap);

			var sections = new Dictionary<string, ClassifierGroup>();
			var groups = new Dictionary<string, ClassifierGroup>();

			var itemsInGroups = new List<ClassifierLink>();

			foreach (var item in itemMap.Values)
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
					ItemCode = item.Code,
					ItemStatusCode = ToStatusCode(item.BusinessStatus) 
				});
			}

			result.Groups = sections.Values.Union(groups.Values).ToList();
			result.Links = itemsInGroups;

			return result;
		}
	}
}
