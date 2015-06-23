using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.roson
{
    class SpaceRobot
    {
        [JsonProperty("robotId")]
        public string RobotId { get; set; }

        [JsonProperty("spaceId")]
        public string SpaceId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
