using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace MeshRecovery_Visualizer
{
    public class Loader
    {
        #region Graph METIS Format

        /// <summary>
        /// Загрузить граф из graph-файла.
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <param name="xadj">Массив индексов (CSR-формат описания графа)</param>
        /// <param name="adjncy">Массив смежности (CSR-формат описания графа)</param>
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
        #endregion
    }
}
