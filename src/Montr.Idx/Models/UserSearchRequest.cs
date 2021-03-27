using Montr.MasterData.Models;

namespace Montr.Idx.Models
{
	public class UserSearchRequest : ClassifierSearchRequest
	{
		public string UserName { get; set; }
	}
}
