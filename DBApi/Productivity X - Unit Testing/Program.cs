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
			string connectionstring = "server; user id = ; password = ";
			DBManager manager = new DBManager(connectionstring);

			bool bRet;
			string sRet;
			int nUserid, nNum;
			List<string> data = new List<string>();
			// Get todays date
			string todaysdate = DateTime.Now.ToString("MM-dd-yyyy");

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
			
			UserLogin loginuser = new UserLogin("jason", "password");

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
			CreateEvent createevent = new CreateEvent("class", todaysdate, "000500 pm", "000530 pm", true, 30, "school", "Description", "College", true, true, "Tom", "jasonmckearney2@gmail.com");
			bRet = manager.SaveEvent(createevent);
			Console.WriteLine("Created Event: " + bRet);

		// Category Tables
				
			// Get categoryname data from category_tbl -> will return the color, can use this for color of event..
			List<string> categoryNamesBasedOnUserID = new List<string>();
			categoryNamesBasedOnUserID = manager.CategoryNamesForUserID(DBObject.id);

			Console.WriteLine("category ids and names: ");
			foreach (string idName in categoryNamesBasedOnUserID)
			{
				Console.Write(idName + " ");
			}
			Console.WriteLine("");
			Console.WriteLine("");

			List<string> categoryData = new List<string>();
			// Afer getting all category ids from above list, look for category with id of 1 -> (school, Red, School Label)
			categoryData = manager.CategoryData(Convert.ToInt32(categoryNamesBasedOnUserID[0]));

			Console.WriteLine("data from category table based upon the id:");
			foreach (string sData in categoryData)
			{
				Console.Write(sData + " ");
			}
				
			EditEvent editEvent = new EditEvent("class", todaysdate, "083000 am", "101500 am", true, 30, "school", "Description", "College", false, false);
			bRet = manager.EditEvent(editEvent);
			Console.WriteLine("Edited Event:" + bRet);

			// Find event info, pass back from the database
			data = manager.FindEventInfo(DBObject.eventid);

			Console.WriteLine("");
			Console.WriteLine("data extracted from events_tbl");
			foreach (string str in data)
			{
				Console.WriteLine(str);
			}
			
		//-------------Weekly Calendar---------------
			
			// Might need queries below and build the date off of the labels being shown on screen.......
			List<string> todayEventIDs = new List<string>();
			// Passes back event ids:
			todayEventIDs = manager.FindTodaysEvents(todaysdate);

			// Find data for each event based upon the eventid:
			foreach (string sEventID in todayEventIDs)
			{
				// Get event info for each id
				data = manager.FindEventInfo(Int32.Parse(sEventID));
				Console.WriteLine("Found data for id:" + sEventID);
			}

		//-----------Today Button queries------------
			List<string> todayEventIDs2 = new List<string>();
			// Passes back event ids:
			todayEventIDs = manager.FindTodaysEvents(todaysdate);

			// Find data for each event based upon the eventid:
			foreach (string sEventID in todayEventIDs2)
			{
				// Get event info for each id
				data = manager.FindEventInfo(Int32.Parse(sEventID));
				Console.WriteLine("Found data for id:" + sEventID);
			}

		// Create, edit and delete a category:

		// Create, add, delete a friend

		// main screen functionalites with database:




			// Work on this if needed...
/*			// Delete Event button
			bRet = manager.DeleteEvent(eventid);
			Console.WriteLine("Event has been deleted: " + bRet);
*/
		

		}
	}
}
