using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Login
    {
        private string email;
        private DateTime time;

        public Login(string _email, DateTime _time)
        {
            Email = _email;
            Time = _time;
        }

        public Login() { }

        public string Email { get => email; set => email = value; }
        public DateTime Time { get => time; set => time = value; }

        public List<Login> get_logins(string email)
        {
            DBServices db = new DBServices();
            return db.get_logins(email);
        }
    }
}