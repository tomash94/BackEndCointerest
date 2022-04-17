using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class UsersController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            
            
           
            //while (true) { }
            return new string[] { "api callback loop inplace", "value2" };
        }

        // GET api/<controller>/5
        public IHttpActionResult Get(string email, string password)
        {

            User back_user = new User();
            try
            {

                back_user = back_user.get_user(email);
                if (back_user != null) 
                {
                    if (back_user.Email != null)
                    {
                        if (back_user.Password == password)
                        {
                            return Ok(back_user);
                        }
                        else return BadRequest("Password or email inccorect");
                    }
                    else return BadRequest("Password or email inccorect");
                }
                else return BadRequest("Password or email inccorect");

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }



        }

        public IHttpActionResult Get(string email, int n)
        {

            User back_user = new User();
            try
            {

                back_user = back_user.get_user(email);
                if (back_user != null)
                {
                    back_user.Password = "null";
                    return Ok(back_user);
                }
                else return BadRequest("email inccorect");

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }




        public IHttpActionResult Get(string search)
        {
            List<User> user_list = new List<User>();
            User back_user = new User();
            try
            {
                user_list = back_user.search_users(search);
                if (user_list[0] != null)
                {
                    if (user_list[0].Username != null)
                    {
                        
                            return Ok(user_list);
                        
                    }
                    else return BadRequest("No user found");
                }
                else return BadRequest("No user found");

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }



        // POST api/<controller>
        public IHttpActionResult Post([FromBody] User user)
        {
            //give new user a default profile picture
            user.Image = "http://194.90.158.74/bgroup53/test2/tar4/Assets/default.png";

            try
            {
                if (user.Insert() == 0)
                {
                    return BadRequest("Email already exists in DB, try a new email");
                }
                else
                {
                    //give the new user his allowance
                    Asset ast = new Asset("USD", user.Email, 100000);
                    if (user.Email == "i2cs2013@gmail.com" || user.Email == "hayun.ori@gmail.com")
                    {
                        ast.Amount = 9999999;
                    }
                    ast.Insert(ast);
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + user.Username), user);
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }



        public IHttpActionResult Post(string Email, string pic_url)
        {
            User user = new User();

            try
            {
                if (user.Change_Image(Email,pic_url) == 0)
                {
                    return BadRequest("There was a problem while trying to update the picture for: "+Email);
                }
                else
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + pic_url), pic_url);
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