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
		private string location;
		private string description;
		private string category;
		private string friendname;
		private string eventColor;
		private bool bAcceptEvent = false;


		private object[] eventData = new object[9];

		public Events()
		{
		}

		public Events(object[] savedEventData, int nEventID)
		{
			eventData = savedEventData;
			eventid = nEventID;
			eventname = Convert.ToString(eventData.ElementAt(0));
			eventdate = Convert.ToString(eventData.ElementAt(1)).Remove(9);
			startat = Convert.ToString(eventData.ElementAt(2));
			endat = Convert.ToString(eventData.ElementAt(3));
			location = Convert.ToString(eventData.ElementAt(4));
			description = Convert.ToString(eventData.ElementAt(5));
			category = Convert.ToString(eventData.ElementAt(6));
			friendname = Convert.ToString(eventData.ElementAt(7));
			bAcceptEvent = Convert.ToBoolean(eventData.ElementAt(8));
		}


		public string GetEventName()
		{
			return eventname;
		}

		public string GetDate()
		{
			return eventdate;
		}
		public string StartTime()
		{
			return startat;
		}
		public string EndTime()
		{
			return endat;
		}
		public string GetCategory()
		{
			return category;
		}
		public string GetLocation()
		{
			return location;
		}
		public string GetDescription()
		{
			return description;
		}

		public string GetFriendName()
		{
			return friendname;
		}

		public void SetEventColor(string color)
		{
			eventColor = color;
		}
		public string GetEventColor()
		{
			return eventColor;
		}

		public int GetEventID()
		{
			return eventid;
		}
		public bool DidFriendAcceptEvent()
		{
			return bAcceptEvent;
		}
	}
}
