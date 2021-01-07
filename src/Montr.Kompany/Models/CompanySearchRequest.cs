using System;
using Montr.Core.Models;

namespace Montr.Kompany.Models
{
	public class CompanySearchRequest : SearchRequest
	{
		public Guid? UserUid { get; set; }
	}
}
