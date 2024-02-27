using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProjectIota.Data;
using ProjectIota.Models;
using ProjectIota.Process;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectIota.Controllers
{
	[Route("[controller]")]
	public class UserLoginController : Controller
	{
		private DataContext dataContext;
		private readonly IDistributedCache cache;

		public UserLoginController(DataContext _dataContext, IDistributedCache _cache)
		{
			dataContext = _dataContext;
			cache = _cache;
		}

		[Route("Index")]
		public IActionResult Index()
		{
			if (HttpContext.Session.GetString("UserId") != null)
			{
				return RedirectToAction("Index", "Dashboard");
			}

			return View();
		}

		[HttpPost]
		public IActionResult Login(User user)
		{
			if (user == null)
			{
				ViewBag.LoginStatus = 0;//Payload is null
			}
			else
			{
				if (user.EMail == null || user.Password == null)
				{
					ViewBag.LoginStatus = 1;//All fields must be filled
				}
				else
				{
					var userData = dataContext.User.Where(x => x.EMail == user.EMail & x.Password == user.Password).FirstOrDefault();
					User userDb = userData;
					if (userDb == null)
					{
						ViewBag.LoginStatus = 2;//Email or password is incorrect
					}
					else
					{
						ViewBag.LoginStatus = 3;//Login successful;
						HttpContext.Session.SetString("UserEMail", userDb.EMail);
						HttpContext.Session.SetString("UserId", userDb.UserId.ToString());
						return RedirectToAction("Index", "Dashboard");
					}
				}
			}

			return View("Index");
		}
	}
}
