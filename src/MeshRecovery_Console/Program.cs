using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeshRecovery_Lib;
using System.IO;
using System.Diagnostics;

namespace MeshRecovery_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] xadj = null;
            int[] adjncy = null;
            Loader.LoadGraphFromMETISFormat(@"E:\NumericalGridGeneration\tests\sources\line2.graph", out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int meshDemention;
            MeshRecovery.Validate(xadj, adjncy.Length, adjncy, out meshDemention);
            timer.Stop();
            Console.WriteLine("Validate отработала. Затрачено: " + timer.Elapsed);
            timer.Reset();
            timer.Start();
            int[][] graphNumeration;
            MeshRecovery.Numerate(xadj, adjncy.Length, adjncy, out graphNumeration);
            timer.Stop();
            Console.WriteLine("Numerate отработала. Затрачено: " + timer.Elapsed);
            Console.ReadKey();
        }
    }
}
