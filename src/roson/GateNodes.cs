using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{
    class GateNodes
    {
        [JsonProperty("nodeId")]
        public string NodeId { get; set; }

        [JsonProperty("gateId")]
        public string GateId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
