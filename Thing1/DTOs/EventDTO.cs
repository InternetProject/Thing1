using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thing1.DTOs
{
    public class EventDto
    {
        public DateTime StartsAt { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}