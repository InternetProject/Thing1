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
    public class PayPalAccountsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: PayPalAccounts
        public ActionResult Index()
        {
            var payPalAccounts = db.PayPalAccounts.Include(p => p.Club);
            return View(payPalAccounts.ToList());
        }

        public ActionResult ViewPayPalAccount(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // CanEditClubData check
            string userId = User.Identity.GetUserId();
            List<ClubMembership> list = db.ClubMemberships.Where(c => c.ClubId == clubId && c.UserId == userId).ToList();
            if (list.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool canEdit = false;
            foreach (ClubMembership item in list)
            {
                canEdit |= item.CanEditClubData;
            }

            if (canEdit == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //1. retrieve club name and nickname
            //2 set these values to ViewBag

            ViewBag.ClubId = clubId;

            Club club = db.Clubs.Find(clubId);
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;

            return View(db.PayPalAccounts.Where(c=>c.ClubId == clubId).ToList());            
        }

        // GET: PayPalAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PayPalAccount payPalAccount = db.PayPalAccounts.Find(id);
            if (payPalAccount == null)
            {
                return HttpNotFound();
            }
            return View(payPalAccount);
        }

        // GET: PayPalAccounts/Create
        public ActionResult CreatePayPalAccount(int? clubId)
        {
            if (clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // CanEditClubData check
            string userId = User.Identity.GetUserId();
            List<ClubMembership> list = db.ClubMemberships.Where(c => c.ClubId == clubId && c.UserId == userId).ToList();
            if (list.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool canEdit = false;
            foreach (ClubMembership item in list)
            {
                canEdit |= item.CanEditClubData;
            }

            if (canEdit == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClubId = clubId;
            Club club = db.Clubs.Find(clubId);
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;

            List<PayPalAccount> listP = db.PayPalAccounts.Where(c => c.ClubId == clubId).ToList();
            foreach(PayPalAccount item in listP)
            {
                return RedirectToAction("PayPalAccountAlreadyExist", new { ClubId = clubId, PayPalClientId = item.PayPalClientId, PayPalSecret = item.PayPalClientSecret });
            }

            return View();
        }

        // GET: PayPalAccounts/PayPalAccountAlreadyExist
        public ActionResult PayPalAccountAlreadyExist(int? ClubId, string PayPalClientId, string PayPalSecret)
        {
            if (ClubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ClubId = ClubId;
            Club club = db.Clubs.Find(ClubId);
            ViewBag.ClubName = club.name;
            ViewBag.ClubNickName = club.nickname;
            ViewBag.PayPalClientId = PayPalClientId;
            ViewBag.PayPalSecret = PayPalSecret;

            return View();
        }

        // POST: PayPalAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePayPalAccount([Bind(Include = "Id,ClubId,PayPalClientId,PayPalClientSecret")] PayPalAccount payPalAccount)
        {
            if (ModelState.IsValid)
            {
                db.PayPalAccounts.Add(payPalAccount);
                db.SaveChanges();
                return RedirectToAction("ViewPayPalAccount", new { clubId = payPalAccount.ClubId });
            }

            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", payPalAccount.ClubId);
            return View(payPalAccount);
        }


        // GET: PayPalAccounts/Edit/5
        public ActionResult EditPayPalAccount(int? Id, int? clubId)
        {
            if (Id == null || clubId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // CanEditClubData check
            string userId = User.Identity.GetUserId();
            List<ClubMembership> list = db.ClubMemberships.Where(c => c.ClubId == clubId && c.UserId == userId).ToList();
            if (list.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool canEdit = false;
            foreach (ClubMembership item in list)
            {
                canEdit |= item.CanEditClubData;
            }

            if (canEdit == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PayPalAccount payPalAccount = db.PayPalAccounts.Find(Id);
            if (payPalAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = Id;
            ViewBag.ClubId = clubId;

            return View(payPalAccount);
        }

        // POST: PayPalAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPayPalAccount([Bind(Include = "Id,ClubId,PayPalClientId,PayPalClientSecret")] PayPalAccount payPalAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payPalAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ViewPayPalAccount", new { clubId = payPalAccount.ClubId });
            }

            ViewBag.ClubId = new SelectList(db.Clubs, "Id", "name", payPalAccount.ClubId);
            return View(payPalAccount);
        }

        // GET: PayPalAccounts/Delete/5
        public ActionResult DeletePayPalAccount(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PayPalAccount payPalAccount = db.PayPalAccounts.Find(Id);
            if (payPalAccount == null)
            {
                return HttpNotFound();
            }

            ViewBag.Id = Id;
            ViewBag.ClubId = payPalAccount.ClubId;

            return View(payPalAccount);
        }

        // POST: PayPalAccounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePayPalAccount(int Id)
        {
            PayPalAccount payPalAccount = db.PayPalAccounts.Find(Id);

            if (payPalAccount == null)
            {
                return HttpNotFound();
            }

            int AffectedClubId = payPalAccount.ClubId;

            db.PayPalAccounts.Remove(payPalAccount);
            db.SaveChanges();
            return RedirectToAction("ViewPayPalAccount", new { clubId = AffectedClubId });
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
