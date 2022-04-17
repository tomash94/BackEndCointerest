using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Coin
    {
        private string coin_name;
        private string coin_info;
        private string coin_url;
        private string coin_picture;
        private List<Coin_update> price_history;


        public Coin()
        {

        }


        public Coin(string coin_name, string coin_info, string coin_url, string coin_picture, List<Coin_update> price_history)
        {
            this.coin_name = coin_name;
            this.coin_info = coin_info;
            this.coin_url = coin_url;
            this.coin_picture = coin_picture;
            this.price_history = price_history;
        }

        public string Coin_name { get => coin_name; set => coin_name = value; }
        public string Coin_info { get => coin_info; set => coin_info = value; }
        public string Coin_url { get => coin_url; set => coin_url = value; }
        public string Coin_picture { get => coin_picture; set => coin_picture = value; }
        public List<Coin_update> Price_history { get => price_history; set => price_history = value; }



        public List<Coin> Get_Coins_with_latest_price_only()
        {
            DBServices ds = new DBServices();
            List<Coin> coins = ds.Get_Coins_with_latest_price_only();
            return coins;
        }


        public List<Coin> Get_Coins_with_all_prices()
        {
            DBServices ds = new DBServices();
            List<Coin> coins = ds.Get_Coins_with_all_prices();
            return coins;
        }

        public Coin GetCoin()
        {
            DBServices ds = new DBServices();
            return ds.get_coin_info(this.coin_name);
        }
    }
}