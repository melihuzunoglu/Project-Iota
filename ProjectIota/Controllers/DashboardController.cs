using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProjectIota.Data;
using ProjectIota.Models;
using ProjectIota.Process;
using System.Diagnostics;

namespace ProjectIota.Controllers
{
	public class DashboardController : Controller
	{
		private DataContext dataContext;
		private readonly IDistributedCache cache;

		public DashboardController(DataContext _dataContext, IDistributedCache _cache)
		{
			dataContext = _dataContext;
			cache = _cache;
		}
 
        public IActionResult Index()
		{
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "UserLogin");
            }
 
            WebMenuProcess webMenuProcess = new WebMenuProcess();
            ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);

            return View();
		}
	}
}
