using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver.graph
{
    public class Graph
    {
        private Dictionary<String, Dictionary<String, int>> vertices = new Dictionary<String, Dictionary<String, int>>();

        public void add_vertex(String name, Dictionary<String, int> edges)
        {
            vertices[name] = edges;
        }

        public List<String> shortest_path(String start, String finish)
        {
            var previous = new Dictionary<String, String>();
            var distances = new Dictionary<String, int>();
            var nodes = new List<String>();

            List<String> path = null;

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<String>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            return path;
        }
    }
}
