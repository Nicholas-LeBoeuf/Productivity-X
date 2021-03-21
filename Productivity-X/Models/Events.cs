using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
	public class Events
	{
		private int eventid;
		private string eventname;
		private string eventdate;
		private string startat;
		private string endat;
		private bool notification;
		private int reminder;
		private string location;
		private string description;
		private string category;
		private bool guest;
		private bool friend;
		private string guestusername;
		private string guestemail;
		private object[] eventData = new object[12];

		public Events()
		{

		}

		public Events(object[] savedEventData, int nEventID)
		{
			eventData = savedEventData;
			eventid = nEventID;
			eventname = Convert.ToString(eventData.ElementAt(0));
			// Finish setting private attributes above




			//categoryid = Convert.ToInt32(categoryData.ElementAt(1).ToString());
			categoryid = nCategoryID;
			categoryname = Convert.ToString(categoryData.ElementAt(0));
			color = Convert.ToString(categoryData.ElementAt(1));
			description = Convert.ToString(categoryData.ElementAt(2));
		}

		public string GetCategoryName()
		{
			return categoryname;
		}
	}
}
