using System;
using Montr.Core.Models;

namespace Montr.Docs.Models
{
	public class ProcessSearchRequest : SearchRequest
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public string Code { get; set; }

		public Guid? Uid { get; set; }
	}
}
