using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Thing1.Models;
using Thing1.Models.ViewModels;


namespace Thing1.Controllers
{
    public class EventsRSVPsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: EventsRSVPs
        public ActionResult Index()
        {
            var eventsRSVPs = db.EventsRSVPs.Include(e => e.AspNetUser).Include(e => e.Event);
            return View(eventsRSVPs.ToList());
        }

        // GET: EventsRSVPs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventsRSVP eventsRSVP = db.EventsRSVPs.Find(id);
            if (eventsRSVP == null)
            {
                return HttpNotFound();
            }
            return View(eventsRSVP);
        }


        // GET: EventsRSVPs/Create
        public ActionResult Create(int? eventId)
        {
            //Use the eventId passed into the Create GET to find the event from the database//////
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event thisEvent = db.Events.Find(eventId);
            if (thisEvent == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventFull = false;
            if (thisEvent.Capacity != null)
            {
                int rsvpGoing = thisEvent.EventsRSVPs.Where(r => r.Status == "going").ToList().Count();
                if (thisEvent.Capacity.Value - rsvpGoing <= 0)
                {
                    ViewBag.EventFull = true;
                }
            }

            //Get the user's id
            var userid = User.Identity.GetUserId();

            // ADD LOGIC HERE FOR CHECKING IF RSVP ALREADY EXISTS
            foreach(var checkRSVP in db.EventsRSVPs)                                      // Iterate through every RSVP in the database
            {
                if (checkRSVP.UserId == userid && checkRSVP.EventId == thisEvent.Id)      // If UserId and EventId match....
                {

                     return RedirectToAction("Edit", new { id = checkRSVP.Id });                                    // ....redirect to the RSVP Edit page
                }
            }

            //Setup ViewBag to send user and event info to the Create View
            ViewBag.thisEventTitle = thisEvent.Title;
            ViewBag.thisEventId = eventId;
            //Pass the event to the Create View
            return View();
        }

        // POST: EventsRSVPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventsRSVP eventsRSVP)
        {
            //Get and set the user's id
            eventsRSVP.UserId = User.Identity.GetUserId();

            //Set HasPaid to False (for now, must implement this logic later)
            eventsRSVP.HasPaid = false;

            if (ModelState.IsValid)
            {
                db.EventsRSVPs.Add(eventsRSVP);
                db.SaveChanges();
                return RedirectToAction("Index", "Events");
            }

            return RedirectToAction("Index", "Home");
        }

        //This method will display list of RSVPs for club officers
        public ActionResult DisplayRSVPs(int? eventId)
        {
            //Use the eventId passed into the DisplayRSVPs to find the event from the database
            if (eventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event thisEvent = db.Events.Find(eventId);
            if (thisEvent == null)
            {
                return HttpNotFound();
            }

            //Use ViewBags to help setup view title and event info (passedEventId is used in action link to return user back to even details page)
            ViewBag.passedEventTitle = thisEvent.Title;
            ViewBag.passedEventId = thisEvent.Id;

            //pull RSVPs of each type (going, interested, not going) and put into temp var's
            var thisEventGoingRSVPs = db.EventsRSVPs.Include(e => e.AspNetUser).Where(e => e.EventId == eventId).Where(e => e.Status == "going");
            var thisEventInterestedRSVPs = db.EventsRSVPs.Include(e => e.AspNetUser).Where(e => e.EventId == eventId).Where(e => e.Status == "interested");
            var thisEventNotGoingRSVPs = db.EventsRSVPs.Include(e => e.AspNetUser).Where(e => e.EventId == eventId).Where(e => e.Status == "not going");

            //initialize RSVP view model
            var rsvpPageData = new RSVPsViewModel();

            //populate each list in the RSVP view model with what was pulled into var's above
            foreach (var a in thisEventGoingRSVPs)
            {
                rsvpPageData.membersGoing.Add(a.AspNetUser);
            }
            foreach (var a in thisEventInterestedRSVPs)
            {
                rsvpPageData.membersInterested.Add(a.AspNetUser);
            }
            foreach (var a in thisEventNotGoingRSVPs)
            {
                rsvpPageData.membersNotGoing.Add(a.AspNetUser);
            }

            return View(rsvpPageData);
        }


        // GET: EventsRSVPs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventsRSVP eventsRSVP = db.EventsRSVPs.Find(id);
            if (eventsRSVP == null)
            {
                 return HttpNotFound();
            }

            //Setup ViewBag to send user and event info to the Edit View
            Event thisEvent = db.Events.Find(eventsRSVP.EventId);
            ViewBag.thisEventTitle = thisEvent.Title;
            ViewBag.thisEventId = thisEvent.Id;

            return View(eventsRSVP);
        }

        // POST: EventsRSVPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventId,UserId,HasPaid,Status")] EventsRSVP eventsRSVP)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventsRSVP).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Events");
            }

            //ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", eventsRSVP.UserId);
           // ViewBag.EventId = new SelectList(db.Events, "Id", "Location", eventsRSVP.EventId);
            return View(eventsRSVP);
        }

        // GET: EventsRSVPs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventsRSVP eventsRSVP = db.EventsRSVPs.Find(id);
            if (eventsRSVP == null)
            {
                return HttpNotFound();
            }
            return View(eventsRSVP);
        }

        // POST: EventsRSVPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventsRSVP eventsRSVP = db.EventsRSVPs.Find(id);
            db.EventsRSVPs.Remove(eventsRSVP);
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
