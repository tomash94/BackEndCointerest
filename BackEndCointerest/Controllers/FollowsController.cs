using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class FollowsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(string email, string discover_user)
        {

            User user = new User();

            try
            {
                if (user.check_follow(email, discover_user) == 0)
                {
                    return Ok("False"); 
                }
                else return Ok("True");

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        // POST api/<controller>
        public IHttpActionResult Post(string email, string discover_user)
        {
            User user = new User();
            Following f = new Following(email, discover_user);
            

            try
            {
                if (user.post_follow(f) == 1)
                {
                    return Ok(email+" Is now following "+discover_user);
                }
                else
                {
                    return BadRequest("Something went wrong with this action");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(string email, string discover_user)
        {
            User user = new User();
            Following f = new Following(email, discover_user);


            try
            {
                if (user.Delete(f) == 1)
                {
                    return Ok(email + " Is now un-following " + discover_user);
                }
                else
                {
                    return BadRequest("Something went wrong with this action");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error Message: " + ex.Message);
            }
        }
    }
}