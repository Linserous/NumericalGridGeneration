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
            string sourceFile = null;
            string outputPath = null;

            ArgParser parser = new ArgParser();

            parser.AddArgument("i|input=", "Path to the graph file", v => sourceFile = v);
            parser.AddArgument("o|out=", "Path to the output file (.json)", v => outputPath = v);

            if (!parser.ParseArguments(args))
                return;

            if (sourceFile == null)
            {
                Console.WriteLine("Please specify the path to graph file");
                return;
            }
            else if (!File.Exists(sourceFile))
            {
                Console.WriteLine($"File is not exist: {sourceFile}");
                return;
            }

            if (outputPath == null)
            {
                outputPath = Path.Combine(new FileInfo(sourceFile).Directory.FullName,
                    Path.GetFileNameWithoutExtension(sourceFile) + ".json");
            }
            else if (Path.GetExtension(outputPath) != ".json")
            {
                Console.WriteLine("Output file must have .json extension");
                return;
            }

            long[] xadj = null;
            int[] adjncy = null;
            Loader.LoadGraphFromMETISFormat(sourceFile, out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            int meshDemention;
            MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDemention);
            timer.Stop();
            Console.WriteLine("Function Validate finished work. Elapsed: " + timer.ElapsedMilliseconds);
            timer.Reset();
            timer.Start();
            int[][] graphNumeration;
            MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
            timer.Stop();
            Console.WriteLine("Function Numerate finished work. Elapsed: " + timer.ElapsedMilliseconds);

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
                File.WriteAllText(outputPath, jsonString);
                Console.WriteLine($"Result is saved into file: {outputPath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can not save result to {args[1]}: {e.Message}");
            }
        }
    }
}
