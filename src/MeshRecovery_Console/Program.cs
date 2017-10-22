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
            if (args.Length == 0)
            {
                Console.WriteLine("No path to source file");
                return;
            }

            long[] xadj = null;
            int[] adjncy = null;
            Loader.LoadGraphFromMETISFormat(args[0], out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int meshDemention;
            MeshRecovery.Validate(xadj, adjncy.Length, adjncy, out meshDemention);
            timer.Stop();
            Console.WriteLine("Function Validate finished work. Elapsed: " + timer.Elapsed);
            timer.Reset();
            timer.Start();
            int[][] graphNumeration;
            MeshRecovery.Numerate(xadj, adjncy.Length, adjncy, out graphNumeration);
            timer.Stop();
            Console.WriteLine("Function Numerate finished work. Elapsed: " + timer.Elapsed);
            //Save graphNumeration to file (discuss format)
        }
    }
}
