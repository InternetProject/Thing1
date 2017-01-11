using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Thing1.Models;
using Microsoft.AspNet.Identity;

namespace Thing1.Controllers
{
    
    public class PaymentHistoryController : Controller
    {
        private user_managementEntities _db = new user_managementEntities();
        // GET: PaymentHistory
        [Authorize]
        public ActionResult Index()
        {

           string userId = User.Identity.GetUserId();

           return View(_db.payments.Where(x => x.userID == userId).ToList());
        }

        // GET: PaymentHistory/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PaymentHistory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaymentHistory/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentHistory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaymentHistory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PaymentHistory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaymentHistory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
