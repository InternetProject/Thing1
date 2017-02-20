using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Thing1.Models.ViewModels
{
    public class ClubAndEventsViewModel
    {
        public Club club { get; set; }
        public IEnumerable<Event> Events { get; set; }
     }
}