using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class TweetsController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public IHttpActionResult Get()
        {
            Tweet t = new Tweet();
            List<Tweet> answer;
            try
            {
              answer =  t.get();
            }
            catch (Exception ex)
            {
                return BadRequest("Error message: " + ex.Message);
            }
            return Ok(answer);
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] List<Tweet> tweets)
        {
            Tweet t = new Tweet();
            int amount;
            try
            {
                amount = t.insert(tweets); 
            }
            catch(Exception ex)
            {
                return BadRequest("Error message: " + ex.Message);
            }
            return Ok("Ok - "+amount + " tweets were added to the db");
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