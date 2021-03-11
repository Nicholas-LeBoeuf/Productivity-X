using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productivity_X___Unit_Testing.Models
{
	class UserCreateAccount : DBObject
	{
		public string email;
		public string fname;
		public string lname;
		public string password;
		public string confirmPassword;

		public UserCreateAccount(string sEmail, string sFName, string sLName, string sPassword, string sConfirmpass, string sUsername)
		{
			email = sEmail;
			fname = sFName;
			lname = sLName;
			password = sPassword;
			confirmPassword = sConfirmpass;

			DBObject.username = sUsername;
		}
	}
}
