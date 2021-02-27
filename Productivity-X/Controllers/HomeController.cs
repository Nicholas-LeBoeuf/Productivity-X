﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult AddUser(User uc)
		{
            bool bUserExists = false;
            if (ModelState.IsValid)
            {
                //                uc = _manager.CreateUser();
                bUserExists = _manager.SaveUser(uc);
                if (!bUserExists)
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
    }
}
