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

        //public ActionResult ClubEvents(int clubId)
        //{
        //    var clubEvents = db.ClubEvents.Where(c => c.ClubId == clubId);
        //    var upcomingEvents = clubEvents.Where(c => c.Event.StartsAt > DateTime.Now).Include(c => c.Event);
        //    return View(upcomingEvents.ToList());
        //}

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
            ViewBag.PrimaryClubName = db.Clubs.Find(clubID).name;
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
        public ActionResult Create([Bind(Include = "Title, StartsAt, EndsAt, Location, Description, TargetAudience, IsPublic, Food, Contact, Price")] Event @event, string[] sponsoringClubs)
        {
            System.Diagnostics.Debug.WriteLine("Clubs: " + sponsoringClubs);
            if (sponsoringClubs != null)
            {
                @event.Clubs = new List<Thing1.Models.Club>();
                foreach (string clubID in sponsoringClubs)
                {
                    var clubToAdd = db.Clubs.Find(int.Parse(clubID));
                    @event.Clubs.Add(clubToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
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
        public ActionResult Edit([Bind(Include = "Title,Date,Time,Location,Id,Description,TargetAudience,IsPublic,Food,Contact,Price")] Event @event)
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
