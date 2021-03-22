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
		private object[] eventData = new object[11];

		public Events()
		{
		}

		public Events(object[] savedEventData, int nEventID, int nReminder)
		{
			eventData = savedEventData;
			eventid = nEventID;
			eventname = Convert.ToString(eventData.ElementAt(0));
			eventdate = Convert.ToString(eventData.ElementAt(1));
			startat = Convert.ToString(eventData.ElementAt(2));
			endat = Convert.ToString(eventData.ElementAt(3));
			notification = Convert.ToBoolean(eventData.ElementAt(4));
			reminder = nReminder;
			location = Convert.ToString(eventData.ElementAt(6));
			description = Convert.ToString(eventData.ElementAt(7));
			category = Convert.ToString(eventData.ElementAt(8));
			guest = Convert.ToBoolean(eventData.ElementAt(9));
			friend = Convert.ToBoolean(eventData.ElementAt(10));
		}

		public string GetEventName()
		{
			return eventname;
		}
	}
}
