using Microsoft.AspNetCore.Mvc;

namespace tendr.core.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}