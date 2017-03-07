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
    // manage and control paypal account (PayPal's Client ID and Client Secret for each club)
    public class PayPalAccountsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: PayPalAccounts
        public ActionResult Index()
        {
            var payPalAccounts = db.PayPalAccounts.Include(p => p.Club);
            return View(payPalAccounts.ToList());
        }

        // view current paypal Client ID and Client Secret for the club
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
        // detail information of paypal Client ID and Client Secret
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
        // Create a new paypal Client ID and Client Secret for the club
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
        // Each club should have only one paypal Client ID and Client Secret. This controller will show an error if club officer trys to register another Client ID and Client Secret given that the club already has one
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
        // After getting new Client ID and Client Secret from the club officer, save them to the database
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
        // The club office can modify the current Client ID and Client Secret values
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
        // After getting modified values of the Client ID and Client Secret from the club officer, save them to the database
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
        // The club office can delete Client ID and Client Secret
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
        // Delete the current Client ID and Client Secret in the database
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
