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
        
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(UserCreateAccnt uc)
		{
            bool bRet = false;
            // Checks if all required fields are met
            if (ModelState.IsValid)
            {
                bRet = _manager.SaveUser(uc);

                if (!bRet)
                {
                    // Go to Login page
                    return View("Index");
                }
                else
                {
                    ViewBag.message = "Username taken, choose a different one!";
                }
            }
            // Go to Create Account page
            return View("CreateAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginUser(UserLogin loginUser)
        {
            bool bUserExists = false;

            if (ModelState.IsValid)
            {
                bUserExists = _manager.LoadUser(loginUser);
                if (bUserExists)
                {
                    // Go to main screen
                    return View("~/Views/Mainwindow/Main.cshtml");
                }
                else
                {
                    // Error message pops up on screen
                    ViewBag.message = "Username not found or password incorrect!";
                }
            }
            // Go to Login screen
            return View("Index");
        }
    }
}
