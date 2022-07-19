using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models.DAL
{
    public class Balance
    {
        //fields
        float net_worth;
        float weekly_change_percent;

        //constructor
        public Balance()
        {

        }

        //properties
       
        public float Net_worth { get => net_worth; set => net_worth = value; }
        public float Weekly_change_percent { get => weekly_change_percent; set => weekly_change_percent = value; }

        //methods
        public Balance get(string email)
        {
            DBServices ds = new DBServices();
            return ds.get_portfolio_worth(email);
        }

        

    }
}