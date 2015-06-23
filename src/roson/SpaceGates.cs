using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{
    class SpaceGates
    {
        [JsonProperty("gateId")]
        public string GateId { get; set; }

        [JsonProperty("spaceId")]
        public string SpaceId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("passable")]
        public Boolean Passable { get; set; }
    }
}
