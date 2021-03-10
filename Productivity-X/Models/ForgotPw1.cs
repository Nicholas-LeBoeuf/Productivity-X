using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
    public class ForgotPw1 : DBObject
    {
        [StringLength(64, MinimumLength = 1)]
        [Required(ErrorMessage = "Please Enter Username..")]
        public string username { get => UserName; set => DBObject.sUsername = value; }

        [StringLength(60, MinimumLength = 4)]
        [EmailAddress]
        [Required(ErrorMessage = "Please Enter Email..")]
        public string email { get; set; }
    }
}
