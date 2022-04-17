using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net;

namespace BackEndCointerest.Models
{
    public class User
    {
        private string email;
        private string username;
        private string image;
        private string password;
        private string bio;
        private List<Transaction> transaction_history;

        public User()
        {

        }

        public User(string _email,string username, string image, string password, string bio)
        {
            this.email = _email;
            this.username = username;

            this.image = image;
            this.password = password;
            this.bio = bio;
        }

        public string Username { get => username; set => username = value; }
        public string Image { get => image; set => image = value; }
        public string Password { get => password; set => password = value; }
        public string Bio { get => bio; set => bio = value; }
        public string Email { get => email; set => email = value; }
        public List<Transaction> Transaction_history { get => transaction_history; set => transaction_history = value; }

        public User get_user(string email)
        {
            DBServices ds = new DBServices();
            User user = ds.Get_user(email);
            return user;
        }

        public List<User> search_users(string username_query_search)
        {
            DBServices ds = new DBServices();
            
            return ds.Search_users(username_query_search);
        }

        public int Insert()
        {
            DBServices ds = new DBServices();
           return ds.Insert(this);
        }

        public int Change_Image(string email, string pic_url)
        {
            DBServices ds = new DBServices();
            return ds.Change_image(email, pic_url);
        }


        public int  check_follow(string email, string discover_user)
        {
            DBServices ds = new DBServices();
            return ds.check_follow(email, discover_user);
        }

        public int login(string username, DateTime time)
        {
            DBServices ds = new DBServices();
            return ds.login(username, time);
        }
    }
}