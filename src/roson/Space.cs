using Newtonsoft.Json;
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

    public class Space
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        private string _strKind;

        [JsonProperty("type")]
        public string StrKind
        {
            get
            {
                return this._strKind;
            }

            set
            {
                _strKind = value;

                if (_strKind.Equals("room"))
                {
                    this.Type = SpaceKind.Room;
                }
                if (_strKind.Equals("corridor"))
                {
                    this.Type = SpaceKind.Corridor;
                }
            }
        }

        [JsonProperty("kind")]
        public String Kind { get; set; }

        [JsonProperty("function")]
        public Object Function { get; set; }

        [JsonProperty("expectedPersonCount")]
        public Int32 ExpectedPersonCount { get; set; }

        [JsonProperty("area")]
        public Double Area { get; set; }

        [JsonProperty("diameter")]
        public Double Diameter { get; set; }

        public SpaceKind Type { get; set; }
        public string Name { get; set; }
        public string NodeName { get; set; }
        public List<Wall> walls { get; set; }
        public List<Gate> gates { get; set; }
        public List<Node> nodes { get; set; }
        public Wall[] wallsArray;

        public Space(SpaceKind type, string name, string id, double area, double diameter)
        {
            this.Type = type;
            this.Name = name;
            this.Id = id;
            this.Area = area;
            this.Diameter = diameter;
            walls = new List<Wall>();
            gates = new List<Gate>();
            nodes = new List<Node>();
        }

        public Space()
        {
            walls = new List<Wall>();
            gates = new List<Gate>();
            nodes = new List<Node>();
        }

        public void generateArray()
        {
            wallsArray = walls.ToArray();
        }

        public Boolean isInside(double x, double y)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = wallsArray.Length - 1;
            double total_angle = GetAngle(
                wallsArray[max_point].From.X, wallsArray[max_point].From.Y,
                x, y,
                wallsArray[0].From.X, wallsArray[0].From.Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    wallsArray[i].From.X, wallsArray[i].From.Y,
                    x, y,
                    wallsArray[i+1].From.X, wallsArray[i+1].From.Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static double GetAngle(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (double)Math.Atan2(cross_product, dot_product);
        }

        private static double DotProduct(double Ax, double Ay, double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static double CrossProductLength(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }
    }
}
