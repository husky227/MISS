using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public enum SpaceKind
    {
        Room,
        Corridor
    }

    class Space
    {

        public SpaceKind Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Double Area { get; set; }
        public Double Diameter { get; set; }
        public List<Wall> walls { get; set; }

        public Space(SpaceKind type, string name, string id, double area, double diameter)
        {
            this.Type = type;
            this.Name = name;
            this.Id = id;
            this.Area = area;
            this.Diameter = diameter;
            walls = new List<Wall>();
        }
    }
}
