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

        // CompareVertex - will find a difference btw vertex
        // Output:
        // i  - [0,..,meshDimension-1] index of difference
        // -1 - same numerate of indexes
        // -2 - length of vertex numerating is different
        // -3 - there are two or more difference
        // -4 - there is difference with step more than 1,  like [0,0] [0,2] 
        int CompareVertex(int[] v1, int[] v2, int meshDimension)
        {
            if ((v1.Length!=meshDimension)||(v2.Length!=meshDimension)) return -2;
            int diff_count = 0;
            int diff_index = -1;
            for (int i=0;i<meshDimension;++i)
            {
                if (Math.Abs(v1[i]-v2[i])==1)
                {
                   diff_index = i;
                   ++diff_count;
                }
                if (Math.Abs(v1[i] - v2[i]) > 1)
                {
                    return -4;
                }
            }
            if (diff_count > 1) return -3;
            else return diff_index;
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
                        Graph graph = new Graph(xadj, adjncy);
                        Traversal traversal = new Traversal(graph);
                        traversal.NewVertex += (sender, e) =>
                        {
                            int[] adj;
                            long adj_count = graph.GetAdjVertices(e, out adj);
                            int flag = 0;
                            for (int k=0;k<adj_count-1;++k)
                            {
                                for (int l=k+1;l<adj_count;++l)
                                {
                                    int res = CompareVertex(graphNumeration[adj[l]], graphNumeration[adj[k]], meshDimension);
                                    if (res == -1)
                                        Assert.Fail("The function CompareVertex return " + res.ToString() + " for numerate: " + GraphNumerationToString(graphNumeration));
                                }
                            }
                            for (int k = 0; k < adj_count ; ++k)
                            {
                                int res = CompareVertex(graphNumeration[e], graphNumeration[adj[k]], meshDimension);
                                if (res < 0)
                                    Assert.Fail("The function CompareVertex return " + res.ToString() + " for numerate: " + GraphNumerationToString(graphNumeration));
                            }
                        }; 
                        traversal.Run();
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
                    output += "\nThere is problem with file: "+e.Value+". The reason: "+e.Key.Message+"|";
                Assert.Fail(output);
            }
            
        }
    }
}
