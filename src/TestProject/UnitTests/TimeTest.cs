using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeshRecovery_Lib;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void CheckWorkTime()
        {
            long[] xadj;
            int[] adjncy;
            int[][] graphNumeration;
            Loader.LoadGraphFromMETISFormat(@"tests\big_graphs\grid_150x200.graph", out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out int meshDimension);
            timer.Stop();
            Assert.IsTrue(valid, "Test graph is not valid");
            Assert.IsTrue(timer.Elapsed.Seconds <= 5, "Validate takes more than 5 seconds.");
            timer.Reset();
            timer.Start();
            int numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
            timer.Stop();
            Assert.AreEqual(0, numerate, "Test graph can not be numerated");
            Assert.IsTrue(timer.Elapsed.Minutes <= 5, "Numerate takes more than 5 minutes.");
        }
    }
}
