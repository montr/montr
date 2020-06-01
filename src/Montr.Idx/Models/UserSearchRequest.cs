using Montr.Core.Models;

namespace Montr.Idx.Models
{
	public class UserSearchRequest : SearchRequest
	{
		public string UserName { get; set; }
	}
}
