using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Thing1.Models;
using System.Web.Mvc;
using System.Data.Entity;

namespace Thing1.Controllers
{
    public class HomeController : Controller
    {
        private user_managementEntities db = new user_managementEntities();

        public ActionResult Index()
        {
            /*if(!Request.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized); // should change this later.
            }
            */
            var userid = User.Identity.GetUserId();
           // var clubMemberships = db.ClubMemberships.Include(c => c.Club);
            return View();// clubMemberships.ToList());//.Where(c => c.UserId == userid).ToList());
        }
    }
}