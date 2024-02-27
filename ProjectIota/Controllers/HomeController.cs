using Microsoft.AspNetCore.Mvc;
using ProjectIota.Data;
using ProjectIota.Models;
using ProjectIota.Process;
using System.Diagnostics;

namespace ProjectIota.Controllers
{
    public class HomeController : Controller
    {
        private DataContext dataContext;

        public HomeController(DataContext context)
        {
            dataContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}