﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
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
        public IEnumerable<MemberDto> GetClubOfficers(int id)
        {

            return (from anu in db.AspNetUsers
                    join cm in db.ClubMemberships on anu.Id equals cm.UserId
                    join c in db.Clubs on cm.ClubId equals c.Id
                    where c.Id == id
                    //TODO: uncomment missing condition to filter only officers
                    //&& cm.IsCurrentOfficer == true
                    select new MemberDto {
                        FirstName = anu.FirstName,
                        LastName = anu.LastName,
                        Email = anu.Email,
                        Program = anu.Program
                    });
        }

        // GET: api/ClubsWS/5/Events
        [Route("{id:int}/Events")]
        public IEnumerable<EventDto> GetClubEvents(int id)
        {
            //TODO: uncomment missing condition for upcoming events
            //var eventIds = db.Events.Where(e => e.EndsAt > DateTime.Now && e.Clubs.Any(c => c.Id == id)).Select(e => e.Id).ToList();
            var eventIds = db.Events.Where(e => e.Clubs.Any(c => c.Id == id)).Select(e => e.Id).ToList();
            var events = db.Events.Where(e => eventIds.Contains(e.Id)).OrderBy(e => e.StartsAt).Include(e => e.Clubs).ToList();
            return events.Select(o =>
                new EventDto
                {
                    Title = o.Title,
                    Date = o.StartsAt.ToString("D"),
                    StartsAt = o.StartsAt.ToString("t"),
                    EndsAt = o.EndsAt.ToString("t"),
                    Location = o.Location,
                    Description = o.Description,
                    IsPublic = o.IsPublic,
                    Clubs = o.Clubs.Select(x => x.nickname).ToList<String>(),
                    Food = o.Food == null ? "" : o.Food,
                    Contact = o.Contact == null ? "" : o.Contact,
                    Price = o.Price.ToString()
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