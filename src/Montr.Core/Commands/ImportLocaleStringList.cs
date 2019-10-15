using System.IO;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class ImportLocaleStringList : LocaleStringSearchRequest, IRequest<ApiResult>
	{
		public string ContentType { get; set; }

		public string FileName { get; set; }

		public Stream Stream { get; set; }
	}
}
