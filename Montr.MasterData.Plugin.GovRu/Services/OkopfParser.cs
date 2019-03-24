using Montr.MasterData.Plugin.GovRu.Models;

namespace Montr.MasterData.Plugin.GovRu.Services
{
	public class OkopfParser : OkParser<OkItem>
	{
		protected override string OkCode => "nsiOkopfData";
	}
}
