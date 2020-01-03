﻿using System;

namespace Montr.Core.Models
{
	public class MetadataSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid? Uid { get; set; }

		public bool? IsSystem { get; set; }

		public bool? IsActive { get; set; }
	}
}
