﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Productivity_X.Models
{
	public class ChangeProfileImage
	{
		public IFormFile ProfileImage { get; set; }
	}
}
