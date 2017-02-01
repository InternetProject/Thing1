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
    public class ClubMembersListController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: ClubMembersList
        public ActionResult Index(int? id)
        {
            var clubMemberships = db.ClubMemberships.Include(c => c.AspNetUser).Include(c => c.Club).Include(c => c.MembershipOption).Include(c => c.TypesOfRole);
            if (id == null)
            {
                return View(clubMemberships.ToList());
            }
            else
            {
                var clubid = id;
                return View(clubMemberships.Where(c => c.ClubId == clubid).ToList());

            }
        }

        // GET: ClubMembersList/Details/5
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

        // GET: ClubMembersList/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description");
            ViewBag.RoleId = new SelectList(db.TypesOfRoles, "Id", "Description");
            return View();
        }

        // POST: ClubMembersList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ClubId,MembershipOptionId,RoleId,TermDate,JoinDate,Description,HasAccessToFinance,CanEditClubData,Violation")] ClubMembership clubMembership)
        {
            if (ModelState.IsValid)
            {
                db.ClubMemberships.Add(clubMembership);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", clubMembership.UserId);
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description", clubMembership.MembershipOptionId);
            ViewBag.RoleId = new SelectList(db.TypesOfRoles, "Id", "Description", clubMembership.RoleId);
            return View(clubMembership);
        }

        // GET: ClubMembersList/Edit/5
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
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", clubMembership.UserId);
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description", clubMembership.MembershipOptionId);
            ViewBag.RoleId = new SelectList(db.TypesOfRoles, "Id", "Description", clubMembership.RoleId);
            return View(clubMembership);
        }

        // POST: ClubMembersList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ClubId,MembershipOptionId,RoleId,TermDate,JoinDate,Description,HasAccessToFinance,CanEditClubData,Violation")] ClubMembership clubMembership)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clubMembership).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", clubMembership.UserId);
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description", clubMembership.MembershipOptionId);
            ViewBag.RoleId = new SelectList(db.TypesOfRoles, "Id", "Description", clubMembership.RoleId);
            return View(clubMembership);
        }

        // GET: ClubMembersList/Delete/5
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

        // POST: ClubMembersList/Delete/5
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
