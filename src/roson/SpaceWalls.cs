using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{

    class SpaceWalls
    {
        [JsonProperty("spaceId")]
        public string SpaceId { get; set; }

        [JsonProperty("wallId")]
        public string WallId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
