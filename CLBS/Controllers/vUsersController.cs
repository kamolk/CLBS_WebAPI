using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CLBS;
using CLBS.Models;
using EntityState = System.Data.Entity.EntityState;

namespace CLBS.Controllers
{
    public class vUsersController : ApiController
    {
        private PRISMCOLLECTION db = new PRISMCOLLECTION();

        // GET: api/vUsers
        [HttpGet]
        public IEnumerable<Users> GetvUsers()
        {
            try
            {
                var USR = db.Users;
                var Users = new List<Users>();
                foreach(var u in USR)
                {
                    var data = new Users
                    {
                        ID = u.ID,
                        Username = u.Username,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        UserEmail = u.UserEmail
                    };
                    Users.Add(data);
                }
               

                return Users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return db.vUsers;
        }

        // GET: api/vUsers/5
        [ResponseType(typeof(Users))]
        public async Task<IHttpActionResult> GetvUsers(string id)
        {
            var Users = await db.Users.FindAsync(id);
            if (Users == null)
            {
                return NotFound();
            }
            var user = new Users()
            {
                ID = Users.ID,
                Username = Users.Username,
                FirstName = Users.FirstName,
                LastName = Users.LastName,
                UserEmail = Users.UserEmail
            };

            return Ok(user);
        }

        // PUT: api/vUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutvUsers(string id, Users vUsers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vUsers.ID)
            {
                return BadRequest();
            }

            db.Entry(vUsers).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(200);
        }

        // POST: api/vUsers
        [ResponseType(typeof(Users))]
        public async Task<IHttpActionResult> PostvUsers(Users Users)
        {
            var s = db.Users.Count(e => e.ID == Users.ID) ;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (UsersExists(Users.ID))
                {
                    return Conflict();
                }
                else
                {
                    db.vUsers.Add(Users);
                    await db.SaveChangesAsync();
                
                }
            }
            catch (DbEntityValidationException e)
            {
                var newException = new FormattedDbEntityValidationException(e);
                throw newException;
            }

            return Ok(200);
        }

        // DELETE: api/vUsers/5
        [ResponseType(typeof(Users))]
        public async Task<IHttpActionResult> DeletevUsers(string id)
        {
            Users vUsers = await db.vUsers.FindAsync(id);
            if (vUsers == null)
            {
                return NotFound();
            }

            db.vUsers.Remove(vUsers);
            await db.SaveChangesAsync();

            return Ok(vUsers);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsersExists(string id)
        {
            return db.Users.Count(e => e.ID == id) > 0;
        }
    }
}