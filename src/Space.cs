using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    class Space
    {
        public enum Kind
        {
            Room,
            Corridor
        }

        public Kind Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Double Area { get; set; }
        public Double Diameter { get; set; }
    }
}
