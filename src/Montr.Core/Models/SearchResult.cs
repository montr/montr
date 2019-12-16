using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class SearchResult<TModel> // : ApiResult (?)
	{
		public int? TotalCount { get; set; }

		public IList<TModel> Rows { get; set; }
	}
}
