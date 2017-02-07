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

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            user_managementEntities dbContext = new user_managementEntities();
                IEnumerable<SelectListItem> items = dbContext.Clubs.Select(c => new SelectListItem
                {
                Value = c.nickname,
                Text = c.nickname
                    });
            ViewBag.ClubList = items;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
         
            var events = from s in db.Events
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString) || s.TargetAudience.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    events = events.OrderBy(s => s.StartsAt);
                    break;
                case "date_desc":
                    events = events.OrderByDescending(s => s.StartsAt);
                    break;
                default:
                    events = events.OrderBy(s => s.StartsAt);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(events.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Calendar()
        {
            return View();
        }

        public JsonResult CalendarData()
        {
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
        public ActionResult Details(int? id)
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
        // GET: Events/Create
        public ActionResult Create(int clubID)
        {
            PopulateSponsoringClubs(clubID);
            ViewBag.PrimaryClubID = clubID;
            return View();
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
        public ActionResult Create([Bind(Include = "Title, Location, Description, TargetAudience, IsPublic, Food, Contact, Price")] Event @event, string primaryClub, string[] sponsoringClubs, string startDate, string startTime, string endDate, string endTime)
        {
            int pclub = int.Parse(primaryClub);
            if (CanCreateAndEditEvents(pclub))
            {
                @event.Clubs = new List<Thing1.Models.Club>();
                @event.Clubs.Add(db.Clubs.Find(pclub));
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
            membership = db.ClubMemberships.Where(c => c.UserId == userid).Where(c => c.ClubId == clubID).Single();
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
            return View(@event);
        }
        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Title,Location,Id,Description,TargetAudience,IsPublic,Food,Contact,Price")] Event @event, string startDate, string startTime, string endDate, string endTime)
        {
            //if (CanCreateAndEditEvents(clubID))
            //{
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
            //}
            //else
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            //}
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

