using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
    public class ForgotPw
    {
        public string email { get; set; }

        public string username { get; set; }

        public string verificationCode { get; set; }

        public string newPassword { get; set; }

        public string confirmPassword { get; set; }
    }
}
