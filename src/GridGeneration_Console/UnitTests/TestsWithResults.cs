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
        string[] head_columns = { "Name of file", "Valid", "Numerable", "Time", "Result" };
        int Padding = 25;

        void WriteRow(StreamWriter file, params string[] strings)
        {
            file.WriteLine(PrepareRow(strings));
        }
        void PrepareResultsHead(StreamWriter file)
        {
            file.WriteLine("\tTest result fot Graph Restorer functions ( " + DateTime.Now.ToLongTimeString() + " ).");
            WriteRow(file, head_columns); //, "Memory" );
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
            return MeshRecovery_Lib.MeshRecovery.Validate(index, indexSize, graphData, out int gridDimension);
        }
        int GetNumerate(string file, out int[] graphNumeration)
        {
            //TODO: Dinar: implement correct version
            int[] index = { };
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

        public ResultsInTable()
        {

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
                WriteRowExcel(workSheet, i+2, current_file_name, valid.ToString(), numerate.ToString(), timer.Elapsed.ToString(), IntArrayToList(graphNumeration));
            }
            for (int i = 0; i < head_columns.Length; ++i)
            {
                workSheet.Columns[i+1].AutoFit();
            }
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
