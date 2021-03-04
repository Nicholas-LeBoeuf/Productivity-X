using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
	public class ForgotPw2
	{
		[StringLength(16, MinimumLength = 1)]
		[Required(ErrorMessage = "Please Enter Security Code..")]
		public string verificationCode { get; set; }
	}
}
