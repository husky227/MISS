using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver
{
    class Collision
    {
        public int Id1 { get; set; }
        public int Id2 { get; set; }
        public int Time { get; set; }

        public Collision(int id1, int id2, int time)
        {
            this.Id1 = id1;
            this.Id2 = id2;
            this.Time = time;
        }
    }
}
