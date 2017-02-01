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

        // GET: MembershipOption
        public ActionResult Index()
        {
            var membershipOptions = db.MembershipOptions.Include(m => m.Club).Include(m => m.TypesOfMembershipOption);
            return View(membershipOptions.ToList());
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

            ViewBag.ClubId = clubId;

            Club club = db.Clubs.Find(clubId);
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;

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

        // GET: MembershipOption/Details/5
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

        // GET: MembershipOption/Create
        public ActionResult CreateMembershipOption(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClubId = clubId;
            ViewBag.TypeId = new SelectList(db.TypesOfMembershipOptions, "Id", "Description");
            ViewBag.Duration = this.GetDurationSelectList();
        
            return View();
        }

        // POST: MembershipOption/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMembershipOption([Bind(Include = "Id,ClubId,TypeId,Duration,ExpDate,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (ModelState.IsValid)
            {
                db.MembershipOptions.Add(membershipOption);
                db.SaveChanges();
                return RedirectToAction("ViewCurrentMembershipOptions", new { clubId = membershipOption.ClubId });
            }
      
            ViewBag.ClubId = membershipOption.ClubId;
            ViewBag.TypeId = new SelectList(db.TypesOfMembershipOptions, "Id", "Description");
            ViewBag.Duration = this.GetDurationSelectList();

            return View(membershipOption);
        }

        // GET: MembershipOption/Edit/5
        public ActionResult EditMembershipOption(int? Id, int? clubId)
        {
            if (Id == null || clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipOption membershipOption = db.MembershipOptions.Find(Id);
            if (membershipOption == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = Id;
            ViewBag.ClubId = clubId;
            ViewBag.TypeId = new SelectList(db.TypesOfMembershipOptions, "Id", "Description", membershipOption.TypeId);
            ViewBag.Duration = this.GetDurationSelectList(membershipOption.Duration);

            return View(membershipOption);
       }

        // POST: MembershipOption/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembershipOption([Bind(Include = "Id,ClubId,TypeId,Duration,ExpDate,Price,Description,IsActive")] MembershipOption membershipOption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(membershipOption).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewCurrentMembershipOptions", new { clubId = membershipOption.ClubId });
            }

            ViewBag.Id = membershipOption.Id;
            ViewBag.ClubId = membershipOption.ClubId;
            ViewBag.TypeId = new SelectList(db.TypesOfMembershipOptions, "Id", "Description", membershipOption.TypeId);
            ViewBag.Duration = this.GetDurationSelectList(membershipOption.Duration);

            return View(membershipOption);
        }

        // GET: MembershipOption/Delete/5
        public ActionResult DeleteMembershipOption(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MembershipOption membershipOption = db.MembershipOptions.Find(Id);
            if (membershipOption == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = Id;
            ViewBag.ClubId = membershipOption.ClubId;

            return View(membershipOption);      

        }

        // POST: MembershipOption/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMembershipOption(int Id)
        {
            MembershipOption membershipOption = db.MembershipOptions.Find(Id);

            if (membershipOption == null)
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
