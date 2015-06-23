using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityDriver
{
    public enum NodeKind
    {
        GateNode,
        SpaceNode,
        WallNode
    }

    public class Node
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string StrType { get; set; }

        private string _strKind;

        [JsonProperty("kind")]
        public string StrKind { 
            get {
                return this._strKind;
            }

            set {
                _strKind = value;

                if(_strKind.Equals("gateNode"))
                {
                    this.Type = NodeKind.GateNode;
                }
                if (_strKind.Equals("spaceNode"))
                {
                    this.Type = NodeKind.SpaceNode;
                }
                if (_strKind.Equals("wallNode"))
                {
                    this.Type = NodeKind.WallNode;
                }
            }
        }

        [JsonProperty("position")]
        public Point Position { get; set; }

        public NodeKind Type { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes { get; set; }

        public Node(NodeKind type, string id, double x, double y)
        {
            this.Type = type;
            this.Id = id;
            this.Position = new Point(x, y);
            this.Nodes =  new List<Node>();
        }

        public Node()
        {
            this.Nodes = new List<Node>();
        }
    }
}
