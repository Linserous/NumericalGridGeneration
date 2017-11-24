using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MeshRecovery_Lib;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class FunctionsTest
    {
        string PathToSources = @"tests/sources";
        string PathToAnswers = @"tests/answers";
        string Fail = @"Fail";
        string GraphNumerationToString(int[][] graphNumeration)
        {
            if (graphNumeration == null) return Fail;

            string result="";

            for (int i = 0; i < graphNumeration.Length; ++i)
            {
                if (graphNumeration[i] == null) return Fail;
                result += "[";
                for (int j = 0; j < graphNumeration[i].Length; ++j)
                {
                    result += graphNumeration[i][j].ToString();
                    if ((j > 0) && (j< graphNumeration[i].Length -1)) result += ",";
                }
                result += "]";
            }
            return result;
        }

        [TestMethod]
        public void CheckAllFilesWithBaseAlgorithm()
        {
            string[] files = Directory.GetFiles(PathToSources);
            Assert.AreNotEqual(files.Length, 0);
            Dictionary<Exception, string> exceptions = new Dictionary<Exception, string>();
            for (int i = 0; i < files.Length; ++i)
            {
                long[] xadj;
                int[] adjncy;
                int[][] graphNumeration;
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                bool valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out int meshDimension);
                int numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out  graphNumeration);

                StreamReader read;
                try
                {
                    read = new StreamReader(PathToAnswers + "/" + current_file_name.Replace(".graph", ".num"));
                    string answer = read.ReadLine();
                    read.Close();
                    
                    //checking expected validation and numerating resrults
                    StringAssert.Contains(answer, valid.ToString()+" "+numerate.ToString());
                    
                    //checking created graph numeration 
                    if (valid&&(numerate>-1))
                    {
                        int errCode = NumerationHelper.ValidateNumeration(xadj, adjncy, meshDimension, graphNumeration);
                        Assert.IsTrue(errCode >= 0,
                            $"ValidateNumeartion returns {errCode} for numeration {GraphNumerationToString(graphNumeration)}");
                    }
                }
                catch (Exception e)
                {
                    exceptions.Add(e, current_file_name);
                }
            }

            if (exceptions.Count>0)
            {
                string output = "List of exceptions: ";
                foreach (var e in exceptions)
                    output += Environment.NewLine+ "There is problem with file: "+e.Value+". The reason: "+e.Key.Message+"|";
                Assert.Fail(output);
                
            }        
        }
    }
}
