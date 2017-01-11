using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;

namespace Thing1.Controllers
{
    public class MemberManagement1Controller : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: MemberManagement1

        public ViewResult Index(string searchString)
        {
            int id = 2;  // id needs to be set by club selection
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            var memberList = from cm in db.ClubMemberships
                             where cm.ClubId == id
                             select cm;

 
            if (!String.IsNullOrEmpty(searchString))
            {
                memberList = memberList.Where(s => s.AspNetUser.LastName.Contains(searchString));
            }

            return View(memberList.ToList());
            
        }

        //public ActionResult Index()
        //{
        //    //var clubMemberships = db.ClubMemberships.Include(c => c.AspNetUser).Include(c => c.Club).Include(c => c.MembershipOption).Include(c => c.TypesOfRole);
        //    //return View(clubMemberships.ToList());


        //    int id = 2;  // id needs to be set by club selection
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    //ArrayList<ClubMembership> clubMembership = db.ClubMemberships.Find(id);

        //    List<ClubMembership> result = (from cm in db.ClubMemberships
        //                                   where cm.ClubId == id
        //                                   select cm).ToList();
        //    if (result == null)
        //    {
        //        result = new List<ClubMembership>();
        //    }
        //    return View(result);
        //}

        // GET: MemberManagement1/Details/5
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

        // GET: MemberManagement1/Details/5
        //Returns details of Club Membership by Club id
        public ActionResult ListByClub(int? id)
        {
            id = 2;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //ArrayList<ClubMembership> clubMembership = db.ClubMemberships.Find(id);
            List<ClubMembership> result = (from cm in db.ClubMemberships
                          where cm.ClubId == id
                          select cm).ToList();
            if (result == null)
            {
                result = new List<ClubMembership>();
            }
            return View(result);
        }

        // GET: MemberManagement1/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description");
            ViewBag.RoleId = new SelectList(db.TypesOfRoles, "Id", "Description");
            return View();
        }

        // POST: MemberManagement1/Create
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

        // GET: MemberManagement1/Edit/5
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

        // POST: MemberManagement1/Edit/5
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

        // GET: MemberManagement1/Delete/5
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

        // POST: MemberManagement1/Delete/5
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
