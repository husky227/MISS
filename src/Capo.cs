
namespace CityDriver
{
    public class Capo
    {
        public string Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Node Node { get; set; }

        public Capo(string id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
