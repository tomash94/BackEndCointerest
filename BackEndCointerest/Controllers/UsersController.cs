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
            List<TopUser> t_list = new List<TopUser>();
            User back_user = new User();
            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    t_list = back_user.top_users();
                    return Ok(t_list);
                }
                if(search == "*")
                {
                    return Ok(back_user.get_users());
                }

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
                    
                    ast.Insert(ast);
                    Transaction ts = new Transaction(ast.Email, ast.Coin_name, DateTime.Now, ast.Amount,   1, "Welcome to Cointerest app! invest wisely ;)");
                    ts.Insert(ts);
                    Wallet_worth wallet_worth = new Wallet_worth();
                    wallet_worth.insert_new_user(user.Email);

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
        public IHttpActionResult Put(int id, string email, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return BadRequest("ERROR - the value is null or empty");
            }
            if(id == 0) //update bio
            {
                User u = new User();
                try
                {
                    int n = u.update_bio(email, value);
                    if(n == 1)
                    {
                        return Ok(" - Bio updated for " + email);
                    }
                    if (n == 0)
                    {
                        return BadRequest("ERROR - no email found in order to update the bio");
                    }
                }
                catch (Exception ex)
                {
                    return Content(HttpStatusCode.BadRequest, ex);
                }
            }

            return BadRequest("ERROR - no action set for id = " + id);
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}