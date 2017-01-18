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
        private user_managementEntities db = new user_managementEntities();

        private SelectList GetDurationSelectList(int currentDuration = 1)
        {
          return new SelectList(new List<SelectListItem>
          {
                new SelectListItem { Selected = true, Text = "1 year", Value = "1" },
                new SelectListItem { Selected = false, Text = "2 years", Value = "2" },
                new SelectListItem { Selected = false, Text = "3 years", Value = "3" },
            }, "Value", "Text", currentDuration);
        }

         // GET: MembershipOptions/Create
        public ActionResult CreateMembershipOptions(int? clubId)
        {
            ViewBag.CurrentClubId = clubId;
            ViewBag.Club = new SelectList(db.Clubs, "Id", "name");
           ViewBag.Type = new SelectList(db.TypesOfMembershipOptions, "Id", "Description");
            ViewBag.Duration = this.GetDurationSelectList();

            return View();
                       
        }

        // POST: MembershipOptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMembershipOptions([Bind(Include = "Id,ClubId,TypeId,Duration,Price,Description,Is_Active")] MembershipOption membershipOption)
        {   
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.MembershipOptions.Add(membershipOption);
            db.SaveChanges();
            return RedirectToAction("ViewCurrentMembershipOptions", new { clubId = membershipOption.ClubId });

        }

        // GET: MembershipOptions/Edit/5
        public ActionResult EditMembershipOptions(int? id)
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
            //ViewBag.Club = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);

            ViewBag.Club = db.Clubs.Find(membershipOption.ClubId);
            ViewBag.Type = new SelectList(db.TypesOfMembershipOptions, "Id", "Description", membershipOption.TypeId);
            ViewBag.Duration = this.GetDurationSelectList(membershipOption.Duration);

            return View(membershipOption);
        }
        // POST: MembershipOptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembershipOptions([Bind(Include = "Id,ClubId,TypeId,Duration,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Entry(membershipOption).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewCurrentMembershipOptions", new { clubId = membershipOption.ClubId });

        }
        // GET: MembershipOptions/View/5
        public ActionResult ViewCurrentMembershipOptions(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //1. retrieve club name and nickname
            //2 set these values to ViewBag

            return View(db.MembershipOptions.Where(c => c.ClubId == clubId).ToList());
            /*
                MembershipOption membershipOption = db.MembershipOptions.Find(clubId);
                if (membershipOption == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);
                return View(membershipOption);
                */
        }

        // POST: MembershipOptions/View/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCurrentMembershipOptions([Bind(Include = "Id,ClubId,TypeId,Duration,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membershipOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Club = new SelectList(db.Clubs, "Id", "name", membershipOption.ClubId);
            ViewBag.Type = new SelectList(db.TypesOfMembershipOptions, "Id", "Description", membershipOption.TypeId);
            ViewBag.Duration = this.GetDurationSelectList(membershipOption.Duration);

            return View(membershipOption);
        }

        // GET: MembershipOptions/Delete/5
        public ActionResult DeleteMembershipOption(int? id)
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
        [HttpPost, ActionName("DeleteMembershipOption")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMembershipOption(int id)
        {
            MembershipOption membershipOption = db.MembershipOptions.Find(id);

            if(membershipOption == null)
            {
                return HttpNotFound();
            }

            int AffectedClubId = membershipOption.ClubId;

            db.MembershipOptions.Remove(membershipOption);
            db.SaveChanges();
            return RedirectToAction("ViewCurrentMembershipOptions", new { clubId = AffectedClubId });
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