﻿using System;
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
    /// <summary>
    /// Sends Emails.
    /// </summary>
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

        /// <summary>
        /// Grabs a JSON list of emails based on the ClubID and the checked boxes
        /// </summary>
        /// <param name="ClubId">ClubId</param>
        /// <param name="everyone">on or off</param>
        /// <param name="directors">on or off</param>
        /// <param name="vps">on or off</param>
        /// <param name="ft1styears">on or off</param>
        /// <param name="ft2ndyears">on or off</param>
        /// <param name="fembas">on or off</param>
        /// <param name="formermembers"></param>
        /// <returns>Emails in a String</returns>
        public JsonResult RecipientEmails(int? ClubId, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas, String formermembers)
        {
            List<ClubMembership> recipients = RecipientList(ClubId, everyone, directors, vps, ft1styears, ft2ndyears, fembas, formermembers);
            if (recipients == null) return Json(new { Recipients= "Could not pull recipient list... Are you sure you're logged in and an officer of this club?" }, JsonRequestBehavior.DenyGet);

            String recipientEmails = "";
            foreach (ClubMembership recipient in recipients)
            {
                recipientEmails += ", " + recipient.AspNetUser.Email;
            }
            if (recipientEmails.Length == 0) recipientEmails = "   ";

            return Json(new { Recipients=recipientEmails.Substring(2) }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// Grabs a list of ClubMembership objects based on the ClubID and the checked boxes
        /// </summary>
        /// <param name="ClubId">ClubId</param>
        /// <param name="everyone">on or off. Not everyone anymore, just all CURRENT members.</param>
        /// <param name="directors">on or off</param>
        /// <param name="vps">on or off</param>
        /// <param name="ft1styears">on or off</param>
        /// <param name="ft2ndyears">on or off</param>
        /// <param name="fembas">on or off</param>
        /// <param name="formermembers"></param>
        /// <returns>List of ClubMemberships</returns>
        private List<ClubMembership> RecipientList(int? ClubId, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas, String formermembers)
        {
            string theUserId = User.Identity.GetUserId();
            if (ClubId == null | ClubId == 0 | User.Identity == null | db.ClubMemberships.Where(c => c.AspNetUser.Id == theUserId & c.ClubId == ClubId & c.IsCurrentOfficer==true).ToList().Count == 0)
            {
                return null;
            }
            return db.ClubMemberships.Where(c => (c.ClubId == ClubId) & ((formermembers.Equals("on") & c.TermDate < DateTime.Now) | ((c.JoinDate < DateTime.Now & c.TermDate > DateTime.Now) & (everyone.Equals("on") | ((directors.Equals("on") & c.Description.StartsWith("Director ")) | (vps.Equals("on") & (c.Description.StartsWith("VP ") | c.Description.StartsWith("President"))) | (ft1styears.Equals("on") & c.AspNetUser.Program.Equals("FT " + DateTime.Now.Year + (DateTime.Now.Month < 7 ? 1 : 2))) | (ft2ndyears.Equals("on") & c.AspNetUser.Program.Trim().Equals("FT " + (DateTime.Now.Year + (DateTime.Now.Month < 7 ? 0 : 1)))))) | (fembas.Equals("on") & c.AspNetUser.Program.Trim().StartsWith("FEMBA"))))).OrderBy(c => c.AspNetUser.Email).ToList();
        }

        // POST: Emails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Sends the emails
        /// </summary>
        /// <param name="ClubId"></param>
        /// <param name="email"></param>
        /// <param name="everyone"></param>
        /// <param name="directors"></param>
        /// <param name="vps"></param>
        /// <param name="ft1styears"></param>
        /// <param name="ft2ndyears"></param>
        /// <param name="fembas"></param>
        /// <param name="formermembers"></param>
        /// <returns>async</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Subject,Body")] int? ClubId, Email email, String everyone, String directors, String vps, String ft1styears, String ft2ndyears, String fembas, String formermembers)
        {
            if (ModelState.IsValid)
            {
                List<ClubMembership> recipients = RecipientList(ClubId, everyone, directors, vps, ft1styears, ft2ndyears, fembas, formermembers);
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
                    await smtp.SendMailAsync(message);
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
