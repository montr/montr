using Microsoft.AspNetCore.Mvc;

namespace Tendr.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
