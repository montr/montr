using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class GetMenuHandler : IRequestHandler<GetMenu, Menu>
	{
		private readonly IContentService _contentService;

		public GetMenuHandler(IContentService contentService)
		{
			_contentService = contentService;
		}

		public Task<Menu>  Handle(GetMenu request, CancellationToken cancellationToken)
		{
			var menu = _contentService.GetMenu(request.MenuId);

			return Task.FromResult(menu);
		}
	}
}
