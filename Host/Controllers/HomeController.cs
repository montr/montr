using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}
