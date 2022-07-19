using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Following
    {
        //fields
        private string email;
        private string username;

        public string Email { get => email; set => email = value; }
        public string Username { get => username; set => username = value; }


        //constructor
        public Following() { }

        public Following(string email, string username)
        {
            this.email = email;
            this.username = username;
        }
    }
}