using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class ListController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
