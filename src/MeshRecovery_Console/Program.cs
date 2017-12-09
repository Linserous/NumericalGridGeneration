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
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: MeshRecovery_Console.exe <path to .graph file> <path to save result>");
                return;
            }
            long[] xadj = null;
            int[] adjncy = null;
            Loader.LoadGraphFromMETISFormat(args[0], out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int meshDemention;
            MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDemention);
            timer.Stop();
            Console.WriteLine("Function Validate finished work. Elapsed: " + timer.Elapsed);
            timer.Reset();
            timer.Start();
            int[][] graphNumeration;
            MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
            timer.Stop();
            Console.WriteLine("Function Numerate finished work. Elapsed: " + timer.Elapsed);

            //Save graphNumeration to file
            string jsonString = null;
            try
            {
                jsonString = JsonSerializer.SerializeNumeration(graphNumeration);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can not serialize result: {e.Message}");
                return;
            }

            try
            {
                File.WriteAllText(args[1], jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can not save result to {args[1]}: {e.Message}");
            }
        }
    }
}
