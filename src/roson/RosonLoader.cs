using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CityDriver.roson;

namespace CityDriver
{
    public class RosonLoader
    {
        private Dictionary<string, Wall> walls;
        private Dictionary<string, Space> spaces;
        private Dictionary<string, Node> nodes;
        private Dictionary<string, Gate> gates;
        public Dictionary<string, Capo> robots { get; set; }
        private List<double> boundaries;

        public void LoadRoson(string path)
        {
            walls = new Dictionary<string, Wall>();
            spaces = new Dictionary<string, Space>();
            nodes = new Dictionary<string, Node>();
            gates = new Dictionary<string, Gate>();
            robots = new Dictionary<string, Capo>();

            var json = System.IO.File.ReadAllText(path);

            Dictionary<String, JArray> objects = JsonConvert.DeserializeObject<Dictionary<String, JArray>>(json);
            foreach (JObject obj in objects["walls"])
            {
                Wall wall = obj.ToObject<Wall>();
                walls.Add(wall.Id, wall);
            }
            foreach (JObject obj in objects["gates"])
            {
                Gate gate = obj.ToObject<Gate>();
                gates.Add(gate.Id, gate);
            }
            foreach (JObject obj in objects["spaces"])
            {
                Space space = obj.ToObject<Space>();
                spaces.Add(space.Id, space);
            }
            foreach (JObject obj in objects["nodes"])
            {
                Node node = obj.ToObject<Node>();
                nodes.Add(node.Id, node);
            }
            foreach (JObject obj in objects["robots"])
            {
                Capo robot = obj.ToObject<Capo>();
                robots.Add(robot.Id, robot);
            }
            foreach (JObject obj in objects["space-walls"])
            {
                SpaceWalls spaceWalls = obj.ToObject<SpaceWalls>();
                Space space;
                if (spaces.TryGetValue(spaceWalls.SpaceId, out space))
                {
                    Wall wall;
                    if (walls.TryGetValue(spaceWalls.WallId, out wall))
                    {
                        space.walls.Add(wall);
                    }
                }
            }
            foreach (JObject obj in objects["space-gates"])
            {
                SpaceGates spaceGates = obj.ToObject<SpaceGates>();
                Space space;
                if (spaces.TryGetValue(spaceGates.SpaceId, out space))
                {
                    Gate gate;
                    if (gates.TryGetValue(spaceGates.GateId, out gate))
                    {
                        space.gates.Add(gate);
                    }
                }
            }
            foreach (JObject obj in objects["space-nodes"])
            {
                SpaceNodes spaceNodes = obj.ToObject<SpaceNodes>();

                Space space;
                if (spaces.TryGetValue(spaceNodes.SpaceId, out space))
                {
                    Node node;
                    if (nodes.TryGetValue(spaceNodes.NodeId, out node))
                    {
                        space.NodeName = node.Id;
                    }
                }
            }
            foreach (JObject obj in objects["gate-nodes"])
            {
                GateNodes gateNodes = obj.ToObject<GateNodes>();

                Gate gate;
                if (gates.TryGetValue(gateNodes.GateId, out gate))
                {
                    Node node;
                    if (nodes.TryGetValue(gateNodes.NodeId, out node))
                    {
                        gate.nodes.Add(node);
                    }
                }
            }
            foreach (JObject obj in objects["node-nodes"])
            {
                NodeNodes nodeNodes = obj.ToObject<NodeNodes>();

                Node node;
                if (nodes.TryGetValue(nodeNodes.NodeFromId, out node))
                {
                    Node nodeOut;
                    if (nodes.TryGetValue(nodeNodes.NodeToId, out nodeOut))
                    {
                        node.Nodes.Add(nodeOut);
                    }
                }
            }
            foreach (JObject obj in objects["space-robots"])
            {
                SpaceRobot spaceRobot = obj.ToObject<SpaceRobot>();
                Space space;
                if (spaces.TryGetValue(spaceRobot.SpaceId, out space))
                {
                    Capo robot;
                    if (robots.TryGetValue(spaceRobot.RobotId, out robot))
                    {
                        robot.Node = nodes[space.NodeName];
                    }
                }
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
