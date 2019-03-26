using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class XmlOkopfParser : XmlOkParser<OkItem>
	{
		protected override string OkCode => "nsiOkopfData";
	}
}
