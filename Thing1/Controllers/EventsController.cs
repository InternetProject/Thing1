using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;
using PagedList;
using Thing1.Models.ViewModels;
using Microsoft.AspNet.Identity;


namespace Thing1.Controllers
{
    public class EventsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();
        // GET: Events

        public ViewResult Index(string currentFilter, string searchString, int? clubID, int? page)
        {
            string userId = User.Identity.GetUserId();

            List<int> clubs = new List<int>();
            List<ClubMembership> list = db.ClubMemberships.Where(c => c.UserId == userId).ToList();
            foreach (ClubMembership item in list)
            {
                clubs.Add(item.ClubId);
            }

            ViewBag.MyClubs = clubs;

            var events = new List<Thing1.Models.Event>();
            var myEvents = new List<Thing1.Models.Event>();
            var myEventRSVPS = new List<Thing1.Models.EventsRSVP>(); 

            if (clubID != null)
            {
                ViewBag.currentClub = clubID;
                if (!String.IsNullOrEmpty(searchString))
                {
                    events = db.Events.Where(e => e.EndsAt > DateTime.Now).Where(e => e.Clubs.Any(c => c.Id == clubID)).Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.TargetAudience.Contains(searchString)).ToList();
                    myEventRSVPS = db.EventsRSVPs.Include(e => e.Event).Where(r => r.AspNetUser.Id == userId).Where(r => r.Status == "going" || r.Status == "interested").Where(r => r.Event.Clubs.Any(c => c.Id == clubID)).Where(r => r.Event.EndsAt > DateTime.Now).Where(r => r.Event.Title.Contains(searchString) || r.Event.Description.Contains(searchString) || r.Event.TargetAudience.Contains(searchString)).ToList();
                    page = 1;
                }
                else
                {
                    events = db.Events.Where(e => e.EndsAt > DateTime.Now).Where(e => e.Clubs.Any(c => c.Id == clubID)).ToList();
                    myEventRSVPS = db.EventsRSVPs.Include(e => e.Event).Where(r => r.AspNetUser.Id == userId).Where(r => r.Status == "going" || r.Status == "interested").Where(r => r.Event.Clubs.Any(c => c.Id == clubID)).Where(r => r.Event.EndsAt > DateTime.Now).ToList();
                    searchString = currentFilter;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    events = db.Events.Where(e => e.EndsAt > DateTime.Now).Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.TargetAudience.Contains(searchString)).ToList();
                    myEventRSVPS = db.EventsRSVPs.Include(e => e.Event).Where(r => r.AspNetUser.Id == userId).Where(r => r.Status == "going" || r.Status == "interested").Where(r => r.Event.EndsAt > DateTime.Now).Where(r => r.Event.Title.Contains(searchString) || r.Event.Description.Contains(searchString) || r.Event.TargetAudience.Contains(searchString)).ToList();
                    page = 1;
                }
                else
                {
                    events = db.Events.Where(e => e.EndsAt > DateTime.Now).ToList();
                    myEventRSVPS = db.EventsRSVPs.Include(e => e.Event).Where(r => r.AspNetUser.Id == userId).Where(r => r.Status == "going" || r.Status == "interested").Where(r => r.Event.EndsAt > DateTime.Now).ToList();
                    searchString = currentFilter;
                }
            }
            ViewBag.CurrentFilter = searchString;
            foreach (var rsvp in myEventRSVPS)
            {
                myEvents.Add(rsvp.Event);
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var eventsData = new EventsViewModel();
            eventsData.events = events.OrderBy(s => s.StartsAt).ToPagedList(pageNumber, pageSize);
            eventsData.myEvents = myEvents.OrderBy(r => r.StartsAt).ToPagedList(pageNumber, pageSize);
            eventsData.clubs = db.Clubs.ToList();
            return View(eventsData);
        }

        public ActionResult Calendar()
        {
            return View(db.Clubs.ToList());
        }

        public JsonResult CalendarData()
        {
            var colorList = new List<string> {"#d00","#0d0","#00d","#cc0","#c0c","#0cc"};

            DateTime start = DateTime.Parse(this.Request.QueryString["start"]);
            DateTime end = DateTime.Parse(this.Request.QueryString["end"]);
            var dbEvents = db.Events.Where(e => start <= e.StartsAt && e.StartsAt <= end).ToList();
            var events = new List<object>();
            foreach (var e in dbEvents)
            {
                var @event = new
                {
                    title = e.Title,
                    start = e.StartsAt,
                    end = e.EndsAt,
                    url = Url.Action("Details", "Events", new { id = e.Id }),
                    clubIds = e.Clubs.Select( c => c.Id).ToList(),
                    color = colorList[e.Club.Id%colorList.Count]
                };
                events.Add(@event);
            }
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        // GET: Events/DisplayClubEvents
        public ActionResult DisplayClubEvents(int clubId)
        {
            var clubToView = db.Clubs.Find(clubId);
            var clubEvents = new List<Thing1.Models.Event>();
            clubEvents = clubToView.Events.Where(e => e.EndsAt > DateTime.Now).ToList();
            return View(clubEvents);
        }
        // GET: Events/Details/5
        public ActionResult Details(int? id, int? clubId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            /*  ViewBag.ClubId = clubId;

              Club club = db.Clubs.Find(clubId);
              ViewBag.ClubName = club.name;
              ViewBag.ClubNickName = club.nickname;
              */

            //This code is to help determine if user can RSVP to event
            //Passes whether or not user is a club member to Details view (if True, then they can RSVP)
            ClubMembership membership = new ClubMembership();
            var userid = User.Identity.GetUserId();
            membership = db.ClubMemberships.Where(c => c.UserId == userid).Where(c => c.ClubId == @event.PrimaryClubID).FirstOrDefault();
            if (membership == null)
            {
                ViewBag.isClubMember = false;
            }
            else
            {
                ViewBag.isClubMember = true;
            }

            //This code is to help determine if user can perform officer actions (Show RSVPs, Edit, and Delete for event)
            //Passes whether or not user is a club officer to Details view (if True, then they can perform officer functions...Show RSVP, Edit/Delete Event)
            //Currently just directly uses CanEditClubData to determine if officer.  Might want to call CanEditAndCreateEvents() instead?  But need to sort out int? vs int
            if (membership != null && membership.CanEditClubData)
            {
                ViewBag.isClubOfficer = true;
            }
            else
            {
                ViewBag.isClubOfficer = false;
            }

            return View(@event);
        }
        // GET: Events/Create
        public ActionResult Create(int clubID)
        {
            if (CanCreateAndEditEvents(clubID))
            {
                PopulateSponsoringClubs(clubID);
                ViewBag.PrimaryClubID = clubID;
                return View();
            }
            else
            {
                return RedirectToAction("AccessDenied", "Home", null);
            }
        }
        //
        private void PopulateSponsoringClubs(int clubID)
        {
            var allClubs = db.Clubs;
            var viewModel = new List<SponsoringClubData>();
            foreach (var club in allClubs)
            {
                viewModel.Add(new SponsoringClubData
                {
                    ClubID = club.Id,
                    Name = club.nickname,
                    Sponsoring = club.Id == clubID
                });
            }
            ViewBag.Clubs = viewModel;
        }
        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Title,StartsAt,EndsAt,Id,Location,Description,TargetAudience,IsPublic,Food,Contact,Price")] Event @event)
        public ActionResult Create([Bind(Include = "Title, Location, Description, TargetAudience, IsPublic, Food, Contact, Price")] Event @event, string primaryClubID, string[] sponsoringClubs, string startDate, string startTime, string endDate, string endTime)
        {
            int pclub = int.Parse(primaryClubID);
            if (CanCreateAndEditEvents(pclub))
            {
                @event.Clubs = new List<Thing1.Models.Club>();
                var primaryClub = db.Clubs.Find(pclub);
                @event.Clubs.Add(primaryClub);
                @event.Club = primaryClub;
                if (sponsoringClubs != null)
                {
                    foreach (string clubID in sponsoringClubs)
                    {
                        var clubToAdd = db.Clubs.Find(int.Parse(clubID));
                        @event.Clubs.Add(clubToAdd);
                    }
                }
                DateTime sDate = Convert.ToDateTime(startDate);
                TimeSpan sTime = TimeSpan.Parse(startTime);
                DateTime start = sDate + sTime;
                DateTime startsAt = Convert.ToDateTime(start);
                DateTime eDate = Convert.ToDateTime(endDate);
                TimeSpan eTime = TimeSpan.Parse(endTime);
                DateTime end = eDate + eTime;
                DateTime endsAt = Convert.ToDateTime(end);
                @event.StartsAt = startsAt;
                @event.EndsAt = endsAt;
                if (ModelState.IsValid)
                {
                    db.Events.Add(@event);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View();
        }
        private bool CanCreateAndEditEvents(int clubID)
        {
            ClubMembership membership = new ClubMembership();
            var userid = User.Identity.GetUserId();
            membership = db.ClubMemberships.Where(c => c.UserId == userid).Where(c => c.ClubId == clubID).FirstOrDefault();
            if (membership == null)
                return false;
            if (membership.CanEditClubData) return true;
            else return false;
        }
        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            if (CanCreateAndEditEvents(@event.Club.Id))
                return View(@event);
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Title,Location,Id,Description,TargetAudience,IsPublic,Food,Contact,Price")] Event @event, string startDate, string startTime, string endDate, string endTime, int clubId)
        {
            if (CanCreateAndEditEvents(clubId))
            {
            //@event.Clubs = new List<Thing1.Models.Club>();
            //@event.Clubs.Add(db.Clubs.Find(pclub));
            //if (sponsoringClubs != null)
            //{
            //    foreach (string clubID in sponsoringClubs)
            //    {
            //        var clubToAdd = db.Clubs.Find(int.Parse(clubID));
            //        @event.Clubs.Add(clubToAdd);
            //    }
            //}
            DateTime sDate = Convert.ToDateTime(startDate);
            TimeSpan sTime = TimeSpan.Parse(startTime);
            DateTime start = sDate + sTime;
            DateTime startsAt = Convert.ToDateTime(start);
            DateTime eDate = Convert.ToDateTime(endDate);
            TimeSpan eTime = TimeSpan.Parse(endTime);
            DateTime end = eDate + eTime;
            DateTime endsAt = Convert.ToDateTime(end);
            @event.StartsAt = startsAt;
            @event.EndsAt = endsAt;
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(@event);
        }
        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }
        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Include(i => i.Clubs).Where(i => i.Id == id).Single();
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}

