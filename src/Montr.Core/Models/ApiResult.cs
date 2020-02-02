using System;
using System.Collections.Generic;
using System.Linq;

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

		public long? AffectedRows { get; set; }

		public string RedirectUrl { get; set; }

		public IList<ApiResultError> Errors { get; set; }

		// todo: add tests
		public void AssertSuccess(Func<string> getMessage)
		{
			if (Success == false)
			{
				var message = getMessage();

				// todo: get detailed errors from result
				var errors = string.Join(", ", Errors.SelectMany(x => x.Messages));

				// todo: use ApiException ?
				throw new ApplicationException($"{message} - {errors}");
			}
		}
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
