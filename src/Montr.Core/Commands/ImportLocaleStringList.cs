using MediatR;
using Microsoft.AspNetCore.Http;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class ImportLocaleStringList : LocaleStringSearchRequest, IRequest<ApiResult>
	{
		public string ContentType { get; set; }

		public string FileName { get; set; }

		public IFormFile File { get; set; }
	}
}
