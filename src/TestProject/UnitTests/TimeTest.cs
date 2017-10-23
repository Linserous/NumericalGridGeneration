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
            Loader.LoadGraphFromMETISFormat(@"tests\sources_big_graphs\grid_100x150x80.graph", out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out int meshDimension);
            timer.Stop();
            if (timer.Elapsed.Seconds > 5) Assert.Fail("Validate more than 5 sec");
            timer.Reset();
            timer.Start();
            int numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
            timer.Stop();
            if (timer.Elapsed.Minutes > 5) Assert.Fail("Numerate more than 5 min");
        }
    }
}
