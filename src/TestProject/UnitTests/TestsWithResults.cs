using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using MeshRecovery_Lib;
using System.Collections.Generic;

namespace TestsWithResults
{
    [TestClass]
    public class ResultsInTable  // TODO: Dinar: refactoring is neeeded
    {
        string PathToSources = @"tests/sources";
        string PathToResults = @"tests/results";
        string PathToExcelResults = @"tests/resultsExcel";
        string result_type = @".rt";
        string excel_type = @".xlsx";
        string[] head_columns = { "Name of file", "Valid", "Numerable", "Time", "Result" };
        int Padding = 20;

        void WriteRow(StreamWriter file, params string[] strings)
        {
            file.WriteLine(PrepareRow(strings));
        }
        void PrepareResultsHead(StreamWriter file)
        {
            file.WriteLine("\tTest result for Mesh Recovery functions ( " + DateTime.Now.ToLongTimeString() + " ).");
            WriteRow(file, head_columns); //, "Memory" );
        }
        string PrepareRow(params string[] strings)
        {
            string result="";
            for (int i = 0; i < strings.Length; ++i)
            {
                if (strings[i].Length > Padding) Padding = strings[i].Length;
            }

            for (int i = 0; i<strings.Length; ++i)
            {
                result += "|"+strings[i].PadLeft(Padding) +"\t";
            }
            return result+"|";
        }
        
        int GetNumerate(string file, out int[] graphNumeration)
        {
            //TODO: Dinar: implement correct version
            long [] index = { };
            int indexSize = 0;
            int[] graphData = { };
            //int[] graphNumeration = { };
            return MeshRecovery_Lib.MeshRecovery.Numerate(index,indexSize,graphData, out graphNumeration);
        }

        string IntArrayToList(int[] arr)
        {
            string result = "["+arr[0];
            for (int i = 1; i < arr.Length; ++i)
            {
                result += ","+arr[i].ToString();
            }

            return result + "]";
        }

        string GenerateNewName()
        {
            // could produce bad name bacause of system envirement
            return DateTime.Now.ToString().Replace(':', '-').Replace('.', '-').Replace(' ', '_').Replace("/", "-") + Environment.UserName;
        }

        void WriteRowExcel(Excel._Worksheet sheet, int line, params string[] str)
        {
            for (int i = 0; i < str.Length; ++i)
                sheet.Cells[line, i + 1] = str[i];
        }

        [TestMethod]
        public void BaseAlgorithmResultInInExcel()
        {
            try
            {
                var temp =  new Excel.Application();
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
            string[] files = Directory.GetFiles(PathToSources);
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
                int[] graphNumeration;
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                timer.Start();
                valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDimension);
                numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
                timer.Stop();
                //TODO: Dinar: check memory usage ? 
                WriteRowExcel(workSheet, i+2, current_file_name, valid.ToString(), numerate.ToString(), timer.ElapsedTicks.ToString(), IntArrayToList(graphNumeration));
                timer.Reset();
            }
            for (int i = 0; i < head_columns.Length; ++i)
            {
                workSheet.Columns[i+1].AutoFit();
            }
            workSheet.Name = "Results";
            workSheet.SaveAs(Environment.CurrentDirectory+"/"+PathToExcelResults +"/"+ new_file_name);
            
            // Make the object visible.
            excelApp.Visible = true;
            
        }

   
        [TestMethod]
        public void BaseAlrorithmResultInRT()
        {
            List<string> files = new List<string>( Directory.GetFiles(PathToSources));
            if (files.Count == 0) return;
            files.Sort ( Comparer<string>.Create((y,x) => x.Length-y.Length ) );
            string new_file_name = PathToResults + "/" + GenerateNewName() + result_type;
            StreamWriter result = new StreamWriter(new_file_name);
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            PrepareResultsHead(result);
            for (int i = 0; i < files.Count; ++i)
            {
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);

                int meshDimension = 0;
                long[] xadj;
                int[] adjncy;
                bool valid = false;
                int numerate = 0;
                int[] graphNumeration;
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                timer.Start();
                valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out meshDimension);
                numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);
                timer.Stop();
                //TODO: Dinar: check memory usage ? 
                //TODO: Dinar: check memory usage ? 
                WriteRow(result, current_file_name, valid.ToString(), numerate.ToString(), timer.ElapsedTicks.ToString(), IntArrayToList(graphNumeration));
                timer.Reset();
            }

            result.WriteLine("\tEND OF THE TEST RESULTS");
            result.Close();
            
        }

    }
}
