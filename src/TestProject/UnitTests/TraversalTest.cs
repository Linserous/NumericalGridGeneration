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

            List<int> vertices = new List<int>();
            t.NewVertex += (sender, e) => vertices.Add(e);
            t.Run();
            CollectionAssert.AreEqual(vertices, new List<int> { 0, 1, 3, 2 });

            vertices.Clear();
            t.DIRECTION = Traversal.Direction.BFS;
            t.Run();
            CollectionAssert.AreEqual(vertices, new List <int>{ 0, 1, 2, 3 });
        }
    }
}
