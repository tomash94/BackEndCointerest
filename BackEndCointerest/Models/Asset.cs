using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Asset
    {
        private string coin_name;
        private string email;
        private float amount;
        private Coin coin_info;
        private float asset_worth_in_USD;

        public Asset()
        {

        }

        public Asset(string coin_name, string email, float amount)
        {
            this.Coin_name = coin_name;
            this.Email = email;
            this.Amount = amount;
        }

        public string Coin_name { get => coin_name; set => coin_name = value; }
        public string Email { get => email; set => email = value; }
        public float Amount { get => amount; set => amount = value; }
        public Coin Coin_info { get => coin_info; set => coin_info = value; }
        public float Asset_worth_in_USD { get => asset_worth_in_USD; set => asset_worth_in_USD = value; }

        public List<Asset> get_assets_of_certain_user(string email)
        {
            DBServices ds = new DBServices();
            List<Asset> asset_list = ds.get_assets_of_certain_user(email);
            
            return asset_list;
        }

        public int Insert(Asset asset)
        {
            DBServices ds = new DBServices();
            return ds.Insert(asset);
        }

    }
}