using Newtonsoft.Json;
namespace CityDriver
{
    public class Capo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("location.x")]
        public double X { get; set; }

        [JsonProperty("location.y")]
        public double Y { get; set; }
        public Node Node { get; set; }

        public Capo(string id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public Capo()
        {
        }
    }
}
