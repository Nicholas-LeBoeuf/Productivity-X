using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Productivity_X.Models
{
    public class FriendsView
    {
        public int id { get; set; }
        public int friendid { get; set; }
        public string friendname { get; set; }
        public string friendemail { get; set; }
        public bool request { get; set; }        
        public string profilepic { get; set; }

    }
}