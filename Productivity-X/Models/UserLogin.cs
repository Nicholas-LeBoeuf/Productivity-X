using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
	public class UserLogin
	{
/*		[Key]
		public int userID { get; set; }
*/
		[StringLength(64, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Username..")]
		public string username { get; set; }

		[StringLength(14, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Password..")]
		[DataType(DataType.Password)]
		public string password { get; set; }
	}
}
