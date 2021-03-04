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
//        private readonly ILogger<HomeController> _logger;

        private readonly DBManager _manager;
        private int m_UserID = 0;
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
        public IActionResult ForgotPassword2(ForgotPw1 forgotpassword)
        {
            int nID;
            SecurityCodeGenerator securityCode = new SecurityCodeGenerator();
            string sCode;

            // Checks if all required fields are met
            if (ModelState.IsValid)
            {
                nID = _manager.GetUserID(forgotpassword);

                if (nID != -1)
                {
                    m_UserID = nID;
                    sCode = securityCode.GetSecurityCode();

                    _manager.SaveSecurityCode(nID, sCode);

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

                    // Go to Login page
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
        public IActionResult ForgotPassword3()
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
                bUserExists = _manager.CheckPassword(loginUser);
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
