using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Tweet
    {
        //fields
        private string tweet_id;
        private string author;
        private float comp_score;
        private int followers;
        private int engagement;
        private string tweet_text;
        private string link;
        private DateTime tweet_time;

        public string Tweet_id { get => tweet_id; set => tweet_id = value; }
        public string Author { get => author; set => author = value; }
        public float Comp_score { get => comp_score; set => comp_score = value; }
        public int Followers { get => followers; set => followers = value; }
        public int Engagement { get => engagement; set => engagement = value; }
        public string Tweet_text { get => tweet_text; set => tweet_text = value; }
        public string Link { get => link; set => link = value; }
        public DateTime Tweet_time { get => tweet_time; set => tweet_time = value; }


        //constructor 
        public Tweet() { }






        //functions
        public int insert(List<Tweet> tweets)
        {
            DBServices dbs = new DBServices();
            foreach(Tweet t in tweets)
            {
                
                try
                {
                    dbs.Insert(t);
                }
                catch(Exception ex)
                {
                    return 0;
                }
            }
            return 1;
        }

        public List<Tweet> get()
        {
            DBServices dbs = new DBServices();
            return dbs.get_tweets();
        }

    }
}