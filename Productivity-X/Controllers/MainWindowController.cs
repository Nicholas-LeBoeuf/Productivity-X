using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Productivity_X.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Productivity_X.Controllers
{
    public class MainWindowController : Controller
    {
        private readonly DBManager _manager;

        public MainWindowController(DBManager manager)
        {
            _manager = manager;

        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormCollection form)
        {
            int userid = (int)TempData["userid"];
            string storePath = "../Images/";
            try
            {
                if (form.Files == null || form.Files[0].Length == 0)
                {
                    return RedirectToAction("Main");
                }

                var path = Path.Combine(
                        Directory.GetCurrentDirectory(), storePath,
                        form.Files[0].FileName);

                /*  using (var stream = new FileStream(path, FileMode.Create))
                  {
                      await form.Files[0].CopyToAsync(stream);
                  }
                */

                _manager.SaveUserProfilePicDB(storePath + form.Files[0].FileName, userid);
                TempData["MessageToUser"] = "Profile pic updated!";
                TempData["ProfilePicFromDB"] = _manager.GetProfilePicFromDB(userid);
                TempData.Keep("ProfilePicFromDB");
                TempData["userid"] = userid;
                return RedirectToAction("Main");
            }
            catch(Exception e)
			{
                TempData["MessageToUser"] = "Image not selected!";
                return RedirectToAction("Main");
            }
        }

    
        public IActionResult Main()
        {
            ViewBag.message = TempData["MessageToUser"] as string;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            GetTasksHelper();
            GetCategoriesHelper();
            return View();
        }
        public IActionResult Weekly()
        {
            List<Categories> categoriesSaved = new List<Categories>();
            
            int userid = (int)TempData["userid"];       
            categoriesSaved = _manager.GetCategoriesFromDB(userid);
                
            ViewData["categoryobjects"] = categoriesSaved;
            TempData["userid"] = userid;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View();
        }

// Event Action Results
        public IActionResult CreateEvent(UserCreateEvent createEvent)
        {
            GetCategoriesHelper();
            bool bRet = true;

            /*if((createEvent.guest == true && createEvent.guestEmail == null && createEvent.guestUsername == null) || 
                (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername != null) || (createEvent.guest == false && createEvent.guestEmail != null && createEvent.guestUsername == null || createEvent.category == null) || createEvent.eventName == null)
			{
                bRet = false;
            }*/

            int userid = (int)TempData["userid"];

            // True, save event to the database
            if (bRet)
			{
                bRet = _manager.SaveEvent(createEvent,userid);
				if (bRet)
				{
                    ViewBag.message = "Event saved successfully!";
				}
            }
			else
			{
                ViewBag.message = "Event was not saved, all fields were not filled in!";
            }
            TempData["userid"] = userid;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View("Weekly");
        }

        public IActionResult DeleteEvent(int? eventid)
		{
            int userid = (int)TempData["userid"];
            _manager.DeleteEvent(Convert.ToInt32(eventid), userid);
            GetEventsHelper();
            TempData["userid"] = userid;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
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

        public void GetEventsHelper()
        {
            List<Events> eventsSaved = new List<Events>();

            int userid = (int)TempData["userid"];

            TempData["userid"] = userid;

            // Display events...
            eventsSaved = _manager.GetEventsFromDB(userid);

            ViewData["eventobjects"] = eventsSaved;
        }
        public IActionResult Events()
        {
            GetCategoriesHelper();
            GetEventsHelper();
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View();
        }

        public IActionResult Today()
        {
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View();
        }

        // ToDo Action Results.....
        public IActionResult UpdateStatus(int? taskid)
        {
            // Set userid
            int userid = (int)TempData["userid"];

            bool bComplete = _manager.TaskCompleteFromDB(Convert.ToInt32(taskid), userid);
            _manager.UpdateTask(Convert.ToInt32(taskid), userid, bComplete);

            TempData["userid"] = userid;
            GetTasksHelper();

            return View("ToDo");
        }
        public IActionResult DeleteTask(int? taskid)
        {
            // Set userid
            int userid = (int)TempData["userid"];
  
//            string taskname = _manager.GetTaskNameFromDB(Convert.ToInt32(taskid), userid);
            _manager.DeleteTask(Convert.ToInt32(taskid), userid);

            // Update Category list
            GetTasksHelper();
            TempData["userid"] = userid;
            return View("ToDo");
        }

        public IActionResult CreateTask(UserCreateTask createTask)
		{
            bool bRet = true;

            int userid = (int)TempData["userid"];

            if (createTask.taskName == null)
                bRet = false;

            if (bRet)
            {
                bRet = _manager.SaveTask(createTask, userid);
                if (bRet)
                {
                    ViewBag.message = "Task saved successfully!";
                }
                else
                {
                    ViewBag.message = "Too many tasks created, can only have 10 at a time!";
                }
            }
            else
            {
                ViewBag.message = "Task was not saved, field was not filled in or was filled in incorrectly!";
            }
            GetTasksHelper();
            TempData["userid"] = userid;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View("ToDo");
		}
        public void GetTasksHelper()
        {
            List<ToDoTasks> tasksSaved = new List<ToDoTasks>();

            int userid = (int)TempData["userid"];

            _manager.DeleteOrKeepTasksAfterMidnight(userid);
            tasksSaved = _manager.GetTasksFromDB(userid);
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");

            ViewData["taskobjects"] = tasksSaved;
            TempData["userid"] = userid;
        }

        public IActionResult ToDo()
        {
            GetTasksHelper();
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View();
        }

// Category Action Results.....
        public IActionResult DeleteCategory(int? categoryid)
		{
            // Set userid
            int userid = (int)TempData["userid"];
            // Find Categoryname
            string categoryname = _manager.GetCategoryNameFromDB(Convert.ToInt32(categoryid), userid);
            _manager.DeleteCategory(Convert.ToInt32(categoryid), categoryname, userid);

            // Update Category list
            GetCategoriesHelper();
            TempData["userid"] = userid;
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
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
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View("Categories");
        }
        public void GetCategoriesHelper()
		{
            List<Categories> categoriesSaved = new List<Categories>();
            
            int userid = (int)TempData["userid"];

            categoriesSaved = _manager.GetCategoriesFromDB(userid);

            ViewData["categoryobjects"] = categoriesSaved;
            TempData["userid"] = userid;
        }
        public IActionResult Categories()
        {
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            GetCategoriesHelper();

            return View();
        }

        public IActionResult Friends()
        {
            TempData["ProfilePicFromDB"] = TempData["ProfilePicFromDB"] as string;
            TempData.Keep("ProfilePicFromDB");
            return View();
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CombinedSchedules()
        {
            return View();
        }
    }
}
