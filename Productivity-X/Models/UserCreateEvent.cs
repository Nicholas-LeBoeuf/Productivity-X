﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
    public class UserCreateEvent
    {
        public string eventName { get; set; }
        public string event_date { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string friendUsername { get; set; }

    }
}
