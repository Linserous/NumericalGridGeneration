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
            string result = graphNumeration[0].ToString();
            for (int i = 0; i < graphNumeration.Length; ++i)
                result += " "+graphNumeration.ToString();
            return result;
        }

        [TestMethod]
        public void CheckAllFilesWithBaseAlgorithm()
        {
            string[] files = Directory.GetFiles(PathToSources);
            Assert.AreNotEqual(files.Length, 0);

            for (int i = 0; i < files.Length; ++i)
            {
                string current_file_name = files[i].Remove(0, (PathToSources + "\\").Length);
                
                Loader.LoadGraphFromMETISFormat(files[i], out long[] xadj, out int[] adjncy);
                bool valid = MeshRecovery.Validate(xadj, 0, adjncy, out int meshDimension);
                int numerate = MeshRecovery.Numerate(xadj, 0, adjncy, out int[] graphNumeration);
                //TODO: Dinar: check possible values 
                if (graphNumeration == null) graphNumeration = new int[] { 0, 0 };
                StreamReader read = new StreamReader(PathToAnswers + "/" + current_file_name.Replace(".graph", ".num"));
                //TODO: Dinar: resolve answers for cube and line graphss
                Assert.AreEqual(GraphNumerationToString(graphNumeration), read.ReadToEnd());
                read.Close();
            }
            
        }
    }
}
