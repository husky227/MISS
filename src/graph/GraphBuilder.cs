using CityDriver.graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace CityDriver
{
    public class GraphBuilder
    {
        private Graph graph;

        public GraphBuilder(List<Node> nodesList)
        {
            graph = new Graph();
            foreach (Node node in nodesList)
            {
                Dictionary<string, int> neighbors = new Dictionary<string, int>();
                foreach (var neighbor in node.Nodes)
                {
                    if(neighbors.ContainsKey(neighbor.Id)) {
//                        Console.WriteLine("Trying to add already added key");
                    } else {
                        neighbors.Add(neighbor.Id, 1);
                    }
                }
                graph.add_vertex(node.Id, neighbors);
            }
        }

        public Graph GetGraph()
        {
            return graph;
        }
    }

}
