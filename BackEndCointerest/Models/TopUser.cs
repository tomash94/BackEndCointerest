using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class TopUser : User
    {
        //fields 
        Balance balance;

        //properties
        public Balance Balance { get => balance; set => balance = value; }

        //constructors
        public TopUser() : base()
        {
            balance = new Balance();
        }
    }
}