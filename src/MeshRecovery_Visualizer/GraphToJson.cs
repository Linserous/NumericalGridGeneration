using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MeshRecovery_Visualizer
{
    public class GraphToJson
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
