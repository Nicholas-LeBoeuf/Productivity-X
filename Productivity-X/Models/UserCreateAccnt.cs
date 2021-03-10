using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
	public class UserCreateAccnt : DBObject
	{
/*		[Key]
		public int userID { get; set; }
*/
		[StringLength(64, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Firstname..")]
		public string fname { get; set; }

		[StringLength(64, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Lastname..")]
		public string lname { get; set; }

		[StringLength(64, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Username..")]
		public string username { get => UserName; set => DBObject.sUsername = value; }

		[StringLength(60, MinimumLength = 4)]
		[EmailAddress]
		[Required(ErrorMessage = "Please Enter Email..")]
		public string email { get; set; }

		[StringLength(14, MinimumLength = 4)]
		[Required(ErrorMessage = "Please Enter Password..")]
		[DataType(DataType.Password)]
		public string password { get; set; }

		[StringLength(14, MinimumLength = 4)]
		[Required(ErrorMessage = "Please Reenter Password..")]
		[DataType(DataType.Password)]
		[Compare("password")]
		public string confirmPassword { get; set; }
	}
}