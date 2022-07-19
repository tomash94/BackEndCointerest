using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackEndCointerest.Models
{
    public class Value_interval
    {
        //fields
        private string name;
        private double value;


        public string Name { get => name; set => name = value; }
        public double Value { get => value; set => this.value = value; }


        public Value_interval(string _name, double _value) {
            Name = _name;
            Value = _value;


        }

        public Value_interval() { }


    }
}