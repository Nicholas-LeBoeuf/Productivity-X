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
            return View();
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
        public ActionResult CreateEvent()
        {
            return PartialView();
        }
    }
}
