using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Web.Models;
using Montr.Web.Services;

namespace Montr.Web.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ContentController : ControllerBase
	{
		private readonly IContentProvider _contentProvider;

		public ContentController(IContentProvider contentProvider)
		{
			_contentProvider = contentProvider;
		}

		[HttpPost]
		public async Task<ActionResult<Menu>> Menu(ContentRequest request)
		{
			return await _contentProvider.GetMenu(request.MenuId);
		}
	}
}
