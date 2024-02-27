using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIota.Data;
using ProjectIota.Models;

namespace ProjectIota.Controllers
{
    [Route("[controller]")]
    public class WebMenuController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
