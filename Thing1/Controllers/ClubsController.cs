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
    public class ClubsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: Clubs
        public ActionResult Index()
        {
            var clubs = db.Clubs.Include(c => c.TypesOfClub);
            return View(clubs.ToList());
        }

        // GET: Clubs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // GET: Clubs/Create
        public ActionResult Create()
        {
            ViewBag.typeId = new SelectList(db.TypesOfClubs, "Id", "Description");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,typeId,nickname,email,website,description,image,email_template,one_year_fee,two_year_fee,three_year_fee,other_fee_1,other_fee_2")] Club club)
        {
            if (ModelState.IsValid)
            {
                db.Clubs.Add(club);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.typeId = new SelectList(db.TypesOfClubs, "Id", "Description", club.typeId);
            return View(club);
        }

        // GET: Clubs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            ViewBag.typeId = new SelectList(db.TypesOfClubs, "Id", "Description", club.typeId);
            return View(club);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,typeId,nickname,email,website,description,email_template,one_year_fee,two_year_fee,three_year_fee,other_fee_1,other_fee_2")] Club club, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                /*if (image!=null)
                {
                    byte[] theImage = new byte[image.ContentLength];
                    image.InputStream.Read(theImage, 0, theImage.Length);
                    club.image = theImage;
                }*/
                db.Entry(club).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.typeId = new SelectList(db.TypesOfClubs, "Id", "Description", club.typeId);
            return View(club);
        }

        // GET: Clubs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Club club = db.Clubs.Find(id);
            db.Clubs.Remove(club);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Management(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
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
