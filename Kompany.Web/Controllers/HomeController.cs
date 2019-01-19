using Microsoft.AspNetCore.Mvc;

namespace Kompany.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
