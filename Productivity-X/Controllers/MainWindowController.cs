using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Controllers
{
    public class MainWindowController : Controller
    {
        public IActionResult Main()
        {
            return View();
        }
    }
}
