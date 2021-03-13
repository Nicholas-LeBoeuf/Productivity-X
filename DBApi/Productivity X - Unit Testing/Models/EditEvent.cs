using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productivity_X___Unit_Testing.Models
{
	class EditEvent : DBObject
	{
		public string m_sEventName;
		public string m_sEventDate;
		public string m_sTimeStartAt;
		public string m_sTimeEndAt;
		public bool m_nNotification;
		// Reminder could be 30 or 15 minutes from 
		public int m_nReminder;
		public string m_sLocation;
		public string m_sDescription;
		public string m_sCategory;

		public bool m_bGuest;
		public string m_sGuestUsername;
		public string m_sGuestEmail;
		public bool m_bIsFriend;

		public EditEvent(string sNameEvent, string sEventDate, string nTimeStart_At, string nTimeEnd_At, bool nNotification, int nReminder, string sLocation,
			string sDescription, string sCategory, bool bGuest, bool bisFriend, string sGuestUsername = "", string sGuestEmail = "")
		{
			m_sEventName = sNameEvent;
			m_sEventDate = sEventDate;
			m_sTimeStartAt = nTimeStart_At;
			m_sTimeEndAt = nTimeEnd_At;
			m_nNotification = nNotification;
			m_nReminder = nReminder;
			m_sLocation = sLocation;
			m_sDescription = sDescription;
			m_sCategory = sCategory;

			m_bGuest = bGuest;
			m_sGuestUsername = sGuestUsername;
			m_sGuestEmail = sGuestEmail;
			m_bIsFriend = bisFriend;
		}
	}
}
