using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MeshRecovery_Lib;

namespace UnitTests
{
    [TestClass]
    public class FunctionsTest
    {
        string PathToSources = @"tests/sources";
        string PathToAnswers = @"tests/answers";

        string GraphNumerationToString(int[] graphNumeration)
        {
            if (graphNumeration == null) return "";

            string result = graphNumeration[0].ToString();
            for (int i = 1; i < graphNumeration.Length; ++i)
                result += " "+graphNumeration[i].ToString();
            return result;
        }

        [TestMethod]
        public void CheckAllFilesWithBaseAlgorithm()
        {
            string[] files = Directory.GetFiles(PathToSources);
            Assert.AreNotEqual(files.Length, 0);

            for (int i = 0; i < files.Length; ++i)
            {
                long[] xadj;
                int[] adjncy;
                int[] graphNumeration;
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                Loader.LoadGraphFromMETISFormat(files[i], out xadj, out adjncy);
                bool valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out int meshDimension);
                int numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out  graphNumeration);

                StreamReader read;
                try
                {
                    read = new StreamReader(PathToAnswers + "/" + current_file_name.Replace(".graph", ".num"));
                    StringAssert.Contains(read.ReadToEnd().Replace('\n', ' '), GraphNumerationToString(graphNumeration));
                    read.Close();
                }
                catch (Exception e)
                {
                    if (e.GetType().FullName == "System.IO.FileNotFoundException")
                    {
                        Assert.Inconclusive("There is no asnwer for " + current_file_name);
                    }
                    else throw e;
                }
            }
            
        }
    }
}
