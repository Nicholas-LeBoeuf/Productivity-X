using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Productivity_X.Models
{
    public class UserCreateEvent
    {
        [StringLength(64, MinimumLength = 1)]
        [Required(ErrorMessage = "Please Enter Eventname..")]
        public string eventName { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string event_date { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh-MM-ss am/pm}")]
        [Required(ErrorMessage = "Please Enter time to start at in format: hh:mm:ss am/pm..")]
        public string start_at { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh-MM-ss am/pm}")]
        [Required(ErrorMessage = "Please Enter time to start at in format: hh:mm:ss am/pm..")]
        public string end_at { get; set; }

        public bool notification { get; set; }
        public int reminder { get; set; }
        public string location { get; set; }

        [StringLength(100, MinimumLength = 0)]
        public string description { get; set; }
        public string category { get; set; }
        public bool guest { get; set; }
        public bool friend { get; set; }
        public string guestUsername { get; set; }
        public string guestEmail { get; set; }
    }
}

