using System.Collections.Generic;
using Montr.Web.Models;

namespace Tendr.Models
{
	public class DataResult<TModel> : ApiResult
	{
		public int TotalCount { get; set; }

		public IEnumerable<TModel> Rows { get; set; }
	}
}