using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;

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
        int Padding = 25;

        void WriteRow(StreamWriter file, params string[] strings)
        {
            file.WriteLine(PrepareRow(strings));
        }
        void PrepareResultsHead(StreamWriter file)
        {
            file.WriteLine("\tTest result fot Graph Restorer functions ( " + DateTime.Now.ToLongTimeString() + " ).");
            WriteRow(file, "Name of file", "Valid", "Numerable", "Time", "Result"); //, "Memory" );
        }
        string PrepareRow(params string[] strings)
        {
            string result="";
            for (int i = 0; i<strings.Length; ++i)
            {
                result += "|"+strings[i].PadLeft(Padding) +"\t";
            }
            return result+"|";
        }
        
        bool GetValid(string file)
        {
            //TODO: Dinar: implement correct version
            int []  index = { };
            int indexSize = 0;
            int[] graphData = { };
            return GridGeneration_Lib.GraphRestorer.Validate(index, indexSize, graphData, out int gridDimension);
        }
        int GetNumerate(string file, out int[] graphNumeration)
        {
            //TODO: Dinar: implement correct version
            int[] index = { };
            int indexSize = 0;
            int[] graphData = { };
            //int[] graphNumeration = { };
            return GridGeneration_Lib.GraphRestorer.Numerate(index,indexSize,graphData, out graphNumeration);
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
        public ResultsInTable()
        {
            //Directory.CreateDirectory(PathToSources);
            Directory.CreateDirectory(PathToResults);
            Directory.CreateDirectory(PathToExcelResults);
        }

        [TestMethod]
        public void BaseAlgorithmResultInInExcel()
        {
            try
            {
                new Excel.Application();
            }
            catch 
            {
                return;
            }

            var excelApp = new Excel.Application();
            excelApp.Workbooks.Add();
            Excel._Worksheet workSheet = excelApp.ActiveSheet;

            workSheet.Cells[1, 1] = "Name of file";
            workSheet.Cells[1, 2] = "Valid";
            workSheet.Cells[1, 3] = "Numerable";
            workSheet.Cells[1, 4] = "Time";
            workSheet.Cells[1, 5] = "Result";
            //workSheet.Cells[1, 6] = "Memory";
            string[] files = Directory.GetFiles(PathToSources);
            if (files.Length == 0) return;

            string new_file_name = GenerateNewName() + excel_type;

            for (int i = 0; i < files.Length; ++i)
            {
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                bool valid = false;
                int numerate = -1;
                int[] graphNumeration;
                timer.Start();
                valid = GetValid(files[i]);
                numerate = GetNumerate(files[i], out graphNumeration);
                timer.Stop();
                //TODO: Dinar: check possible values 
                if (graphNumeration == null) graphNumeration = new int[] { 0, 0 };
                //TODO: Dinar: check memory usage ? 
                workSheet.Cells[i + 2, 1] = current_file_name;
                workSheet.Cells[i + 2, 2] = valid;
                workSheet.Cells[i + 2, 3] = numerate;
                workSheet.Cells[i + 2, 4] = timer.Elapsed.ToString();
                workSheet.Cells[i + 2, 5] = IntArrayToList(graphNumeration);
            }
            
            workSheet.Columns[1].AutoFit();
            workSheet.Columns[2].AutoFit();
            workSheet.Columns[3].AutoFit();
            workSheet.Columns[4].AutoFit();
            workSheet.Columns[5].AutoFit();
            workSheet.Name = new_file_name;
            workSheet.SaveAs(Environment.CurrentDirectory.Normalize() +"/" +PathToExcelResults +"/"+ new_file_name);
            
            // Make the object visible.
            excelApp.Visible = true;
        }

   
        [TestMethod]
        public void BaseAlrorithmResultInRT()
        {
            string[] files = Directory.GetFiles(PathToSources);
            if (files.Length == 0) return;

            string new_file_name = PathToResults + "/" + GenerateNewName() + result_type;
            StreamWriter result = new StreamWriter(new_file_name);

            PrepareResultsHead(result);
            for (int i = 0; i < files.Length; ++i)
            {
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                bool valid = false;
                int numerate = -1;
                int[] graphNumeration;
                timer.Start();
                valid = GetValid(files[i]);
                numerate = GetNumerate(files[i], out graphNumeration);
                timer.Stop();
                //TODO: Dinar: check possible values 
                if (graphNumeration == null) graphNumeration = new int[] { 0, 0 };
                //TODO: Dinar: check memory usage ? 
                WriteRow(result, current_file_name, valid.ToString(), numerate.ToString(), timer.ElapsedMilliseconds.ToString(), IntArrayToList(graphNumeration));
            }
            result.WriteLine("\tEND OF THE TEST RESULTS");
            result.Close();
            
        }

    }
}
