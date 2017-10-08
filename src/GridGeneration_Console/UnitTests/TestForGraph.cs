using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GridGeneration_Lib;

namespace UnitTests
{
    [TestClass]
    public class TestForGraph
    {
        [TestMethod]
        public void TestMethod1()
        {
            long[] xadj = {0, 2, 5, 8, 11,13,16,18};
            int[] adjncy = { 1, 3,
                0, 2, 6,
                1, 3, 5,
                0, 2, 4,
                3, 5,
                2, 4, 6,
                1, 5 };
            Graph graph = new Graph(xadj, adjncy);
            graph.GetAdjVertices(2, out int[] buff);
            
            long count = graph.GetAdjVerticesCount(2);
            bool result = (buff.Length == count);
            Assert.IsTrue(result);
            Assert.IsTrue(graph.GetVerticesCount() == 7);
            Assert.IsTrue(graph.GetEdgeCount() == 9);
        }
    }
}
