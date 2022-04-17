using BackEndCointerest.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Prediction
    {
        //fields
        private string coin_name;
        private float predicted_price;
        private DateTime x_time;
        private DateTime y_time;
        private float x_current_price;
        private float y_true_price;

        public string Coin_name { get => coin_name; set => coin_name = value; }
        public float Predicted_price { get => predicted_price; set => predicted_price = value; }
        public DateTime X_time { get => x_time; set => x_time = value; }
        public DateTime Y_time { get => y_time; set => y_time = value; }
        public float X_current_price { get => x_current_price; set => x_current_price = value; }
        public float Y_true_price { get => y_true_price; set => y_true_price = value; }


        //constructors
        public Prediction(string _coin_name, float _predicted_price, DateTime _x_time, DateTime _y_time, float _x_current_price, float _y_true_price)
        {
            Coin_name = _coin_name;
            Predicted_price = _predicted_price;
            X_time = _x_time;
            Y_time = _y_time;
            X_current_price = _x_current_price;
            Y_true_price = _y_true_price;
            
        }

        public Prediction(string _coin_name, float _predicted_price, float _x_current_price)
        {
            Coin_name = _coin_name;
            Predicted_price = _predicted_price;
            X_current_price = _x_current_price;


        }

        public Prediction()
        {

        }


        //functions
        public int insert(string _coin_name, float _predicted_price, float _x_current_price)
        {
            DBServices dbs = new DBServices();
            Prediction p = new Prediction();
            p.coin_name = _coin_name;
            p.predicted_price = _predicted_price;
            p.x_time = DateTime.Now;
            p.y_time = DateTime.Now.AddHours(24);
            p.X_current_price = _x_current_price;
            return dbs.Insert(p);


        }

        public int update(Prediction p)
        {
            DBServices dbs = new DBServices();
            return dbs.Insert(p);
        }

        public List<Prediction> get(string _coin_name)
        {
            DBServices dbs = new DBServices();
            return dbs.get_predictions(_coin_name);
        }



    }
}