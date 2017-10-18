using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Visualizer
{
    namespace Json
    {
        public class Node
        {
            public Node(string id, string label)
            {
                this.id = id;
                this.label = label;
            }

            public string id { set; get; }
            public string label { set; get; }
        }

        public class Edge
        {
            public Edge(string id, string target, string source)
            {
                this.id = id;
                this.target = target;
                this.source = source;
            }

            public string id { set; get; }
            public string target { set; get; }
            public string source { set; get; }
        }

        public class Graph
        {
            public Graph(List<Node> nodes, List<Edge> edges)
            {
                this.nodes = nodes;
                this.edges = edges;
            }
            public List<Node> nodes { get; set; }
            public List<Edge> edges { get; set; }
        }
    }
}
