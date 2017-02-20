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

        private user_managementEntities db = new user_managementEntities();

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


        // GET: ClubMemberships/SelectMembershipOptions/5
        [Authorize]
        public ActionResult SelectMembershipOptions(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(clubId);
            if (club == null)
            {
                return HttpNotFound();
            }

            ////////////////////////////////////////////////////////////////////
            // Sign-up availability Check Logic SHOULD COME HERE
            // 1. Get current memberships Per each type of membership that users already have 
            // 2. Only show up memberships that have longer period than the current one
            ////////////////////////////////////////////////////////////////////

            // OR!!!!!!!!!!!!
            // On the POST method
            // You can just post check if the user can actually sing-up for the membership he or she just selected! --> MUCH EASIER!

            /*
            // 1. Get current memberships per each type that users already have 
            List<KeyValuePair<TypesOfMembershipOption, ClubMembership>> curMembershipsPerType = new List<KeyValuePair<TypesOfMembershipOption, ClubMembership>>();
            string userId = User.Identity.GetUserId();
            List<TypesOfMembershipOption> typesOfMemberships = db.TypesOfMembershipOptions.ToList();
            foreach (var type in typesOfMemberships)
            {
                List<ClubMembership> membershipPerType = db.ClubMemberships.Where(c => c.UserId == userId && c.ClubId == clubId && c.TermDate > DateTime.Now && c.MembershipOption.TypeId == type.Id).ToList();
                if (membershipPerType.Count >= 2)
                {
                    // No way
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }
                else if (membershipPerType.Count == 1)
                {
                    curMembershipsPerType.Add(new KeyValuePair<TypesOfMembershipOption, ClubMembership>(type, membershipPerType.First()));
                }
                else
                {
                    // Okay, the user does not have any memberbership for this type
                }
            }

            // 2. Only show up memberships that have longer period than the current one
            */


            ////////////////////////////////////////////////////////////////////

            ViewBag.ClubId = club.Id;
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;
            ViewBag.MembershipOptions = club.MembershipOptions.ToList();

            string userId = User.Identity.GetUserId();

            ViewBag.ClubMemberships = db.ClubMemberships.Where(c => c.UserId == userId && c.ClubId == clubId).ToList();

            //Serialize
            Session["Club Object"] = club;
            
            return View();
        }

        // POST: ClubMemberships/SelectMembershipOptions
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectMembershipOptions([Bind(Include = "Id,UserId,ClubId,MembershipOptionId,RoleId,TermDate,JoinDate,Description,HasAccessToFinance,CanEditClubData,Violation")] ClubMembership clubMembership)
        {
            if (ModelState.IsValid)
            {

                // duplicate check
                List<ClubMembership> list = db.ClubMemberships.Where(c => c.UserId == clubMembership.UserId && c.ClubId == clubMembership.ClubId).ToList();

                if(list.Count > 0)
                {
                    foreach(ClubMembership item in list)
                    {
                        if(item.MembershipOptionId == clubMembership.MembershipOptionId)
                        {
                            // duplicate membership exist
                            return RedirectToAction("DuplicateMembership", new {
                                duplicateClubId = clubMembership.ClubId,
                                duplicateMembershipOptionId = clubMembership.MembershipOptionId,
                            });
                        }

                    }
                }


                // proceed with new membership
                MembershipOption membershipOption = db.MembershipOptions.Where(n=> n.Id == clubMembership.MembershipOptionId).ToList().First();

                if (membershipOption == null)
                {
                    return HttpNotFound();
                }

                clubMembership.JoinDate = DateTime.Now;
                //clubMembership.TermDate = clubMembership.JoinDate.AddYears(membershipOption.Duration);
                clubMembership.TermDate = membershipOption.ExpDate;
                clubMembership.Description = membershipOption.Description;

                // Serialize
                Session["ClubMembership Object"] = clubMembership;
                Session["MembershipOption Object"] = membershipOption;

                return RedirectToAction("MembershipOptionConfirmation");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", clubMembership.UserId);
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            return View(clubMembership);
        }

        public ActionResult DuplicateMembership(int? duplicateClubId, int? duplicateMembershipOptionId)
        {

            
            string userId = User.Identity.GetUserId();

            MembershipOption membershipOption = db.MembershipOptions.Where(n => n.Id == duplicateMembershipOptionId).ToList().First();
            ClubMembership clubMembership = db.ClubMemberships.Where(n => n.ClubId == duplicateClubId && n.UserId == userId && n.MembershipOptionId == duplicateMembershipOptionId).ToList().First();
            Club club = db.Clubs.Find(duplicateClubId);

            ViewBag.DuplicateClubId = duplicateClubId;
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;
            ViewBag.Duration = membershipOption.Duration;
            ViewBag.JoinDate = clubMembership.JoinDate.Day + "/" + clubMembership.JoinDate.Month + "/" + clubMembership.JoinDate.Year;
            ViewBag.TermDate = clubMembership.TermDate.Day + "/" + clubMembership.TermDate.Month + "/" + clubMembership.TermDate.Year;
            ViewBag.Description = membershipOption.Description;

            return View();
        }

        // GET: ClubMemberships/MembershipOptionConfirmation/5
        [Authorize]
        public ActionResult MembershipOptionConfirmation()
        {
            //DeSerialize
            Club club = (Club)Session["Club Object"];
            ClubMembership clubMembership = (ClubMembership)Session["ClubMembership Object"];
            MembershipOption membershipOption = (MembershipOption)Session["MembershipOption Object"];
        
            if (club == null || clubMembership == null || membershipOption == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClubId = club.Id;
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;
            ViewBag.Price = membershipOption.Price;
            ViewBag.Duration = membershipOption.Duration;
            ViewBag.MembershipOptionId = membershipOption.Id;
            ViewBag.JoinDate = clubMembership.JoinDate.ToString("G");
            ViewBag.TermDate = clubMembership.TermDate.ToString("G");
            ViewBag.Description = membershipOption.Description;

            //Store description: membershipOption.Description for membershipOption == clubMembership.MembershipOption and ClubID == club.ClubID
            //ViewBag.Description = db.MembershipOptions.Where(n => n.Id == clubMembership.MembershipOptionId).ToList().First().Description;

            //below is a test to see what gets stored
            //string tempMessage = club.name + " " + membershipOption.Duration + " (" + clubMembership.JoinDate + "~" + clubMembership.TermDate + ")" + " USD: " + membershipOption.Price;
            //return Content(tempMessage);

            return View(clubMembership);
        }


        // GET: ClubMemberships/Payment
        [Authorize]
        public ActionResult Payment()
        {
            // ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            return View();
        }

        // GET: ClubMemberships/Create
        public ActionResult Create(int? clubId)
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name");
            ViewBag.MembershipOptionId = new SelectList(db.MembershipOptions, "Id", "Description", clubId);
            return View();
        }

        // POST: ClubMemberships/Create
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
                return RedirectToAction("Success", "PayPal", new { clubId = clubMembership.ClubId, clubMembershipId = clubMembership.Id });
            }
            else
            {
            // Need to repopulate the SelectList (for free payment on MembershipOptionConfirmation.cshtml file)
            // http://stackoverflow.com/questions/3393521/the-viewdata-item-that-has-the-key-my-key-is-of-type-system-string-but-must
            
            
            // For UserIds
            IEnumerable<SelectListItem> userIds = db.ClubMemberships.Select(
                x => new SelectListItem { Value = x.UserId, Text = x.UserId });
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            ViewData["UserId"] = userIds;

            // For MembershipOptionIds
            IEnumerable<SelectListItem> membershipOptionIds = db.ClubMemberships.Select(
                x => new SelectListItem { Value = x.MembershipOptionId.ToString(), Text = x.MembershipOptionId.ToString() });
            ViewData["MembershipOptionId"] = membershipOptionIds;

            // For ClubId
            IEnumerable<SelectListItem> clubIds = db.ClubMemberships.Select(
                x => new SelectListItem { Value = x.ClubId.ToString(), Text = x.ClubId.ToString() });
            ViewData["ClubId"] = clubIds;
             
            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);

            return RedirectToAction("Success", "PayPal", new { clubId = clubMembership.ClubId, clubMembershipId = clubMembership.Id });

            //return View(clubMembership);
            }
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
