using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Productivity_X___Unit_Testing.Models
{
	class ForgotPassword : DBObject
	{
		public string email;
		public string password;
		public string confirmPassword;
		public string verificationCode;

		public ForgotPassword(string sEmail, string sPassword, string sConfirmpass, string sUserName, string sVerificationCode)
		{
			email = sEmail;
			password = sPassword;
			confirmPassword = sConfirmpass;
			verificationCode = sVerificationCode;
			DBObject.username = sUserName;
		}
	}
}
