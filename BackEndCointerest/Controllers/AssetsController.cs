using BackEndCointerest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BackEndCointerest.Controllers
{
    public class AssetsController : ApiController
    {
        // GET api/<controller>
        public IHttpActionResult Get(string email)
        {
            Asset ast = new Asset();
            List<Asset> asset_list;
            try
            {
                asset_list = ast.get_assets_of_certain_user(email);
                return Ok(asset_list);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        public IHttpActionResult Get(string email, int n)
        {
            Asset ast = new Asset();
            List<Asset> asset_list;
            try
            {
                asset_list = ast.get_assets_of_certain_user(email);
                float balance = 0;
                foreach(Asset ass in asset_list)
                {
                    balance += ass.Asset_worth_in_USD;
                }
                return Ok(balance);
            }
            catch (Exception ex)
            {
                //return BadRequest(ex.Massage);
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Asset input_asset, string user_comment)
        {
            float price_of_coin = -1;
            // first we save an asset object holding the user's dollars 
            Asset user_dollar_asset = new Asset();
            Asset user_owned_asset = new Asset();

            try
            {
                //get the values of all the coins in DB
                List<Coin> coins_list = new List<Coin>();
                Coin coins = new Coin();
                coins_list = coins.Get_Coins_with_latest_price_only();

                //find the price of the coin that the user wants to buy/sell
                foreach (Coin coin in coins_list)
                {
                    if (coin.Coin_name == input_asset.Coin_name)
                    {
                        price_of_coin = coin.Price_history[0].Coin_value * input_asset.Amount;
                    }

                }

                List<Asset> user_asset_list = user_dollar_asset.get_assets_of_certain_user(input_asset.Email);
                foreach (Asset iterable_asset in user_asset_list)
                {
                    if (iterable_asset != null)
                    {
                        if (iterable_asset.Coin_name != null)
                        {
                            if (iterable_asset.Coin_name == "USD")
                            {
                                user_dollar_asset = iterable_asset;
                            }

                            if(iterable_asset.Coin_name == input_asset.Coin_name )
                            {
                                user_owned_asset = iterable_asset;
                            }
                        }
                    }
                }
                //return error response if user does not have USD asset
                if (user_dollar_asset.Coin_name == null) return BadRequest("no dollars found in your account");


                // if the amount is bigger than 0 that means he wants to buy, 
                // which means we need to check if he has sufficient amount of USD 
                if (input_asset.Amount > 0)
                {
                    if (price_of_coin == -1)
                    {
                        return BadRequest("No coin found with name: " + input_asset.Coin_name);
                    }
                    if (price_of_coin > user_dollar_asset.Amount)
                    {
                        return BadRequest("Not enough USD to make the purchase, you need an additional " + (price_of_coin - user_dollar_asset.Amount) + " USD to make the purchase");
                    }
                    else // if everything is correct, update the values of USD and specific coin in the user's assets
                    {
                        
                        input_asset.Insert(input_asset);
                        user_dollar_asset.Amount = -price_of_coin;
                        user_dollar_asset.Insert(user_dollar_asset);

                        //input a transaction log to DB
                        Transaction ts = new Transaction(input_asset.Email, input_asset.Coin_name, DateTime.Now,input_asset.Amount, (-1)*price_of_coin, user_comment);
                        ts.Insert(ts);

                        return Created(new Uri(Request.RequestUri.AbsoluteUri + input_asset.Coin_name), input_asset);
                    }

                }

                //if the amount is 0 it means buying or selling 0 coins which is an error/user mistake
                if (input_asset.Amount == 0) return BadRequest("You can't buy or sell 0 coins");
                
                //if amount<0 that means he wants to sell
                if(input_asset.Amount < 0)
                {
                    if(user_owned_asset == null) return BadRequest("You don't own any " + input_asset.Coin_name);

                    if ((-1) * (input_asset.Amount) > user_owned_asset.Amount) //means he doesn't have enough of that coin to sell the amount he requested 
                    { 
                        return BadRequest("Error: \n"+input_asset.Email+" is trying to sell "+input_asset.Amount+" of "+input_asset.Coin_name+" but only owns "+user_owned_asset.Amount); 
                    }
                    else // if everything is correct, update the values of USD and specific coin in the user's assets
                    {
                        user_dollar_asset.Amount = (-1)*price_of_coin; // minus because the amount is negative and the user gained dollars and not lost them.
                        user_dollar_asset.Insert(user_dollar_asset);
                        input_asset.Insert(input_asset);

                        //input a transaction log to DB
                        Transaction ts = new Transaction(input_asset.Email, input_asset.Coin_name, DateTime.Now, input_asset.Amount, price_of_coin, user_comment);
                        ts.Insert(ts);

                        return Created(new Uri(Request.RequestUri.AbsoluteUri + input_asset.Coin_name), input_asset);
                    }
                }

                return Content(HttpStatusCode.InternalServerError, "There was a server issue, this message should'nt appear");

            }

            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }


        // When a new user is created - this will add a dollar asset to their account
        // should only be called in the "sign up" after signing up
        public IHttpActionResult Post(string email)
        {
            Asset ast = new Asset("USD", email, 100000);
            if (email == "i2cs2013@gmail.com" || email == "hayun.ori@gmail.com")
            {
                ast.Amount = 9999999;
            }
            
            try
            {
                ast.Insert(ast);
                return Created(new Uri(Request.RequestUri.AbsoluteUri + ast.Coin_name), ast);
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