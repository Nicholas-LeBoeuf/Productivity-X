using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
	public class User
	{
		[Key]
		public int userID { get; set; }

		[Required(ErrorMessage = "Please Enter Firstname..")]
		public string firstname { get => firstname; set => firstname = ""; }

		[Required(ErrorMessage = "Please Enter Lastname..")]
		public string lastname { get => lastname; set => firstname = ""; }

		[Required(ErrorMessage = "Please Enter sername..")]
		public string username { get => username; set => firstname = ""; }

		[Required(ErrorMessage = "Please Enter your email..")]
		public string email { get => email; set => firstname = ""; }

		[Required(ErrorMessage = "Please Enter first name..")]
		public string password { get => password; set => firstname = ""; }
		public string confirmpassword { get => confirmpassword; set => firstname = ""; }
		public string verificationcode { get => verificationcode; set => firstname = ""; }
	}
}
