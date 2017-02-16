using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;
using Thing1.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace Thing1.Controllers
{
    public class ClubsController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        // GET: Clubs
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
            List<ClubMembership> list = db.ClubMemberships.Where(c => c.UserId == userId).ToList();
            foreach(ClubMembership item in list)
            {
                if (dictionary.ContainsKey(item.ClubId)){
                    dictionary[item.ClubId] |= item.CanEditClubData; 
                } else {
                    dictionary.Add(item.ClubId, item.CanEditClubData);
                }   
            }

            ViewBag.ClubManageable = dictionary;

            var clubs = db.Clubs.Include(c => c.TypesOfClub);
            return View(clubs.ToList());
        }


        // GET: Clubs/Details/5
        /* public ActionResult Details(int? id)
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
         }*/

        public ActionResult Details(int? id)
        {
            ClubAndEventsViewModel combinedModel = new ClubAndEventsViewModel();
            combinedModel.club = db.Clubs.Find(id);
            DateTime currentTime = DateTime.Now;
            combinedModel.Events = db.Events.Where(e => e.StartsAt >= currentTime && e.PrimaryClubID == id).ToList();
            return View(combinedModel);
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
            ClubAndEventsViewModel combinedModel = new ClubAndEventsViewModel();
            combinedModel.club = db.Clubs.Find(id);
            DateTime currentTime = DateTime.Now;
            combinedModel.Events = db.Events.Where(e => e.StartsAt >= currentTime && e.PrimaryClubID == id).ToList();
            return View(combinedModel);
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
