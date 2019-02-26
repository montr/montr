﻿using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierSearchRequest : SearchRequest
	{
		public System.Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }
	}
}
