using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeshRecovery_Lib;

namespace UnitTests
{
    [TestClass]
    public class TestForGraph
    {
        [TestMethod]
        public void CommonTest()
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

            CollectionAssert.AreEqual(buff, new int[]{ 1, 3, 5});

            Assert.AreEqual(buff.Length, count);
            Assert.AreEqual(graph.GetVerticesCount(), 7);
            Assert.AreEqual(graph.GetEdgeCount(), 9);
        }
        [TestMethod]
        public void ConnectTest()
        {
            long[] xadj = { 0, 2, 5, 8, 11, 13, 16, 18 };
            int[] adjncy = { 1, 3,
                0, 2, 6,
                1, 3, 5,
                0, 2, 4,
                3, 5,
                2, 4, 6,
                1, 5 };
            Graph graph = new Graph(xadj, adjncy);
            Assert.IsTrue(graph.CoherentGraphCheck());
        }
    }
}
