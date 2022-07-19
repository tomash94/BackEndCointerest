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
        //fields
        private string email;
        private string username;
        private string image;
        private string password;
        private string bio;
        private List<Transaction> transaction_history;

        //constructors
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


        //get-set methods
        public string Username { get => username; set => username = value; }
        public string Image { get => image; set => image = value; }
        public string Password { get => password; set => password = value; }
        public string Bio { get => bio; set => bio = value; }
        public string Email { get => email; set => email = value; }
        public List<Transaction> Transaction_history { get => transaction_history; set => transaction_history = value; }

        //functions
        public User get_user(string email)
        {
            DBServices ds = new DBServices();
            User user = ds.Get_user(email);
            return user;
        }
public List<User> get_users()
        {
            DBServices ds = new DBServices();
            return ds.get_all_users();
        }


        public List<User> search_users(string username_query_search)
        {
            DBServices ds = new DBServices();
            
            

            if (!check_substring(username_query_search))
            {
                return ds.Search_users(username_query_search, 0);
            }
            else
            {
                return ds.Search_users(username_query_search, 1);
            }

        }

        public List<TopUser> top_users()
        {
            DBServices ds = new DBServices();
            return ds.top_5_earners(DateTime.Now.AddDays(-7));
        }

        public int Insert()
        {
            DBServices ds = new DBServices();
           return ds.Insert(this);
        }

        public int Delete(object obj)
        {
            DBServices ds = new DBServices();
            return ds.Delete(obj);
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

        public int post_follow(Following f)
        {
            DBServices ds = new DBServices();
            return ds.Insert(f);
        }

        public int login(string username, DateTime time)
        {
            DBServices ds = new DBServices();
            Login log = new Login(username, time);
            return log.insert();
        }

        public int update_bio(string email, string bio)
        {
            DBServices ds = new DBServices();
            return ds.update_bio(email, bio);
        }

        private bool check_substring(string substring)
        {
            //check if a given string is equal to any of the coins in the app market.
            
            List<Coin> c_list = new List<Coin>();
            DBServices ds = new DBServices();

            c_list = ds.Get_Coins_with_latest_price_only();

            foreach(Coin item in c_list)
            {
                if (substring.ToLower() == item.Coin_name.ToLower()) return true;
            }
            return false;
        }
    }
}