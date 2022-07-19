using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Wallet_worth
    {
        //fields
        string email;
        DateTime time;
        float worth;

        //constructors
        public Wallet_worth()
        {

        }

        //properties
        public string Email { get => email; set => email = value; }
        public DateTime Time { get => time; set => time = value; }
        public float Worth { get => worth; set => worth = value; }

        //methods
        public int insert()
        {
            DBServices ds = new DBServices();
            List<Wallet_worth> w_list = ds.get_current_wallet_worth();

            return ds.Insert(w_list);
        }

        public int insert_new_user(string email)
        {
            Wallet_worth item = new Wallet_worth();
            item.Email = email;
            item.Time = DateTime.Now;
            item.Worth = 100000;

            DBServices ds = new DBServices();
            return ds.Insert(item);
        }
    }
}