using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class ExportLocaleStringList : LocaleStringSearchRequest, IRequest<FileResult>
	{
	}
}
