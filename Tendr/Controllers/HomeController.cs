using Microsoft.AspNetCore.Mvc;

namespace Tendr.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
