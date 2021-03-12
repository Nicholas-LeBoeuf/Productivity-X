using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Productivity_X___Unit_Testing.Models
{
	class DBObject
	{
		public static string username;
        public static int id;
		public static int eventid;
			
		public string UserName
		{
			get => username;
			set => username = value;
		}

		public int UserID
		{
			get => id;
			set => id = value;
		}

		public int EventID
		{
			get => eventid;
			set => eventid = value;
		}
	}
}
