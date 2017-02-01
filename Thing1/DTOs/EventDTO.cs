using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thing1.DTOs
{
    public class EventDto
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public List<String> Clubs { get; set; }
        public string Food { get; set; }
        public string Contact { get; set; }
        public string Price { get; set; }
    }
}