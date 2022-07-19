using BackEndCointerest.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BackEndCointerest.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CoinsController : ApiController
    {
        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public IHttpActionResult Get()
        {
            Coin coin = new Coin();
            List<Coin> coins_list;
            try
            {
               coins_list = coin.Get_Coins_with_latest_price_only();
                return Ok(coins_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        public IHttpActionResult Get(string coin_name, DateTime start, DateTime finish)
        {
            //example {"start": "2017-11-01T00:00:00"}
            Coin_update cu = new Coin_update();
            List<Coin_update> prices_list;
            try
            {
                prices_list = cu.get(coin_name,start,finish);
                return Ok(prices_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        public IHttpActionResult Get(string coin_name, string interval_type, DateTime start, DateTime finish)
        {
            //example {"start": "2017-MM-DDT00:00:00"}
            List<Value_interval> result = new List<Value_interval>();
            List<Graph_value_interval> admin_panel_result = new List<Graph_value_interval>();
            Coin coin = new Coin();
            if (interval_type == "C")
            {
                admin_panel_result = coin.get_comp(coin_name, interval_type, start, finish);
                return Ok(admin_panel_result);
            }

            
            try
            {
                return Ok(coin.graph_data(coin_name, interval_type, start, finish));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
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