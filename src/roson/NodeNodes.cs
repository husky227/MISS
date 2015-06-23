using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{
    class NodeNodes
    {
        [JsonProperty("nodeFromId")]
        public string NodeFromId { get; set; }

        [JsonProperty("nodeToId")]
        public string NodeToId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("cost")]
        public Double Cost { get; set; }

        [JsonProperty("blocked")]
        public Double Blocked { get; set; }
    }
}
