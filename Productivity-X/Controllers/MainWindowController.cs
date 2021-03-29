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
        //    List<Events> eventsSaved = new List<Events>();
            
            int userid = (int)TempData["userid"];
            //            int numCategories = _manager.TotalCategories(userid);


            categoriesSaved = _manager.CategoryData(userid);
                
            ViewData["categoryobjects"] = categoriesSaved;
            TempData["userid"] = userid;

        //    // Display events...
        //    eventsSaved = _manager.EventData(userid);


            return View();
        }

        public IActionResult CreateEvent(UserCreateEvent createEvent)
        {
            GetCategoriesHelper();
            bool bRet = true;

            if((createEvent.guest == true && createEvent.guestEmail == null && createEvent.guestUsername == null) || 
                (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername != null) || (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername == null || createEvent.category == null))
			{
                bRet = false;
            }

            int userid = (int)TempData["userid"];

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
            TempData["userid"] = userid;
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

        public IActionResult DeleteEvent(int? eventid)
		{
            _manager.DeleteEvent(Convert.ToInt32(eventid));
            GetEventsHelper();
            return View("Events");
		}

        public IActionResult GetWeeklyEvents()
        {
            int userid = (int)TempData["userid"];
            TempData["userid"] = userid;
            return Json(_manager.GetWeeklyEvents(userid));
        }
        public IActionResult GetTodayEvents()
        {
            int userid = (int)TempData["userid"];
            TempData["userid"] = userid;
            return Json(_manager.GetTodayEvents(userid));
        }
        public IActionResult Today()
        {
            return View();
        }
        public IActionResult ToDo()
        {
            return View();
        }
        public IActionResult DeleteCategory(int? categoryid)
		{
            // Set userid
            int userid = (int)TempData["userid"];
            // Find Categoryname
            string categoryname = _manager.GetCategoryName(Convert.ToInt32(categoryid), userid);
            _manager.DeleteCategory(Convert.ToInt32(categoryid), categoryname);

            // Update Category list
            GetCategoriesHelper();
            TempData["userid"] = userid;
            return View("Categories");
		}

        public IActionResult CreateCategory(UserCreateCategory createCategory)
        {
            bool bRet = true;

            int userid = (int)TempData["userid"];

            if (createCategory.categoryname == null && createCategory.color == null)
            {
                bRet = false;
            }

            if (bRet)
            {
                bRet = _manager.SaveCategory(createCategory, userid);
                if (bRet)
                {
                    ViewBag.message = "Category saved successfully!";
                }
                else
                {
                    ViewBag.message = "Category or color already exists!";
                }
            }
            else
            {
                ViewBag.message = "Category was not saved, not all fields were filled in or were filled in incorrectly!";
            }
            GetCategoriesHelper();
            TempData["userid"] = userid;
            return View("Categories");
        }
        public void GetCategoriesHelper()
		{
            List<Categories> categoriesSaved = new List<Categories>();
            
            int userid = (int)TempData["userid"];

            categoriesSaved = _manager.CategoryData(userid);

            ViewData["categoryobjects"] = categoriesSaved;
            TempData["userid"] = userid;
        }
        public IActionResult Categories()
        {
            GetCategoriesHelper();

            return View();
        }

        public IActionResult Friends()
        {
            return View();
        }

        public void GetEventsHelper()
		{
            List<Events> eventsSaved = new List<Events>();

            int userid = (int)TempData["userid"];

            TempData["userid"] = userid;

            // Display events...
            eventsSaved = _manager.EventData(userid);

            ViewData["eventobjects"] = eventsSaved;
        }
        public IActionResult Events()
        {
            GetEventsHelper();    
            return View();
        }


    }
}
