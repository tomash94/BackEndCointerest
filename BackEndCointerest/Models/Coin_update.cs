using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackEndCointerest.Models.DAL;

namespace BackEndCointerest.Models
{
    //                 --- Coin_update --- 
    // representing the "Coin_Updates_2022" table in the DB. 
    // is used to withdraw real market data from the CoinMarketCap API and transfer to our DB.
    // also to pull said data on the applications request. 
    public class Coin_update
    {
        //fields
        private string coin_name;
        private string symbol;
        private float coin_value;
        private DateTime update_date;
        private float percent_change_24h;
        private float percent_change_1h;
        private float percent_change_7d;
        private float percent_change_30d;
        private double volume_24h;


        //properties
        public string Symbol { get => symbol; set => symbol = value; }
        public float Coin_value { get => coin_value; set => coin_value = value; }
        public DateTime Update_date { get => update_date; set => update_date = value; }
        public string Coin_name { get => coin_name; set => coin_name = value; }
        public float Percent_change_24h { get => percent_change_24h; set => percent_change_24h = value; }
        public float Percent_change_1h { get => percent_change_1h; set => percent_change_1h = value; }
        public float Percent_change_7d { get => percent_change_7d; set => percent_change_7d = value; }
        public float Percent_change_30d { get => percent_change_30d; set => percent_change_30d = value; }
        public double Volume_24h { get => volume_24h; set => volume_24h = value; }

        //constructors
        public Coin_update()
        {

        }

        public Coin_update(string _coin_name, float _coin_value, DateTime _update_date)
        {
            Coin_name = _coin_name;
            Coin_value = _coin_value;
            Update_date = _update_date;
        }



        //methods
        public int Insert(List<Coin_update> updates)
        {
            DBServices dbs = new DBServices();

            try
            {
                dbs.Insert(updates);
            }
            catch (Exception exc)
            {
                return 0;
            }
            
            return 1;
        }

        public List<Coin_update> get(string coin_name, DateTime start, DateTime finish)
        {
            DBServices dbs = new DBServices();
            return dbs.Get_price_history_per_coin_name(coin_name, start, finish);
        }

    }
}