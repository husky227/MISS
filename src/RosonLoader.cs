﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityDriver
{
    public class RosonLoader
    {
        private Dictionary<string, Wall> walls;
        private Dictionary<string, Space> spaces;
        private Dictionary<string, Node> nodes;
        public Dictionary<string, Capo> robots { get; set; }
        private List<double> boundaries;

        public void LoadRoson(string path)
        {
            walls = new Dictionary<string, Wall>();
            spaces = new Dictionary<string, Space>();
            nodes = new Dictionary<string, Node>();
            robots = new Dictionary<string, Capo>();

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
                                }
                                else if (spaceType.Equals("room"))
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
                }
                else if (kind.Equals("spaceNode"))
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

            foreach (JObject obj in objects["robots"])
            {
                var id = obj.GetValue("id").Value<string>();
                JObject location = (JObject)obj.GetValue("location");
                var x = location.GetValue("x").Value<double>();
                var y = location.GetValue("y").Value<double>();

                Capo robot = new Capo(id, x, y);
                robots.Add(id, robot);
            }

            foreach (JObject obj in objects["space-robots"])
            {
                var spaceId = obj.GetValue("spaceId").Value<string>();
                var robotId = obj.GetValue("robotId").Value<string>();
                Node node = nodes[spaces[spaceId].NodeName];
                robots[robotId].Node = node;
                ;
            }
            foreach (Space space in spaces.Values)
            {
                space.generateArray();
            }
            createBoundaries();
        }

        public void createBoundaries()
        {
            double max_y = 0, max_x = 0, min_y = 0, min_x = 0;
            bool first = true;
            foreach (Wall wall in walls.Values)
            {
                if (first)
                {
                    max_x = Math.Max(wall.From.X, wall.To.X);
                    min_x = Math.Min(wall.From.X, wall.To.X);
                    max_y = Math.Max(wall.From.Y, wall.To.Y);
                    min_y = Math.Min(wall.From.Y, wall.To.Y);
                    first = false;
                }
                else
                {
                    if (max_x < wall.From.X)
                    {
                        max_x = wall.From.X;
                    }
                    else if (max_x < wall.To.X)
                    {
                        max_x = wall.To.X;
                    }
                    else if (min_x > wall.From.X)
                    {
                        min_x = wall.From.X;
                    }
                    else if (min_x > wall.To.X)
                    {
                        min_x = wall.To.X;
                    }

                    if (max_y < wall.From.Y)
                    {
                        max_y = wall.From.Y;
                    }
                    else if (max_y < wall.To.Y)
                    {
                        max_y = wall.To.Y;
                    }
                    else if (min_y > wall.From.Y)
                    {
                        min_y = wall.From.Y;
                    }
                    else if (min_y > wall.To.Y)
                    {
                        min_y = wall.To.Y;
                    }
                }
            }

            List<double> result = new List<double>();
            result.Add(min_x + 0.3);
            result.Add(min_y + 0.3);
            result.Add(max_x - 0.3);
            result.Add(max_y - 0.3);
            boundaries = result;
        }

        public Dictionary<String, Node> GetNodes()
        {
            return nodes;
        }

        public Dictionary<String, Space> GetSpaces()
        {
            return spaces;
        }

        public Point GetRandomPoint()
        {
            Random random = new Random();
            double x = random.NextDouble() * (boundaries[2] - boundaries[0]) + boundaries[0];
            double y = random.NextDouble() * (boundaries[3] - boundaries[1]) + boundaries[1];
            return new Point(x, y);
        }

        public Dictionary<String, Wall> GetWalls()
        {
            return walls;
        }

        public List<double> GetBoundaries()
        {
            return boundaries;
        }
    }
}
