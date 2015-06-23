using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class Point
    {
        [JsonProperty("x")]
        public Double X { get; set; }

        [JsonProperty("y")]
        public Double Y { get; set; }

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
