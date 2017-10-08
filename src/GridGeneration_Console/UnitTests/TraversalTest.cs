using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using MeshRecovery_Lib;
namespace UnitTests
{
    [TestClass]
    public class TraversalTest
    {
        [TestMethod]
        public void CheckTraversal()
        {
            long[] xadj = { 0, 2, 4, 6, 8 };
            int[] adjncy = { 1, 2, 0, 3, 1, 3, 0, 2 };

            Graph g = new Graph(xadj, adjncy);
            Traversal t = new Traversal(g);

            List<int> vertices1 = new List<int>();
            t.run(v => vertices1.Add(v));
            CollectionAssert.AreEqual(vertices1, new List<int> { 0, 1, 3, 2 });

            List<int> vertices2 = new List<int>();
            t.DIRECTION = Traversal.Direction.BFS;
            t.run(v => vertices2.Add(v));
            CollectionAssert.AreEqual(vertices2, new List <int>{ 0, 1, 2, 3 });
        }
    }
}
