using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public static class ModelStateExtensions
	{
		public static ApiResult ToApiResult(this ModelStateDictionary modelState)
		{
			var result = new ApiResult
			{
				Success = modelState.IsValid,
				Errors = new List<ApiResultError>()
			};

			foreach (var pair in modelState)
			{
				result.Errors.Add(new ApiResultError
				{
					Key = pair.Key,
					Messages = pair.Value.Errors.Select(x => x.ErrorMessage).ToArray()
				});
			}

			return result;
		}
	}
}
