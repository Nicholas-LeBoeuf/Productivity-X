using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productivity_X___Unit_Testing.Models
{
	class UserLogin : DBObject
	{
		public string password;

		public UserLogin(string sUsername,string sPassword)
		{
			DBObject.username = sUsername;
			password = sPassword;
		}
	}
}
