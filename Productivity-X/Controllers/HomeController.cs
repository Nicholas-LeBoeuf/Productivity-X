using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Productivity_X.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Controllers
{
    public class HomeController : Controller
    {
//        private readonly ILogger<HomeController> _logger;

        private readonly DBManager _manager;
//        private User uc = new User();

/*        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
*/

        public HomeController(DBManager manager)
		{
            _manager = manager;
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

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(UserCreateAccnt uc)
		{
            bool bRet = false;
            if (ModelState.IsValid)
            {
                bRet = _manager.SaveUser(uc);

                if (!bRet)
                {
                    return View("Index");
                }
                else
                {
                    ViewBag.message = "Username taken, choose a different one!";
                }
            }
            return View("CreateAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginUser(UserLogin loginUser)
        {
            bool bUserExists = false;

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                bUserExists = _manager.LoadUser(loginUser);
                if (bUserExists)
                {
                    return View("~/Views/Mainwindow/Main.cshtml");
                }
                else
                {
                    ViewBag.message = "Username not found or password incorrect!";
                }
            }
            return View("Index");
        }
    }
}
