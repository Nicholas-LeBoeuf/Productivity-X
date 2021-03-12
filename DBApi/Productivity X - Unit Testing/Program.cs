using System;
using System.Collections.Generic;
using Productivity_X___Unit_Testing.Models;

namespace Productivity_X___Unit_Testing
{
	class Program
	{
		static void Main(string[] args)
		{
			// Please use your own database server, tested with local database on MySQL Workbench
			string connectionstring = "server=localhost;user id=root;password=MhiglayJAV1";
			DBManager manager = new DBManager(connectionstring);

			bool bRet;
			string sRet;
			int nUserid, nNum;

		//---------------Create an Account:---------------
			// Testing create account and save into database:
			UserCreateAccount createAccount = new UserCreateAccount("jasonmckearney2@gmail.com", "jason", "McKearney", "test", "test", "jason");
			// True if saved successfully
			bRet = manager.SaveUser(createAccount);
			Console.WriteLine("Save user: " + bRet);
			nNum = manager.GetUserID();
			Console.WriteLine("Get userID: " + nNum);

			// Get the password for jason
			sRet = manager.GetPassword();
			Console.WriteLine("Found Password: " + sRet);


			//---------------Update password for a user:---------------
			// Test for updating password based upon email, username, etc..
			// Type in fields, username, email, new password:
			ForgotPassword forgetpassword = new ForgotPassword("jasonmckearney2@gmail.com", "admin", "admin", "jason", "52541d6");
			// Get userid
			nUserid = manager.GetUserID(forgetpassword);
			Console.WriteLine("Get userID: " + nUserid);

			// Save the security code to database:
			manager.SaveSecurityCode("52541d6");
			// Get the security from database:
			bRet = manager.CheckSecurityCode(forgetpassword);
			Console.WriteLine("Check security code: " + bRet);

			// Get new password for jason
			sRet = manager.GetPassword();
			Console.WriteLine("Updated Password: " + sRet);


			//---------------Login User:---------------
			UserLogin loginuser = new UserLogin("jason","password");

			//Returns true if valid password for username:
			bRet = manager.CheckPassword(loginuser);
			Console.WriteLine("Check password for jason and password: password: " + bRet);

			// Returns true if successfull
			bRet = manager.LoadUser(loginuser);
			Console.WriteLine("Load User: " + bRet);

			// Get the password for jason
			sRet = manager.GetPassword();
			Console.WriteLine("Get Password: " + sRet);

			// Get username
			sRet = manager.GetUserName();
			Console.WriteLine("Username based upon userid: " + sRet);

			loginuser.UserName = "jason";
			loginuser.password = "admin";

			manager.LoadUser(loginuser);
			// Get userid
			nNum = manager.GetUserID();
			Console.WriteLine("Get userid: " + nNum);

			// Get username
			sRet = manager.GetUserName();
			Console.WriteLine("Get username: " + sRet);

			nNum = manager.CountUsers();
			Console.WriteLine("Number of users created: " + nNum);

		//-----------Create event for calendar-----------
			Console.WriteLine("");
			Console.WriteLine("Create event for Calendar test cases");
			CreateEvent createevent = new CreateEvent("class", "2021-03-15", "000500 pm", "000530 pm", true, 30, "school", "Description", "red", true, true);
			bRet=manager.SaveEvent(createevent);
			Console.WriteLine("Created Event: " + bRet);

			EditEvent editEvent = new EditEvent("class", "2021-03-15", "083000 am", "101500 am", true, 30, "school", "Description", "red", false, false);
			bRet = manager.EditEvent(editEvent);
			Console.WriteLine("Edited Event:" + bRet);

			List<string> data = new List<string>();
			data = manager.FindEventInfo();

			Console.WriteLine("");
			Console.WriteLine("data extracted from events_tbl");
			foreach(string str in data)
			{
				Console.WriteLine(str);
			}


		//-----------Today Button query------------
			string todaysdate = DateTime.Now.ToString("MM-dd-yyyy");
			manager.FindTodaysEvents(todaysdate);



			bRet = manager.DeleteEvent();
			Console.WriteLine("Event has been deleted: " + bRet);



		}
	}
}
