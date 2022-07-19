using BackEndCointerest.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static BackEndCointerest.coins_api;

namespace BackEndCointerest
{



    public class CoinMarketCapLoop
    {
        public CoinMarketCapLoop()
        {

        }


        private static string API_KEY = "f70ecc73-65a0-4947-a8a9-95ef0e273d1a";

        static string makeAPICall()
        {
            var URL = new UriBuilder("https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            //queryString["symbol"] = "BTC,ETH";
            queryString["start"] = "1";
            queryString["limit"] = "200";
            queryString["convert"] = "USD";

            URL.Query = queryString.ToString();

            var client = new WebClient();
            client.Headers.Add("X-CMC_PRO_API_KEY", API_KEY);
            client.Headers.Add("Accepts", "application/json");
            return client.DownloadString(URL.ToString());

        }

        public async Task<List<Coin_update>> api_func()
        {
            try
            {
                string response = makeAPICall();
                var model = JsonConvert.DeserializeObject<Root>(response);
                List<Coin_update> updates = new List<Coin_update>();
                Coin_update c_update = new Coin_update();
                foreach (var item in model.data)
                {
                    if (item.name == "Bitcoin" || item.name == "Dogecoin" || item.name == "Carno" || item.name == "XRP" || item.name == "Ethereum" || item.name == "Solana" || item.name == "BNB" || item.name == "Polygon" || item.name == "Cosmos" || item.name == "NEAR Protocol")
                    {
                        //add volume_24h 
                        c_update = new Coin_update();
                        c_update.Coin_name = item.name;
                        c_update.Coin_value = (float)item.quote.USD.price;
                        c_update.Symbol = item.symbol;
                        c_update.Update_date = DateTime.Now;
                        c_update.Percent_change_1h = (float)item.quote.USD.percent_change_1h;
                        c_update.Percent_change_24h = (float)item.quote.USD.percent_change_24h;
                        c_update.Percent_change_30d = (float)item.quote.USD.percent_change_30d;
                        c_update.Percent_change_7d = (float)item.quote.USD.percent_change_7d;
                        c_update.Volume_24h = (double)item.quote.USD.volume_24h;
                        updates.Add(c_update);
                    }
                }


               return updates;


            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return null;
        } 


        public async Task PeriodicFooAsync(int n = 600)
        {

            while (true)
            {

                if (1 == 1)
                {
                    Thread.Sleep(1000*n);
                }
                else return;
            }
        }
    }
}