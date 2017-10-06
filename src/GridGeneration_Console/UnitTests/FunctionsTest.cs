using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class FunctionsTest
    {
        string PathToSources = @"tests/sources";
        string PathToAnswers = @"tests/answers";

        string GraphNumerationToStaring(int[] grapgNumeration)
        {
            string result = grapgNumeration[0].ToString();
            for (int i = 0; i < grapgNumeration.Length; ++i)
                result += " "+grapgNumeration.ToString();
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
                
                Graph.Loader.LoadGraphFromMETISFormat(files[i], out long[] xadj, out int[] adjncy);
                bool valid = MeshRecovery_Lib.MeshRecovery.Validate(xadj, 0, adjncy, out int meshDimension);
                int numerate = MeshRecovery_Lib.MeshRecovery.Numerate(xadj, 0, adjncy, out int[] graphNumeration);
                //TODO: Dinar: check possible values 
                if (graphNumeration == null) graphNumeration = new int[] { 0, 0 };
                StreamReader read = new StreamReader(PathToAnswers + "/" + current_file_name.Replace(".graph", ".num"));
                //TODO: Dinar: resolve answers for cube and line graphss
                Assert.AreEqual(GraphNumerationToStaring(graphNumeration), read.ReadToEnd());
                read.Close();
            }
            
        }
    }
}
