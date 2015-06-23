using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{
    class SpaceNodes
    {
        [JsonProperty("spaceId")]
        public string SpaceId { get; set; }

        [JsonProperty("nodeId")]
        public string NodeId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
