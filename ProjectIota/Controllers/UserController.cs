using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectIota.Data;
using ProjectIota.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectIota.Controllers
{
	[Route("[controller]")]
	public class UserController : Controller
	{
		private DataContext dataContext;

		public UserController(DataContext context)
		{
			dataContext = context;
		}

		[Route("Index")]
		public IActionResult Index()
		{
			return View();
		}

		[Route("Logout")]
		public IActionResult Logout(User user)
		{
            HttpContext.Session.Remove("UserEMail");
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Index", "Home");
		}
	}
}
