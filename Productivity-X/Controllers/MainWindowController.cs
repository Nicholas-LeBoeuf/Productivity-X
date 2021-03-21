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

            List<Categories> categoriesSaved = new List<Categories>();
            List<Events> eventsSaved = new List<Events>();
            // Will need to populate the calendar before opening it, query is in DBManager....
            int userid = (int)TempData["userid"];
            //            int numCategories = _manager.TotalCategories(userid);


            categoriesSaved = _manager.CategoryData(userid);
                
            ViewData["categoryobjects"] = categoriesSaved;
            TempData["userid"] = userid;

            // Display events...
            eventsSaved  = _manager

            return View();
        }

        public IActionResult CreateEvent(UserCreateEvent createEvent)
        {
            bool bRet = true;

            if((createEvent.guest == true && createEvent.guestEmail == null && createEvent.guestUsername == null) || 
                (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername != null) || (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername == null))
			{           
                bRet = false;
			}

            int userid = 0;

            // True, save event to the database
			if (bRet)
			{
                bRet = _manager.SaveEvent(createEvent,userid);
				if (bRet)
				{
                    ViewBag.message = "Event saved successfully!";
				}
				else
				{
                    ViewBag.message = "Event already exists!";
                }
            }
			else
			{
                ViewBag.message = "Event was not saved, not all fields were filled in or were filled in incorrectly!";
            }

/*            bool bRet = false;
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
*/
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
