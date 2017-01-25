using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Thing1.Models;

namespace Thing1.Controllers
{
    public class MyProfileController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: MyProfile
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            var userid = User.Identity.GetUserId();
            AspNetUser myProfile = db.AspNetUsers.Find(userid);
            if (myProfile == null)
            {
                return HttpNotFound();
            }
            return View(myProfile);
        }

        // GET: MyProfile/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: MyProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,LastName,FirstName,PreferredName,Program,TShirtSize")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.Entry(aspNetUser).Property(p => p.EmailConfirmed).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.SecurityStamp).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.PasswordHash).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.PhoneNumberConfirmed).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.TwoFactorEnabled).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.LockoutEndDateUtc).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.LockoutEnabled).IsModified = false;
                db.Entry(aspNetUser).Property(p => p.AccessFailedCount).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }
    }
}

