using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityDriver
{
    class RosonLoader
    {
        private Dictionary<string, Wall> walls;
        private Dictionary<string, Space> spaces;
        private Dictionary<string, Node> nodes;

        public void LoadRoson(string path)
        {
            walls = new Dictionary<string, Wall>();
            spaces = new Dictionary<string, Space>();
            nodes = new Dictionary<string, Node>();

            var json = System.IO.File.ReadAllText(path);

            Dictionary<String, JArray> objects = JsonConvert.DeserializeObject<Dictionary<String, JArray>>(json);
            //Console.WriteLine(objects["walls"]);
            foreach (String jObject in objects.Keys)
            {
                JArray objectsArray = objects[jObject];
                foreach (JObject obj in objectsArray)
                {
                    var type = (string)obj.GetValue("type");
                    if (type != null)
                    {
                        switch (type)
                        {
                            case "wall":
                                var id = obj.GetValue("id").Value<string>();
                                var width = obj.GetValue("width").Value<double>();
                                var height = obj.GetValue("height").Value<double>();
                                var color = obj.GetValue("color").Value<string>();

                                JObject from = (JObject)obj.GetValue("from");
                                var x1 = from.GetValue("x").Value<double>();
                                var y1 = from.GetValue("y").Value<double>();

                                JObject to = (JObject)obj.GetValue("to");
                                var x2 = (double)to.GetValue("x").Value<double>();
                                var y2 = to.GetValue("y").Value<double>();

                                Wall wall = new Wall(id, width, height, color, x1, y1, x2, y2);
                                walls.Add(id, wall);
                                break;
                            case "space":
                                var name = obj.GetValue("name").Value<string>();
                                var spaceType = obj.GetValue("type").Value<string>();

                                CityDriver.SpaceKind enumType = CityDriver.SpaceKind.Room;
                                if (spaceType.Equals("corridor"))
                                {
                                    enumType = CityDriver.SpaceKind.Corridor;
                                } else if (spaceType.Equals("room"))
                                {
                                    enumType = CityDriver.SpaceKind.Room;
                                }
                                var idS = obj.GetValue("id").Value<string>();
                                var area = obj.GetValue("area").Value<double>();
                                var diameter = obj.GetValue("diameter").Value<double>();

                                Space space = new Space(enumType, name, idS, area, diameter);
                                spaces.Add(idS, space);
                                break;
                            default:
                                break;
                        }
                    }
                    /*
                    foreach (KeyValuePair<String, JToken> app in obj)
                    {
                        var appName = obj;
                        var description = (String)app.Key;
                        var value = "";
                        if (description != "from" && description != "to" && description != "position" && description != "location")
                        {
                            value = (String)app.Value;
                        }
                        else if (description == "location")
                        {
                            value = "x: " + (String)app.Value["x"] + " y: " + (String)app.Value["y"] + " z: " + (String)app.Value["z"] + " alpha: " + (String)app.Value["alpha"];
                        }
                        else
                        {
                            value = "x: " + (String)app.Value["x"] + " y: " + (String)app.Value["y"];
                        }

                        Console.WriteLine(appName);
                        Console.WriteLine(description);
                        Console.WriteLine(value);
                        Console.WriteLine("\n");
                    }*/
                }
            }
            foreach (JObject obj in objects["nodes"])
            {
                var id = obj.GetValue("id").Value<string>();
                var kind = obj.GetValue("kind").Value<string>();
                JObject position = (JObject)obj.GetValue("position");
                var x1 = (double)position.GetValue("x").Value<double>();
                var y1 = position.GetValue("y").Value<double>();

                CityDriver.NodeKind enumType = CityDriver.NodeKind.GateNode;
                if (kind.Equals("gateNode"))
                {
                    enumType = CityDriver.NodeKind.GateNode;
                } else if (kind.Equals("spaceNode"))
                {
                    enumType = CityDriver.NodeKind.SpaceNode;
                } 
                Node node = new Node(enumType, id, x1, y1);
                nodes.Add(id, node);
            }

            foreach (JObject obj in objects["node-nodes"])
            {
                var nodeFromId = obj.GetValue("nodeFromId").Value<string>();
                Node from = nodes[nodeFromId];
                var nodeToId = obj.GetValue("nodeToId").Value<string>();
                Node to = nodes[nodeToId];
                from.Nodes.Add(to);
                to.Nodes.Add(from);
            }
            foreach (JObject obj in objects["space-walls"])
            {
                var spaceId = obj.GetValue("spaceId").Value<string>();
                var wallId = obj.GetValue("wallId").Value<string>();
                Space space = spaces[spaceId];
                Wall wall = walls[wallId];
                space.walls.Add(wall);
            }
            foreach (JObject obj in objects["space-nodes"])
            {
                var spaceId = obj.GetValue("spaceId").Value<string>();
                var nodeId = obj.GetValue("nodeId").Value<string>();
                Space space = spaces[spaceId];
                space.NodeName = nodeId;
            }
            foreach (Space space in spaces.Values)
            {
                space.generateArray();
            }
        }

        public Dictionary<String, Node> GetNodes()
        {
            return nodes;
        }

        public Dictionary<String, Space> GetSpaces()
        {
            return spaces;
        }

        public Dictionary<String, Wall> GetWalls()
        {
            return walls;
        }
    }
}
