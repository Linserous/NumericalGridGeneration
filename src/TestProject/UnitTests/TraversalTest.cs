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
            long[] xadj = { 0, 3, 6, 9, 12, 15, 18, 21, 24 };
            int[] adjncy = { 1, 7, 3,
                            0, 2, 6,
                            1, 5, 3,
                            2, 4, 0,
                            5, 3, 7,
                            4, 2, 6,
                            7, 1, 5,
                            0, 4, 6 };
            Graph g = new Graph(xadj, adjncy);

            List<int> vertices = new List<int>();

            var tInDepth = new Traversal<DFS>(g);
            // Subscribe to the NewVertex event to get the path to the graph
            tInDepth.NewVertex += (sender, e) => vertices.Add(e);
            tInDepth.Run();
            CollectionAssert.AreEqual(vertices, new List<int> { 0, 1, 7, 3, 2, 4, 5, 6 });

            vertices.Clear();
            // Traversal<BFS>(g) is equvalent to the  Traversal(g)
            var tInBreadth = new Traversal<BFS>(g);
            tInBreadth.NewVertex += (sender, e) => vertices.Add(e);
            tInBreadth.Run();
            CollectionAssert.AreEqual(vertices, new List<int> { 0, 1, 7, 3, 2, 6, 4, 5 });
        }
    }
}
