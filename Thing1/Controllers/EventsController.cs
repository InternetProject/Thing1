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
        public ActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            var upcomingEvents = db.Events.Where(e => e.EndsAt > DateTime.Now);
            return View(upcomingEvents.OrderBy(e => e.StartsAt).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Calendar()
        {
            return View();
        }

            
        // GET: Events/DisplayClubEvents
        public ActionResult DisplayClubEvents(int clubId)
        {
            var clubToView = db.Clubs.Find(clubId);
            var clubEvents = new List<Thing1.Models.Event>();
            clubEvents = clubToView.Events.ToList();
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
            ClubMembership membership = new ClubMembership();
            var userid = User.Identity.GetUserId();
            membership = db.ClubMemberships.Where(c => c.UserId == userid).Where(c => c.ClubId == pclub).Single();

            if (membership.CanEditClubData)
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
        public ActionResult Edit([Bind(Include = "Title,StartsAt,EndsAt,Location,Id,Description,TargetAudience,IsPublic,Food,Contact,Price")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
