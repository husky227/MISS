using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class Wall
    {
        public string Id { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }
        public string Color { get; set; }
        public Point From { get; set; }
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
    }
}
