using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MeshRecovery_Visualizer
{
    namespace Json
    {
        class Node
        {
            public Node(string id, string label)
            {
                this.id = id;
                this.label = label;
            }

            public string id { set; get; }
            public string label { set; get; }
        }

        class Edge
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

        class Graph
        {
            public Graph(List<Node> nodes, List<Edge> edges)
            {
                this.nodes = nodes;
                this.edges = edges;
            }
            public List<Node> nodes { get; set; }
            public List<Edge> edges { get; set; }
        }

        public class Graph2Json
        {
            static public string Run(long[] xadj, int[] adjncy)
            {
                var nodes = new List<Json.Node>();
                for (var i = 0; i < xadj.Length - 1; ++i)
                {
                    nodes.Add(new Json.Node(Convert.ToString(i), "v_" + Convert.ToString(i)));
                }
                var edges = new List<Json.Edge>();
                for (var i = 0; i < xadj.Length - 1; ++i)
                {
                    for (long j = xadj[i]; j < xadj[i + 1]; ++j)
                    {
                        edges.Add(new Json.Edge(Convert.ToString(i + j), Convert.ToString(i), Convert.ToString(adjncy[j])));
                    }
                }
                var graph = new Json.Graph(nodes, edges);
                return new JavaScriptSerializer().Serialize(graph);
            }
        }
    }
}
