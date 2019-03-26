using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class XmlOkpd2Parser : XmlOkParser<OkItem>
	{
		protected override string OkCode => "nsiOkpd2Data";
	}
}
