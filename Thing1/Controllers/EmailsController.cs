using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace Thing1.Controllers
{
    public class EmailsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: Emails
        public ActionResult Index()
        {
            return View(db.Emails.ToList());
        }

        // GET: Emails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // GET: Emails/Create
        public ActionResult Create(int? clubId)
        {
            return View();
        }

        public JsonResult RecipientEmails(int? ClubId, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas)
        {
            List<ClubMembership> recipients = RecipientList(ClubId, everyone, directors, vps, ft1styears, ft2ndyears, fembas);
            if (recipients == null) return Json(new { Recipients= "ERROR" }, JsonRequestBehavior.DenyGet);

            String recipientEmails = "";
            foreach (ClubMembership recipient in recipients)
            {
                recipientEmails += ", " + recipient.AspNetUser.Email;
            }
            if (recipientEmails.Length == 0) recipientEmails = "   ";

            return Json(new { Recipients=recipientEmails.Substring(2) }, JsonRequestBehavior.DenyGet);
        }

        private List<ClubMembership> RecipientList(int? ClubId, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas)
        {
            string theUserId = User.Identity.GetUserId();
            if (ClubId == null | ClubId == 0 | User.Identity == null | db.ClubMemberships.Where(c => c.AspNetUser.Id == theUserId & c.ClubId == ClubId & (c.Description.StartsWith("Director ") | c.Description.StartsWith("VP ") | c.Description.StartsWith("President"))).ToList().Count == 0)
            {
                return null;
            }
            return db.ClubMemberships.Where(c => (c.ClubId == ClubId) & (c.JoinDate < DateTime.Now & c.TermDate > DateTime.Now) & (everyone.Equals("on") | ((directors.Equals("on") & c.Description.StartsWith("Director ")) | (vps.Equals("on") & (c.Description.StartsWith("VP ") | c.Description.StartsWith("President"))) | (ft1styears.Equals("on") & c.AspNetUser.Program.Equals("FT " + DateTime.Now.Year + (DateTime.Now.Month < 7 ? 1 : 2))) | (ft2ndyears.Equals("on") & c.AspNetUser.Program.Trim().Equals("FT " + (DateTime.Now.Year + (DateTime.Now.Month < 7 ? 0 : 1)))))) | (fembas.Equals("on") & c.AspNetUser.Program.Trim().StartsWith("FEMBA"))).OrderBy(c => c.AspNetUser.Email).ToList();
        }

        // POST: Emails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Subject,Body")] int? ClubId, Email email, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas)
        {
            if (ModelState.IsValid)
            {
                List<ClubMembership> recipients = RecipientList(ClubId, everyone, directors, vps, ft1styears, ft2ndyears, fembas);
                if (recipients == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                email.SentBy = User.Identity.GetUserId();
                email.ClubId = (int)ClubId;
                email.Sent = DateTime.Now;

                MailMessage message = new MailMessage();

                foreach (ClubMembership recipient in recipients)
                {
                    message.Bcc.Add(new MailAddress(recipient.AspNetUser.Email));
                }

                message.Subject = email.Subject;
                message.Body = email.Body;
                message.From = new MailAddress(db.AspNetUsers.Find(email.SentBy).Email);// "the.internet.project.ucla@gmail.com");
                message.ReplyToList.Add(message.From);

                db.Emails.Add(email);
                db.SaveChanges();
                // still need to insert into Recipients.


                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential { UserName = "the.internet.project.ucla@gmail.com", Password = "ivowelch" };
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                    return RedirectToAction("Index");
                }

                //return RedirectToAction("Index");
            }

            return View(email);
        }

        // GET: Emails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // POST: Emails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Subject,Body")] Email email)
        {
            if (ModelState.IsValid)
            {
                db.Entry(email).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(email);
        }

        // GET: Emails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Email email = db.Emails.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        // POST: Emails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Email email = db.Emails.Find(id);
            db.Emails.Remove(email);
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
