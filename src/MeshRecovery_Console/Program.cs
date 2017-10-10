using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeshRecovery_Lib;
using System.IO;
using System.Diagnostics;

namespace MeshRecovery_Console
{
    class Program
    {
        public static void LoadGraphFromMETISFormat(string filename, out long[] xadj, out int[] adjncy)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                char[] splitters = new char[2] { '\t', ' ' };
                String line;
                string[] vals;
                long edges = 0;
                int nodes = 0;
                int i, j, n;

                xadj = null;
                adjncy = null;
                bool wnodes = false;
                bool wedges = false;
                int mnodes = 1;

                //считываю число вершин и число ребер
                if ((line = sr.ReadLine()) != null)
                {
                    vals = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length > 1 &&
                        int.TryParse(vals[0], out nodes) &&
                        long.TryParse(vals[1], out edges))
                    {
                        edges *= 2;
                        xadj = new long[nodes + 1];
                        adjncy = new int[edges];
                    }
                    if (vals.Length > 2 && vals[2].Length == 3)
                    {
                        string s = vals[2].Trim();
                        wedges = s[2] == '1';
                        wnodes = s[1] == '1';
                    }
                    if (wnodes && vals.Length > 3)
                    {
                        int.TryParse(vals[3], out mnodes);
                    }
                }

                if (xadj == null || adjncy == null)
                    throw new Exception(string.Format("LoadMETIS! Нет информации о количестве верши и ребер графа: {0}", line));
                //считываю данные о ребрах
                n = i = j = 0;
                xadj[n] = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    vals = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                    i = 0;
                    if (wnodes) i += mnodes;    //граф взвешенный, игнорирую веса вершин
                    xadj[n + 1] = xadj[n];
                    while (i < vals.Length)
                    {
                        //номер смежной вершины
                        if (!int.TryParse(vals[i], out j))
                            throw new Exception(string.Format("LoadMETIS! Строка не соответствует формату: {0}", line));
                        j--;                //в файле индексы начинаются с 1
                        if (j >= nodes)
                            throw new Exception(string.Format("LoadMETIS! Недопустимые значения индексов: {0}", line));
                        i++;
                        if (wedges) i++;    //граф взвешенный, игнорирую веса ребер
                        adjncy[xadj[n + 1]] = j;
                        xadj[n + 1] += 1;
                    }
                    n++;
                }
                //тест на число ребер
                if (edges != xadj[n])
                    throw new Exception(string.Format("LoadMETIS! Неуспешный тест на число ребер!"));
            }
        }

        static bool Validate(int n, long[] xadj, int[] adjncy)
        {
            //тело функции
            if (true) return true;
        }
        static int[] Numerate(int n, long[] xadj, int[] adjncy)
        {
            int[] a = new int[n];
            //тело функции. Массив переименовать как удобно
            return a;
        }
        static void Main(string[] args)
        {
            long[] xadj = null;
            int[] adjncy = null;
            LoadGraphFromMETISFormat(@"E:\NumericalGridGeneration\tests\sources\line2.graph", out xadj, out adjncy);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Validate(adjncy.Length, xadj, adjncy);
            timer.Stop();
            Console.WriteLine("Validate отработала. Затрачено: " + timer.Elapsed);
            timer.Reset();
            timer.Start();
            Numerate(adjncy.Length, xadj, adjncy);
            timer.Stop();
            Console.WriteLine("Numerate отработала. Затрачено: " + timer.Elapsed);
            Console.ReadKey();
        }
    }
}
