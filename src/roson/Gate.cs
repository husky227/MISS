using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class Gate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("kind")]
        public String Kind { get; set; }

        [JsonProperty("blocked")]
        public Double Blocked { get; set; }

        [JsonProperty("from")]
        public Point From { get; set; }

        [JsonProperty("to")]
        public Point To { get; set; }

        public List<Node> nodes { get; set; }

        public Gate()
        {
            nodes = new List<Node>();
        }
    }
}
