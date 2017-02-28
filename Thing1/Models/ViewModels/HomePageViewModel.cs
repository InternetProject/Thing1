using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//View Model for Home Page
namespace Thing1.Models.ViewModels
{
    public class HomePageViewModel
    {
        public List<ClubMembership> clubMemberships { get; set; }
        public List<Event> eventsToDisplay { get; set; }
        public List<Event> myEvents { get; set; }

        public HomePageViewModel()
        {
            clubMemberships = new List<ClubMembership>();
            eventsToDisplay = new List<Event>();
            myEvents = new List<Event>();
        }
    }
}