using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class LoginsController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(string email)
        {
            Login li = new Login();
            List<Login> login_list;
            try
            {
                login_list = li.get_logins(email);
                if(login_list.Count == 0)
                {
                    return BadRequest("There's no listed logins for this email");
                }
                return Ok(login_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        // POST api/<controller>
        public IHttpActionResult Post(string email)
        {
            User user = new User();
            try
            {
                if (user.login(email, DateTime.Now) == 0)
                {
                    return BadRequest("There's no such email in the Db");
                }
                else
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + email) ,email + " logged in at "+DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

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