using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Graph_value_interval : Value_interval
    {
        private double comp;


        public Graph_value_interval(string _name, double _value, double _comp) : base(_name, _value)
        {
            Comp = _comp;
        }

        public Graph_value_interval() { }
        public double Comp { get => comp; set => comp = value; }
    }
}