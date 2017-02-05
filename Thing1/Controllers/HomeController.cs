using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Thing1.Models;
using System.Web.Mvc;
using System.Data.Entity;
using Thing1.Models.ViewModels;

namespace Thing1.Controllers
{
    public class HomeController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized); // should change this later.
            }
            var userid = User.Identity.GetUserId();
            var clubMemberships = db.ClubMemberships.Include(c => c.Club).Where(c => c.UserId == userid).ToList();
            var numEventsToReturn = 4;
            var upcomingEvents = db.Events.Where(e => e.EndsAt > DateTime.Now).OrderBy(e => e.StartsAt).Take(numEventsToReturn).ToList();
            var homePageData = new HomePageViewModel();
            var myEventRSVPS = db.EventsRSVPs.Include(e => e.Event).Where(r => r.AspNetUser.Id == userid).Where(r => r.Event.EndsAt > DateTime.Now).OrderBy(r => r.Event.StartsAt).Take(numEventsToReturn).ToList();
            foreach (var rsvp in myEventRSVPS)
            {
                homePageData.myEvents.Add(rsvp.Event);
            }
            homePageData.clubMemberships = clubMemberships;
            homePageData.eventsToDisplay = upcomingEvents;
            return View(homePageData);
        }
    }
}