using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver
{
    public enum NodeKind
    {
        GateNode,
        SpaceNode
    }

    class Node
    {
        public NodeKind Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Point Position { get; set; }
        public List<Node> Nodes { get; set; }

        public Node(NodeKind type, string id, double x, double y)
        {
            this.Type = type;
            this.Id = id;
            this.Position = new Point(x, y);
            this.Nodes =  new List<Node>();
        }
    }
}
