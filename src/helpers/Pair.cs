using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver
{
    public class Pair
    {
        public Pair()
        {
        }

        public Pair(Point first, Double second)
        {
            this.First = first;
            this.Second = second;
        }

        public Point First { get; set; }
        public double Second { get; set; }

        public int CompareTo(Pair other)
        {
            if (this.Second < other.Second)
            {
                return -1;
            }
            else if (this.Second == other.Second)
            {
                return 0;
            }
            return 1;
        }
    };

}
