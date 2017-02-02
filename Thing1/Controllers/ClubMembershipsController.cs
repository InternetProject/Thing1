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

                //db.ClubMemberships.Add(clubMembership);
                //db.SaveChanges();


                MembershipOption membershipOption = db.MembershipOptions.Where(n=> n.Id == clubMembership.MembershipOptionId).ToList().First();

                if (membershipOption == null)
                {
                    return HttpNotFound();
                }

                clubMembership.JoinDate = DateTime.Now;
                clubMembership.TermDate = clubMembership.JoinDate.AddYears(membershipOption.Duration);
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
            ViewBag.JoinDate = clubMembership.JoinDate.ToString("d");
            ViewBag.TermDate = clubMembership.TermDate.ToString("d");
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
                return RedirectToAction("Index");
            }

            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", clubMembership.ClubId);
            return View(clubMembership);
        }

        //public ActionResult Test(TestModel testData)
        //{
        //    /*
        //     * string tempMessage = testData.UserId + " " +
        //                        testData.ClubId + " " +
        //                        testData.MembershipOptionId + " " +
        //                        testData.RoleId + " " +
        //                        testData.TermDate + " " +
        //                        testData.JoinDate + " " +
        //                        testData.Description + " " +
        //                        testData.HasAccessToFinance + " " +
        //                        testData.CanEditClubData + " " +
        //                        testData.Violation;
        //    */
        //    //if (ModelState.IsValid)
        //    //{
        //    //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    //}


        //     ClubMembership clubMembership = new ClubMembership();
        //     clubMembership.UserId = testData.UserId;
        //     clubMembership.ClubId = testData.ClubId;
        //     clubMembership.MembershipOptionId = testData.MembershipOptionId;
        //     clubMembership.RoleId = testData.RoleId;
        //     clubMembership.TermDate = testData.TermDate;
        //     clubMembership.JoinDate = testData.JoinDate;
        //     clubMembership.Description = testData.Description;
        //     clubMembership.HasAccessToFinance = testData.HasAccessToFinance;
        //     clubMembership.CanEditClubData = testData.CanEditClubData;
        //     clubMembership.Violation = testData.Violation;

        //     db.ClubMemberships.Add(clubMembership);
        //     db.SaveChanges();
        //     return RedirectToAction("Index");
        //}

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
