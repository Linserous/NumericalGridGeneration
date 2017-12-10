using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using MeshRecovery_Lib;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class FunctionsTest
    {
        string PathToSources = @"tests/sources";
        string GoodFolder = @"good";
        string BadFolder = @"bad";
        string Fail = @"Fail";

        string GraphNumerationToString(int[][] graphNumeration)
        {
            if (graphNumeration == null) return Fail;

            string result = "";

            for (int i = 0; i < graphNumeration.Length; ++i)
            {
                if (graphNumeration[i] == null) return Fail;
                result += "[";
                for (int j = 0; j < graphNumeration[i].Length; ++j)
                {
                    result += graphNumeration[i][j].ToString();
                    if (j < graphNumeration[i].Length - 1) result += ",";
                }
                result += "]";
            }
            return result;
        }

        [TestMethod]
        public void CheckAllFilesWithBaseAlgorithm()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(
                   PathToSources, "*.graph", SearchOption.AllDirectories);

            Assert.AreNotEqual(files.Count(), 0, "There are no test files");
            Dictionary<Exception, string> exceptions = new Dictionary<Exception, string>();
            foreach (var file in files)
            {
                long[] xadj;
                int[] adjncy;
                int[][] graphNumeration;
                Loader.LoadGraphFromMETISFormat(file, out xadj, out adjncy);
                bool valid = MeshRecovery.Validate(xadj, xadj.Length, adjncy, out int meshDimension);
                int numerate = MeshRecovery.Numerate(xadj, xadj.Length, adjncy, out graphNumeration);

                bool isGood = valid && numerate == 0;

                if (isGood)
                {
                    try
                    {
                        int errCode = NumerationHelper.ValidateNumeration(xadj, adjncy, meshDimension, graphNumeration);
                        Assert.IsTrue(errCode >= 0,
                            $"ValidateNumeartion returns {errCode} for numeration {GraphNumerationToString(graphNumeration)}");
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e, Path.GetFileName(file));
                    }
                }

                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(file));
                if (dirInfo.Name == GoodFolder)
                {
                    Assert.IsTrue(isGood);
                }
                else if (dirInfo.Name == BadFolder)
                {
                    Assert.IsFalse(isGood);
                }
                else
                {
                    Assert.Fail("Should be good and bad files only");
                }
            }

            if (exceptions.Count > 0)
            {
                string output = "List of exceptions: ";
                foreach (var e in exceptions)
                    output += Environment.NewLine + "There is problem with file: " + e.Value + ". The reason: " + e.Key.Message + "|";
                Assert.Fail(output);
            }
        }
    }
}
