using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
	public class DBObject
	{
		public static string sUsername;
		public static int nUserID;
		public static int nEventID;

		public string UserName
		{
			get => sUsername;
			set => sUsername = value;
		}

		public int UserID
		{
			get => nUserID;
			set => nUserID = value;
		}

		public int EventID
		{
			get => nEventID;
			set => nEventID = value;
		}
	}
}
