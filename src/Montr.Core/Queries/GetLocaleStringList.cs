using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries;

public class GetLocaleStringList : LocaleStringSearchRequest, IRequest<SearchResult<LocaleString>>
{
}
