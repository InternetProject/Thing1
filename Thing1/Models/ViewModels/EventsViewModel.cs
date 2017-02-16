using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Thing1.Models.ViewModels
{
    public class EventsViewModel
    {
        public PagedList.IPagedList<Thing1.Models.Event> events { get; set;}

        public PagedList.IPagedList<Thing1.Models.Event> myEvents { get; set; }

        public List<Thing1.Models.Club> clubs { get; set; }
    }
}