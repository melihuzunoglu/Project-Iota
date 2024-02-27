using FastReport.Data;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ProjectIota.Data;
using ProjectIota.Process;

namespace ProjectIota.Controllers
{
	[Route("[controller]")]
	public class ReportController : Controller
	{
		private DataContext dataContext;
		private readonly IDistributedCache cache;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ReportController(DataContext _dataContext, IDistributedCache _cache, IWebHostEnvironment webHostEnvironment)
		{
			dataContext = _dataContext;
			cache = _cache;
			this._webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			return View();
		}

		[Route("ShowReport/{type}/{payload}")]
		public IActionResult ShowReport(string type, string payload)
		{
			if (HttpContext.Session.GetString("UserId") == null)
			{
				return RedirectToAction("Index", "UserLogin");
			}

			WebMenuProcess webMenuProcess = new WebMenuProcess();
			ViewBag.menuItems = webMenuProcess.GetWebMenuList(dataContext, HttpContext, cache);


			WebReport webReport = new WebReport();

			var path = "/app/wwwroot/Reports/CategoryList.frx";
			webReport.Report.Load(path);

			webReport.Report.SetParameterValue("Parameter", payload);

			return View(webReport);
		}
	}
}
