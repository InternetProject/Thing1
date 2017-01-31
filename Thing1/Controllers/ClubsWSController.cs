using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Thing1.DTOs;
using Thing1.Models;

namespace Thing1.Controllers
{
    [RoutePrefix("api/ClubsWS")]
    [EnableCors(origins: "http://localhost", headers: "*", methods: "*")]
    public class ClubsWSController : ApiController
    {
        private user_managementEntities db = new user_managementEntities();

        ClubsWSController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: api/ClubsWS
        public IQueryable<Club> GetClubs()
        {
            return db.Clubs;
        }

        // GET: api/ClubsWS/5
        [ResponseType(typeof(Club))]
        public IHttpActionResult GetClub(int id)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }

            return Ok(club);
        }

        // PUT: api/ClubsWS/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClub(int id, Club club)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != club.Id)
            {
                return BadRequest();
            }

            db.Entry(club).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClubExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ClubsWS
        [ResponseType(typeof(Club))]
        public IHttpActionResult PostClub(Club club)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Clubs.Add(club);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ClubExists(club.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = club.Id }, club);
        }

        // DELETE: api/ClubsWS/5
        [ResponseType(typeof(Club))]
        public IHttpActionResult DeleteClub(int id)
        {
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return NotFound();
            }

            db.Clubs.Remove(club);
            db.SaveChanges();

            return Ok(club);
        }

        // GET: api/ClubsWS/5/Officers
        [Route("{id:int}/Officers")]
        public IQueryable<MemberDto> GetClubOfficers(int id)
        {

            return (from anu in db.AspNetUsers
                    join cm in db.ClubMemberships on anu.Id equals cm.UserId
                    join c in db.Clubs on cm.ClubId equals c.Id
                    where c.Id == id
                    //missing condition for officers
                    //select anu);
                    select new MemberDto {
                        FirstName = anu.FirstName,
                        LastName = anu.LastName,
                        Email = anu.Email,
                        Program = anu.Program
                    });
            //return db.ClubMemberships
            //  .Where(cm => cm.Club.Id == id);
        }

        // GET: api/ClubsWS/5/Events
        [Route("{id:int}/Events")]
        public IQueryable<EventDto> GetClubEvents(int id)
        {

            return (from e in db.Events
                    where e.ClubId == id
                    
                    select new EventDto
                    {
                        StartsAt = e.StartsAt,
                        Title = e.Title,
                        Location = e.Location,
                        Description = e.Description
                    });
         }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClubExists(int id)
        {
            return db.Clubs.Count(e => e.Id == id) > 0;
        }
    }
}