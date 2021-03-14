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
        public IActionResult Weekly()
        {
            // Will need to populate the calendar before opening it, query is in DBManager....

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEvent(UserCreateEvent createEvent)
		{

            bool bRet = false;
            // Checks if all required fields are met
            if (ModelState.IsValid)
            {
                bRet = _manager.SaveEvent(createEvent);
                
                if (!bRet)
                {
                    // Go to Weekly page
                    return View("Weekly");
                }
                else
                {
                    ViewBag.message = "Event Created Successfully!";
                }
            }

            return View("Weekly");
		}
        public IActionResult Today()
        {
            return View();
        }
        public IActionResult ToDo()
        {
            return View();
        }
        public IActionResult Categories()
        {
            return View();
        }
        public IActionResult Friends()
        {
            return View();
        }
    }
}
