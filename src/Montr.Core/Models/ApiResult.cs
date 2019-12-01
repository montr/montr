using System;
using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class ApiResult
	{
		public ApiResult()
		{
			Success = true;	
		}

		public bool Success { get; set; }

		/// <summary>
		/// Explanatory message for result.</summary>
		public string Message { get; set; }

		/// <summary>
		/// Uid of new row in case of insert operation.</summary>
		public Guid? Uid { get; set; }

		public int? AffectedRows { get; set; }

		public string RedirectUrl { get; set; }

		// todo: remove, use client redirect
		public string RedirectRoute { get; set; }

		public IList<ApiResultError> Errors { get; set; }
	}

	public class ApiResult<TData> : ApiResult
	{
		public TData Data { get; set; }
	}

	public class ApiResultError
	{
		public string Key { get; set; }

		public string[] Messages { get; set; }
	}
}
