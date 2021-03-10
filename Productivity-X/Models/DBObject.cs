using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
	public class DBObject
	{
		public static string sUsername;
		public static int sID;

		public string UserName
		{
			get => sUsername;
			set => sUsername = value;
		}

		public int UserID
		{
			get => sID;
			set => sID = value;
		}
	}
}
