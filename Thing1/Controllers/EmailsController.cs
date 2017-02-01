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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Emails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Subject,Body")] Email email, String everyone, String directors, String vps, String ft1styears, String ft2ndyears)
        {
            if (ModelState.IsValid)
            {
                email.SentBy = User.Identity.GetUserId();
                email.ClubId = 2;
                email.Sent = DateTime.Now;

                var recipients = db.ClubMemberships.Where(c => everyone.Equals("on") | ((directors.Equals("on") & c.Description.StartsWith("Director ")) | (vps.Equals("on") & (c.Description.StartsWith("VP ") | c.Description.StartsWith("President"))) | (ft1styears.Equals("on") & c.AspNetUser.Program.Equals("FT "+DateTime.Now.Year + (DateTime.Now.Month < 7 ? 1 : 2))) | (ft2ndyears.Equals("on") & c.AspNetUser.Program.Trim().Equals("FT " + (DateTime.Now.Year + (DateTime.Now.Month < 7 ? 0 : 1))))) ).ToList();
                var message = new MailMessage();

                for (int i=0; i<recipients.Count; i++)
                {
                    message.Bcc.Add(new MailAddress(recipients[i].AspNetUser.Email));
                }

                message.Subject = email.Subject;
                message.Body = email.Body;
                message.From = new MailAddress(db.AspNetUsers.Find(email.SentBy).Email);

                db.Emails.Add(email);
                db.SaveChanges();
                // still need to insert into Recipients.

                
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "the.internet.project.ucla@gmail.com",
                        Password = "ivowelch"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return RedirectToAction("Index");
                }
                

                return RedirectToAction("Index");
            }

            return View(email);
        }

        private async Task<ActionResult> sendMessageAsync(MailMessage message)
        {
            return null;
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
