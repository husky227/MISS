using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class Wall
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("width")]
        public Double Width { get; set; }

        [JsonProperty("height")]
        public Double Height { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("from")]
        public Point From { get; set; }

        [JsonProperty("to")]
        public Point To { get; set; }

        public Wall(string id, double width, double height, string color, double x1, double y1, double x2, double y2)
        {
            this.Id = id;
            this.Width = width;
            this.Height = height;
            this.Color = color;
            this.From = new Point(x1, y1);
            this.To = new Point(x2, y2);
        }


        public Wall()
        {
        }
    }
}
