using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
    public class CreateEvent
    {
        public string eventName { get; set; }
        public string event_date { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public bool notification { get; set; }
        public int reminder { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public bool guest { get; set; }
        public bool friend { get; set; }
        public string usernameInvite { get; set; }
    }
}
