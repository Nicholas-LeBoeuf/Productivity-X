﻿ using Microsoft.AspNetCore.Mvc;
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
                userid = _manager.GetUserID(forgotpassword);

                if (userid!= -1)
                {
                    sCode = securityCode.GetSecurityCode();

                    _manager.SaveSecurityCode(userid, sCode);
                    _manager.m_UserID = userid;

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
                bValidSecurityCode = _manager.GetSecurityCode(_manager.m_UserID, forgotpw2);
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
                _manager.UpdatePassword(_manager.m_UserID, forgotpw3);
                
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
