using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class ProfileController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Notification()
		{
			return View();
		}
	}
}
