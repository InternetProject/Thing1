using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;

namespace Thing1.Controllers
{
    public class MembershipOptionsController : Controller
    {
        private user_managementEntities1 db = new user_managementEntities1();

        // GET: MembershipOptions
        public ActionResult Index()
        {
            var membershipOptions = db.MembershipOptions.Include(m => m.Club);
            return View(membershipOptions.ToList());
        }

        // GET: MembershipOptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipOption membershipOption = db.MembershipOptions.Find(id);
            if (membershipOption == null)
            {
                return HttpNotFound();
            }
            return View(membershipOption);
        }

        // GET: MembershipOptions/Create
        public ActionResult Create()
        {
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            return View();
        }

        // POST: MembershipOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ClubId,Option,Expiration,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (ModelState.IsValid)
            {
                db.MembershipOptions.Add(membershipOption);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);
            return View(membershipOption);
        }

        // GET: MembershipOptions/Edit/5
        public ActionResult EditMembershipOptions(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipOption membershipOption = db.MembershipOptions.Find(clubId);
            if (membershipOption == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);
            return View(membershipOption);
        }

        // POST: MembershipOptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembershipOptions([Bind(Include = "Id,ClubId,Option,Expiration,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membershipOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);
            return View(membershipOption);
        }

        // GET: MembershipOptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipOption membershipOption = db.MembershipOptions.Find(id);
            if (membershipOption == null)
            {
                return HttpNotFound();
            }
            return View(membershipOption);
        }

        // POST: MembershipOptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MembershipOption membershipOption = db.MembershipOptions.Find(id);
            db.MembershipOptions.Remove(membershipOption);
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
