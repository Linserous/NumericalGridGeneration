using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GridGeneration_Lib;

namespace UnitTests
{
    [TestClass]
    public class TestForGraph
    {
        [TestMethod]
        public bool TestMethod1()
        {
            long[] xadj = {0, 2, 4, 7, 10,12,15,18};
            int[] adjncy = { 1, 3, 0, 2, 1, 3, 5, 0, 2, 4, 3, 5, 2, 4, 6, 1, 5 };
            Graph graph = new Graph(xadj, adjncy);
            graph.GetAdjVertices(2, out int[] buff);
            
            long count = graph.GetAdjVerticesCount(2);
            bool result = (buff.Length == count) && (graph.GetVerticesCount() == 7 ) && (graph.GetEdgeCount() == 9);
            return result;
        }
    }
}
