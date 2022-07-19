using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Transaction
    {
        private string email;
        private string coin_name;
        private DateTime t_date;
        private float coin_amount;
        private float dollar_amount;
        private string comment;




        //for printing out
        private string username;
        private string user_pic;
        private string coin_pic;
        private string timeAgo;

        public Transaction()
        {

        }

        public Transaction(string emaill, string coin_name, DateTime date, float coin_amount, float dollar_amount, string comment)
        {
            this.Email = emaill;
            this.Coin_name = coin_name;
            T_date = date;
            this.Coin_amount = coin_amount;
            this.Dollar_amount = dollar_amount;
            this.Comment = comment;
        }

        public string Email { get => email; set => email = value; }
        public string Coin_name { get => coin_name; set => coin_name = value; }
        public DateTime T_date { get => t_date; set => t_date = value; }
        public float Coin_amount { get => coin_amount; set => coin_amount = value; }
        public float Dollar_amount { get => dollar_amount; set => dollar_amount = value; }
        public string Comment { get => comment; set => comment = value; }
        public string Username { get => username; set => username = value; }
        public string User_pic { get => user_pic; set => user_pic = value; }
        public string Coin_pic { get => coin_pic; set => coin_pic = value; }
        public string TimeAgo { get => timeAgo; set => timeAgo = value; }

        public int Insert(Transaction trans)
        {
            DBServices ds = new DBServices();
            return ds.Insert(trans);
        }

        public List<Transaction> Get_transactions_of_friends(string email)
        {
            DBServices ds = new DBServices();
            return ds.Get_transactions_of_friends(email);
        }

        public List<Transaction> Get_transactions_of_certain_user(string email)
        {
            DBServices ds = new DBServices();

            return ds.Get_transactions_of_certain_user(email);
        }

        public List<Transaction> Get_transactions_of_certain_coin(string coin_name)// without specifying who the user is following
        {
            DBServices ds = new DBServices();

            return ds.Get_transactions_of_coin(coin_name);
        }

        public List<Transaction> Get_All_trans()
        {
            DBServices ds = new DBServices();
            return ds.get_all_transactions();
        }

        public List<Transaction> Get_trans_of_certain_coin(string email, string coin)
        {
            DBServices ds = new DBServices();
            return ds.Get_transactions_of_coin(email, coin);
        }

        public int create_timeAgo_Str()
        {
            TimeSpan diff = DateTime.Now - t_date;
            if(diff.TotalSeconds < 60)
            {
                TimeAgo = String.Format("{0:0}", diff.TotalSeconds)+ " seconds ago";
                return 1;
            }
            if(diff.TotalMinutes < 60)
            {
                timeAgo = String.Format("{0:0}", diff.TotalMinutes) + " minutes ago";
                return 1;
            }
            if(diff.TotalHours < 24)
            {
                timeAgo = String.Format("{0:0}", diff.TotalHours) + " hours ago";
                return 1;
            }
            TimeAgo = String.Format("{0:0}", diff.TotalDays) + " days ago";
            return 1;
        }
    }
}