using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;
using Microsoft.AspNet.Identity;

namespace Thing1.Controllers
{
    public class ClubMembershipsController : Controller
    {

        private user_managementEntities1 db = new user_managementEntities1();

        // GET: ClubMemberships
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized); // should change this later.
            }

            var userid = User.Identity.GetUserId();
            var clubMemberships = db.ClubMemberships.Include(c => c.Club);
            return View(clubMemberships.Where(c => c.UserId == userid).ToList());
        }

        // GET: ClubMemberships/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubMembership clubMembership = db.ClubMemberships.Find(id);
            if (clubMembership == null)
            {
                return HttpNotFound();
            }
            return View(clubMembership);
        }

        // GET: ClubMemberships/Create
        public ActionResult Create()
        {
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            return View();
        }

        // POST: ClubMemberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ClubId,TermDate,JoinDate,Description,HasAccessToFinance")] ClubMembership clubMembership)
        {
            if (ModelState.IsValid)
            {
                db.ClubMemberships.Add(clubMembership);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            return View(clubMembership);
        }

        // GET: ClubMemberships/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubMembership clubMembership = db.ClubMemberships.Find(id);
            if (clubMembership == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            return View(clubMembership);
        }

        // POST: ClubMemberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ClubId,TermDate,JoinDate,Description,HasAccessToFinance")] ClubMembership clubMembership)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clubMembership).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            return View(clubMembership);
        }

        // GET: ClubMemberships/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClubMembership clubMembership = db.ClubMemberships.Find(id);
            if (clubMembership == null)
            {
                return HttpNotFound();
            }
            return View(clubMembership);
        }

        // POST: ClubMemberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClubMembership clubMembership = db.ClubMemberships.Find(id);
            db.ClubMemberships.Remove(clubMembership);
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
