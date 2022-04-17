using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class PredictionsController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get(string coin_name)
        {
            Prediction p = new Prediction();
            List<Prediction> pl = new List<Prediction>();
            try
            {
                pl = p.get(coin_name);
                if ( pl == null)
                {
                    return BadRequest("no predictions for this coin");

                }
                else return Ok(pl);
            }
            catch (Exception ex)
            {
                return BadRequest("error message: " + ex.Message);
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post(string coin_name, float predicted_price, float current_price)
        {
            Prediction p = new Prediction();
            try
            {
                if (p.insert("Bitcoin", predicted_price, current_price) == 0)
                {
                    return BadRequest("something went wrong, did not insert");

                }
                else return Created(new Uri(Request.RequestUri.AbsoluteUri ), "created a new prediction for: " + coin_name);
            }
            catch(Exception ex)
            {
                return BadRequest("error message: " + ex.Message);
            }
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put(string coin_name, float price)
        {
            Prediction p = new Prediction();
            p.X_current_price = -1;
            p.Coin_name = coin_name;
            p.Y_true_price = price;

            try
            {
                if (p.update(p) == 0)
                {
                    return BadRequest("something went wrong, no rows were updated");

                }
                else return Created(new Uri(Request.RequestUri.AbsoluteUri), "Updated the true price of yesterday's prediction for: " + p.Coin_name);
            }
            catch (Exception ex)
            {
                return BadRequest("error message: " + ex.Message);
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}