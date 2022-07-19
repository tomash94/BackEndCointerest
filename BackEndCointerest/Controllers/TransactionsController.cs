using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class TransactionsController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            Transaction trnsc = new Transaction();
            List<Transaction> trnsc_list;
            try
            {
                trnsc_list = trnsc.Get_All_trans();
                if (trnsc_list.Count == 0)
                {
                    return BadRequest("No transactions found");
                }
                return Ok(trnsc_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET api/<controller>/5
        
        //return all transactions of users that this email follows
        public IHttpActionResult Get(string input_email)
        {
            Transaction trnsc = new Transaction();
            List<Transaction> trnsc_list;
            try
            {
                trnsc_list = trnsc.Get_transactions_of_friends(input_email);
                if(trnsc_list.Count == 0)
                {
                    return BadRequest("No transactions for this user");
                }
                return Ok(trnsc_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }


        public IHttpActionResult Get(string email, int n)
        {
            Transaction trnsc = new Transaction();
            List<Transaction> trnsc_list;
            try
            {
                trnsc_list = trnsc.Get_transactions_of_certain_user(email);
                if (trnsc_list.Count == 0)
                {
                    return BadRequest("No transactions for this user");
                }
                return Ok(trnsc_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        public IHttpActionResult Get( int n, string coin_name)
        {
            Transaction trnsc = new Transaction();
            List<Transaction> trnsc_list;
            try
            {
                trnsc_list = trnsc.Get_transactions_of_certain_coin(coin_name);
                if (trnsc_list.Count == 0)
                {
                    return BadRequest("No transactions for this coin");
                }
                return Ok(trnsc_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        public IHttpActionResult Get(string email, string coin_name) // all transactions of people "email" follows of a certain coin
        {
            Transaction trnsc = new Transaction();
            List<Transaction> trnsc_list;
            try
            {
                trnsc_list = trnsc.Get_trans_of_certain_coin(email,coin_name);
                if (trnsc_list.Count == 0)
                {
                    return BadRequest("No transactions for this coin");
                }
                return Ok(trnsc_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
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