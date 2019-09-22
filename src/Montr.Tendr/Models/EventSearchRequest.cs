using System;
using Montr.Core.Models;

namespace Montr.Tendr.Models
{
	public class EventSearchRequest : Paging
    {
	    public Guid CompanyUid { get; set; }

	    public Guid UserUid { get; set; }

		public string Name { get; set; }
    }
}
