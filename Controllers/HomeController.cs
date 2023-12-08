using Microsoft.AspNetCore.Mvc;
using SportsStore.Common;
using SportsStore.Data;
using SportsStore.Models;
using System.Data;
using System.Diagnostics;

namespace SportsStore.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Soccer"] = Convert.ToInt32(S_SOCCER_PERCENTAGE);
            ViewData["Watersports"] = Convert.ToInt32(S_WATERSPORTS_PERCENTAGE);
            ViewData["Chess"] = Convert.ToInt32(S_CHESS_PERCENTAGE);
            ViewData["Cricket"] = Convert.ToInt32(S_CRICKET_PERCENTAGE);

            // Trying Raw sql query
            //var prod = RawSqlQuery.GetDynamicSqlValue(SP.rawProducts);

            return View();
        }


        public IActionResult Privacy()
        {
            HttpContext.Session.SetString(Constant.SessionTest, "This page is under development.");
            ViewBag.Message = HttpContext.Session.GetString(Constant.SessionTest);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}