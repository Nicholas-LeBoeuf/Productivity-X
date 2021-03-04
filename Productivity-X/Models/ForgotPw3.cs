using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
	public class ForgotPw3
	{
		[StringLength(14, MinimumLength = 4)]
		[Required(ErrorMessage = "Please Enter Password..")]
		[DataType(DataType.Password)]
		public string newPassword { get; set; }

		[StringLength(14, MinimumLength = 4)]
		[Required(ErrorMessage = "Please Reenter Password..")]
		[DataType(DataType.Password)]
		[Compare("newPassword")]
		public string confirmPassword { get; set; }
	}
}
