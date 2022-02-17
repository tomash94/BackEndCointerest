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
        //public IHttpActionResult Post(string email, string discover_user)
        //{
        //    User user = new User();
        //    try
        //    {
        //        if (0== 0)
        //        {
        //            return BadRequest("Email already exists in DB, try a new email");
        //        }
        //        else
        //        {
        //            //give the new user his allowance
        //            Asset ast = new Asset("USD", user.Email, 100000);
        //            if (user.Email == "i2cs2013@gmail.com" || user.Email == "hayun.ori@gmail.com")
        //            {
        //                ast.Amount = 9999999;
        //            }
        //            ast.Insert(ast);
        //            return Created(new Uri(Request.RequestUri.AbsoluteUri + user.Username), user);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.BadRequest, ex);
        //    }
        //}

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}