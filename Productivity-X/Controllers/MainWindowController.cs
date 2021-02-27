using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Productivity_X.Models;

namespace Productivity_X.Controllers
{
    public class MainWindowController : Controller
    {
        private readonly DBManager _manager;

        public MainWindowController(DBManager manager)
        {
            _manager = manager;
        }

        public IActionResult Main()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginUser(User uc)
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
