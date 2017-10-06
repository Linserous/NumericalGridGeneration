using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Graph
{
    public class Graph
    {
        private int[] adjncy;
        private long[] xadj;

        public Graph(long[] xadj, int[] adjncy)
        {
            this.xadj = xadj;
            this.adjncy = adjncy;
        }
    }

    

    public class Loader
    {
        #region Graph METIS Format

        /// <summary>
        /// ��������� ���� �� graph-�����.
        /// </summary>
        /// <param name="filename">������ ���� � �����</param>
        /// <param name="xadj">������ �������� (CSR-������ �������� �����)</param>
        /// <param name="adjncy">������ ��������� (CSR-������ �������� �����)</param>
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
                
                //�������� ����� ������ � ����� �����
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
                    throw new Exception(string.Format("LoadMETIS! ��� ���������� � ���������� ����� � ����� �����: {0}", line));
                //�������� ������ � ������
                n = i = j = 0;
                xadj[n] = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    vals = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                    i = 0;
                    if (wnodes) i += mnodes;    //���� ����������, ��������� ���� ������
                    xadj[n + 1] = xadj[n];
                    while (i < vals.Length)
                    {
                        //����� ������� �������
                        if (!int.TryParse(vals[i], out j))    
                            throw new Exception(string.Format("LoadMETIS! ������ �� ������������� �������: {0}", line));
                        j--;                //� ����� ������� ���������� � 1
                        if (j >= nodes)
                            throw new Exception(string.Format("LoadMETIS! ������������ �������� ��������: {0}", line));
                        i++;
                        if (wedges) i++;    //���� ����������, ��������� ���� �����
                        adjncy[xadj[n + 1]] = j;
                        xadj[n + 1] += 1;
                    }
                    n++;
                }
                //���� �� ����� �����
                if (edges != xadj[n])
                    throw new Exception(string.Format("LoadMETIS! ���������� ���� �� ����� �����!"));
            }
        }

        public static Graph LoadGraphFromMETISFormat(string filename)
        {
            long[] xadj;
            int[] adjncy;
            LoadGraphFromMETISFormat(filename, out xadj, out adjncy);
            return new Graph(xadj, adjncy);
        }

        /// <summary>
        /// ��������� ���� � graph-���� (METIS ������).
        /// </summary>
        /// <param name="g">����������� ����</param>
        /// <param name="filename">���� � ��������� �����</param>
        public static void SaveGraphToMETISFormat(Graph g, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                //�������� ����� ������ � ����� ��� (����� ������� � ������� ��������� ��� ����� ���������) � ������ ������
                //sw.Write(string.Format("{0}\t{1}\t011", g.Size, g.ArcsCount));
                sw.Write(string.Format("{0}\t{1}", g.Size, g.ArcsCount / 2));
                //�������� ������� � �� �����
                for (int i = 0; i < g.Size; i++)
                {
                    //[� �������] [���� 1 (������� ������������)] [������� �������]
                    //sw.Write(string.Format("\n{0}\t", i, 1, g.Weight[i]));
                    sw.Write(string.Format("\n"));
                    foreach (IArc j in g.Inc(i))
                    {
                        sw.Write(string.Format(" {0}", j.NodeB + 1));
                        //sw.Write(string.Format(" {0} {1}", j.NodeB + 1, j.Weight));
                    }
                }
                sw.Close();
            }
        }

        #endregion

        #region XYZ Format

        /// <summary>
        /// ��������� �������������� ���������� � ����� �� XYZ-�����.
        /// </summary>
        /// <param name="filename">������ ���� � �����</param>
        /// <returns>��������� �� ����� ����</returns>
        public static XYZ[] LoadGeometryFromXYZFormat(string filename)
        {
            XYZ[] xyz;
            using (StreamReader sr = new StreamReader(filename))
            {
                char[] splitters = new char[2] { '\t', ' ' };
                String line;
                string[] vals;
                int dim, nodes, nodes_count;
                int i;
                double x, y, z;

                //�������� �����������
                if ((line = sr.ReadLine()) == null || !int.TryParse(line, out dim))
                {
                    throw new Exception(string.Format("LoadXYZ! ��� ������ � �����������: {0}", line));
                }

                //�������� �����������
                if ((line = sr.ReadLine()) == null || !int.TryParse(line, out nodes))
                {
                    throw new Exception(string.Format("LoadXYZ! ��� ������ � ����� ������: {0}", line));
                }

                xyz = new XYZ[nodes];

                //�������� ���������� ������
                nodes_count = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    vals = line.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                    if (vals.Length < (dim + 1))
                    {
                        throw new Exception(string.Format("LoadXYZ! � ������ �� ���������� ������: {0}", line));
                    }
                    if (!int.TryParse(vals[0], out i))
                    {
                        throw new Exception(string.Format("LoadXYZ! � ������ �� ������ ����� �������: {0}", line));
                    }
                    if (i < 0 || i >= nodes)
                    {
                        throw new Exception(string.Format("LoadXYZ! ������������ ����� �������: {0}", line));
                    }
                    if (xyz[i] != null)
                    {
                        throw new Exception(string.Format("LoadXYZ! ������� ��� ���� ���������� �����: {0}", line));
                    }
                    x = y = z = 0.0;
                    if ((dim > 0 && !(double.TryParse(vals[1], System.Globalization.NumberStyles.Any, null, out x) || double.TryParse(vals[1], out x))) ||
                        (dim > 1 && !(double.TryParse(vals[2], System.Globalization.NumberStyles.Any, null, out y) || double.TryParse(vals[2], out y))) ||
                        (dim > 2 && !(double.TryParse(vals[3], System.Globalization.NumberStyles.Any, null, out z) || double.TryParse(vals[3], out z))))
                    {
                        throw new Exception(string.Format("LoadXYZ! ����������� ���������� ����������: {0}", line));
                    }
                    nodes_count++;
                    xyz[i] = new XYZ(x, y, z);
                }

                //���� �� ����� ������
                if (nodes_count != nodes)
                {
                    throw new Exception(string.Format("LoadXYZ! ���������� ���� �� ����� ������!"));
                }
            }
            return xyz;
        }

        public static void SaveGeometryToXYZFormat(XYZ[] xyz, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("3");
                sw.WriteLine(xyz.Length);
                for (int i = 0; i < xyz.Length; i++)
                    sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", i, xyz[i].X, xyz[i].Y, xyz[i].Z));
                sw.Close();
            }
        }


        #endregion
    }

    public class XYZ
    {
        private double x;
        private double y;
        private double z;

        public XYZ(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
