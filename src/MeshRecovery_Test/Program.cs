using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using MeshRecovery_Lib;
using System.IO;

namespace MeshRecovery_Test
{
    class Program
    {
        static string PathToTests = @"tests";
        static string PathToResults = @"tests/results";
        static string PathToExcelResults = @"tests/resultsExcel";
        static string result_type = @".txt";
        static string excel_type = @".xlsx";
        static string testSourcePattern = @"sources";
        static string[] head_columns = {
            "Name of file"
                , "Vertices Count"
                , "Edges Count"
                , "Validate Return Code"
                , "Numerate Return Code"
                , "Numeration Validation Status"
                , "Dimension"
                , "Time (ms)"
        };
        static int Padding = 20;

        static void WriteRow(StreamWriter file, params string[] strings)
        {
            file.WriteLine(PrepareRow(strings));
        }
        static void PrepareResultsHead(System.IO.StreamWriter file, string[] strings)
        {
            for (int i = 0; i < strings.Length; ++i)
            {
                if (strings[i].Length > Padding) Padding = strings[i].Length;
            }
            file.WriteLine("\tTest result for Mesh Recovery functions ( " + DateTime.Now.ToLongTimeString() + " ).");
            WriteRow(file, head_columns); //, "Memory" );
        }
        static string PrepareRow(params string[] strings)
        {
            string result = "";
            for (int i = 0; i < strings.Length; ++i)
            {
                result += "|" + strings[i].PadLeft(Padding) + "\t";
            }
            return result + "|";
        }

        static int GetNumerate(string file, out int[][] graphNumeration)
        {
            //TODO: Dinar: implement correct version
            long[] index = { };
            int indexSize = 0;
            int[] graphData = { };
            //int[] graphNumeration = { };
            return MeshRecovery_Lib.MeshRecovery.Numerate(index, indexSize, graphData, out graphNumeration);
        }

        static string IntArrayToList(int[][] arr)
        {
            if (arr == null) return "Fail";

            string result = "";

            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i] == null) return "Fail";
                result += "[";
                for (int j = 0; j < arr[i].Length; ++j)
                {
                    result += arr[i][j].ToString();
                    if ((j > 0) && (j < arr[i].Length - 1)) result += ",";
                }
                result += "]";
            }
            return result;
        }

        static string GenerateNewName()
        {
            // could produce bad name bacause of system envirement
            return DateTime.Now.ToString().Replace(':', '-').Replace('.', '-').Replace(' ', '_').Replace("/", "-") + Environment.UserName;
        }

        static void WriteRowExcel(Excel._Worksheet sheet, int line, params string[] str)
        {
            for (int i = 0; i < str.Length; ++i)
                sheet.Cells[line, i + 1] = str[i];
        }
        static void PrintResultInExcel()
        {
            try
            {
                var temp = new Excel.Application();
                //don`t forget closing that test app 
                temp.Quit();
            }
            catch
            {
                return;
            }

            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = excelApp.ActiveSheet;

            WriteRowExcel(workSheet, 1, head_columns);
            //workSheet.Cells[1, 6] = "Memory";
            string[] files = GetAllTestFiles().ToArray();
            if (files.Length == 0) return;

            string new_file_name = GenerateNewName() + excel_type;

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

            for (int i = 0; i < files.Length; ++i)
            {
                int meshDimension = 0;
                long[] xadj;
                int[] adjncy;
                bool valid = false;
                int numerate = 0;
                int[][] graphNumeration;
                string current_file_name = files[i].Split('\\').Last<string>();
                Console.WriteLine("Working on " + current_file_name);
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                timer.Start();
                valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDimension);
                numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
                timer.Stop();
                //TODO: Dinar: check memory usage ? 
                Graph graph = new Graph(xadj, adjncy);
                bool success = valid && numerate == 0;
                string validationResult = "";
                if (success)
                {
                    int validationCode = NumerationHelper.ValidateNumeration(xadj, adjncy, meshDimension, graphNumeration);
                    validationResult = validationCode >= 0 ? "Верно" : "Неверно";
                }
                WriteRowExcel(
                    workSheet,
                    i + 2,
                    current_file_name,
                    graph.GetVerticesCount().ToString(),
                    graph.GetEdgeCount().ToString(),
                    valid.ToString(),
                    numerate.ToString(),
                    validationResult,
                    success ? graphNumeration[0].Length.ToString() : "X",
                    timer.Elapsed.Milliseconds.ToString()
                    );

                timer.Reset();
            }
            for (int i = 0; i < head_columns.Length; ++i)
            {
                workSheet.Columns[i + 1].AutoFit();
            }
            workSheet.Name = "Results";
            workSheet.SaveAs(Environment.CurrentDirectory + "/" + PathToExcelResults + "/" + new_file_name);

            // Make the object visible.
            excelApp.Visible = true;
        }

        static void PrintResultInRT()
        {
            List<string> files = GetAllTestFiles();
            if (files.Count == 0) return;
            files.Sort(Comparer<string>.Create((y, x) => x.Split('\\').Last<string>().Length - y.Split('\\').Last<string>().Length));
            string new_file_name = PathToResults + "/" + GenerateNewName() + result_type;
            StreamWriter result = new StreamWriter(new_file_name);
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            PrepareResultsHead(result, files.ToArray());
            for (int i = 0; i < files.Count; ++i)
            {
                string current_file_name = files[i].Split('\\').Last<string>();
                Console.WriteLine("Working on " + current_file_name);
                int meshDimension = 0;
                long[] xadj;
                int[] adjncy;
                bool valid = false;
                int numerate = 0;
                int[][] graphNumeration;
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                timer.Start();
                valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDimension);
                numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
                timer.Stop();
                string validationResult = "";
                if (valid && numerate == 0)
                {
                    int validationCode = NumerationHelper.ValidateNumeration(xadj, adjncy, meshDimension, graphNumeration);
                    validationResult = validationCode >= 0 ? "Верно" : "Неверно";
                }
                //TODO: Dinar: check memory usage ? 
                //TODO: Dinar: check memory usage ? 
                WriteRow(result
                    , current_file_name
                    , valid.ToString()
                    , numerate.ToString()
                    , timer.Elapsed.Milliseconds.ToString()
                    , IntArrayToList(graphNumeration)
                    , validationResult);
                timer.Reset();
            }

            result.WriteLine("\tEND OF THE TEST RESULTS");
            result.Close();

        }
        static List<string> GetAllTestFiles()
        {
            List<string> files = new List<string>();

            List<string> dirs = new List<string>(Directory.GetDirectories(PathToTests, testSourcePattern));

            foreach (string d in dirs)
            {
                foreach (string f in Directory.GetFiles(d, "*.graph", SearchOption.AllDirectories))
                    files.Add(f);
            }
            return files;
        }
        static void PrintHelp()
        {
            Console.WriteLine("This program will prepare result for all"
                              + "graph files in /test/sources*/ and save them into /tests/results*/ ");

            Console.WriteLine("Possible arguments:");
            Console.WriteLine("\t'without arguments' - will prepare excel and ttxt files");
            Console.WriteLine("\texcel - will prepare only excel file");
            Console.WriteLine("\ttext - will prepare only text file");
        }
        static void PrintWrongArg()
        {
            Console.WriteLine("Wrong argument! Use 'help' for information. ");
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintResultInRT();
                PrintResultInExcel();
            }
            else
            {
                switch (args[0])
                {
                    case "excel": PrintResultInExcel(); break;
                    case "text": PrintResultInRT(); break;
                    case "help": PrintHelp(); break;
                    default: PrintWrongArg(); break;
                }
            }

        }
    }
}
