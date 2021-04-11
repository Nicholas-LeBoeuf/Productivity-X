 using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Productivity_X.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Productivity_X.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBManager _manager;

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

        // Login user screen, first screen seen as user opens applications
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginUser(UserLogin loginUser)
        {
            bool bUserExists = false;
            bool bOldEventsExist = false;

            if (ModelState.IsValid)
            {
                TempData["username"] = loginUser.username;
                int nUserID = _manager.GetUserID(TempData["username"] as string);
                bUserExists = _manager.CheckPassword(loginUser, TempData["username"] as string, nUserID);
                TempData["userid"] = nUserID;

                _manager.LoadUser(loginUser, ref nUserID, loginUser.username);
//                TempData["userid"] = nUserID;
                if (bUserExists)
                {
                    // Delete events that are older than 10 days...
                    bOldEventsExist = _manager.DeleteEventsGreaterThan10Days(nUserID);
					if (bOldEventsExist)
					{
                        ViewBag.message = "Deleted events that are greater than 10 days!";
					}

                    GetCategoriesHelper();



                    // Get profile from DB
                    string filename = _manager.GetProfilePicFromDB(nUserID);
                    ViewData["ProfilePicFromDB"] = filename;




                    // Go to main screen
                    return View("~/Views/MainWindow/Main.cshtml");
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

        public void GetCategoriesHelper()
        {
            List<Categories> categoriesSaved = new List<Categories>();
            List<ToDoTasks> tasksSaved = new List<ToDoTasks>();

            int userid = (int)TempData["userid"];

            categoriesSaved = _manager.GetCategoriesFromDB(userid);
            tasksSaved = _manager.GetTasksFromDB(userid);

            ViewData["categoryobjects"] = categoriesSaved;
            ViewData["taskobjects"] = tasksSaved;
            TempData["userid"] = userid;
        }

        // Create accounts screens
        public IActionResult CreateAccount()
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
                    string filename = "~Productivity-X/wwwroot/Images/ProductivityXLogin";
                    // Set default profile image into database...
                    _manager.SaveUserProfilePicDB(filename, _manager.GetUserID(uc.username));

                    ViewData["ProfilePicFromDB"] = filename;

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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot password screens
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterUsernameEmail(ForgotPw1 forgotpassword)
        {
            SecurityCodeGenerator securityCode = new SecurityCodeGenerator();
            string sCode;
            int userid;

            // Checks if all required fields are met
            if (ModelState.IsValid)
            {
                // Save username
                TempData["username"] = forgotpassword.username;
                userid = _manager.GetUserID(forgotpassword, TempData["username"] as string);
                TempData["userid"] = userid;

                if (userid!= -1)
                {
                    sCode = securityCode.GetSecurityCode();

                    _manager.SaveSecurityCode(sCode, userid);
//                    _manager.m_UserID = userid;

                    // Email 
                    MimeMessage message = new MimeMessage();
                    MailboxAddress from = new MailboxAddress("Productivity X",
                    "productivityx2021@gmail.com");
                    message.From.Add(from);

                    MailboxAddress to = new MailboxAddress("User",
                    forgotpassword.email.ToString());
                    message.To.Add(to);
                    message.Subject = "Productivity X Security Code";

                    message.Body = new TextPart("plain")
                    {
                        Text = @"Code: " + sCode.ToString()
                    };

                    // Connect and authenticate with SMTP server
                    SmtpClient client = new SmtpClient();
                    client.Connect("smtp.gmail.com", 465, true);
					client.Authenticate("productivityx2021@gmail.com", "ProDucTIvityx#$2021");

                    // Send email and then disconnect
                    client.Send(message);
                    client.Disconnect(true);
                    client.Dispose();

                    // Go to "enter verification code" screen
                    return View("ForgotPassword2");
                }
                else
                {
                    ViewBag.message = "Username or email not found!";
                }
            }
            return View("ForgotPassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterSecurityCode(ForgotPw2 forgotpw2)
        {
            bool bValidSecurityCode;
            if (ModelState.IsValid)
            {
                int nUserid = (int)TempData["userid"];

                bValidSecurityCode = _manager.CheckSecurityCode(forgotpw2, nUserid);
                TempData["userid"] = nUserid;

                if (bValidSecurityCode)
                {
                    // Go to update password screen
                    return View("ForgotPassword3");
                }
                else
                {
                    // Error message pops up on screen
                    ViewBag.message = "Invalid Security Code!";
                }
            }
            // Go to Login screen
            return View("ForgotPassword2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnterNewPassword(ForgotPw3 forgotpw3)
        {
            if (ModelState.IsValid)
            {
                _manager.UpdatePassword(forgotpw3,(int)TempData["userid"]);
                
                // Message pops up on screen if successful
                ViewBag.message = "Password Updated!";
                // Go to update password screen
                return View("Index");
            }
            // Go to Login screen
            return View("ForgotPassword3");
        }
    }
}
