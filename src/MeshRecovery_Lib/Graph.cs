using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeshRecovery_Lib;

namespace MeshRecovery_Lib
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

        public long GetAdjVertices(int v, out int[] buff)
        {
            buff = null;
            if (v >= xadj.Length - 1 || v < 0) return -1;
            buff = new int[xadj[v+1]-xadj[v]];
            for (long i = xadj[v]; i < xadj[v + 1]; ++i)
                buff[i - xadj[v]] = adjncy[i];
            return xadj[v + 1] - xadj[v];
        }

        public long GetAdjVerticesCount(int v)
        {
            if (v >= xadj.Length - 1 || v < 0) return -1;
            return xadj[v + 1] - xadj[v];
        }

        public int GetEdgeCount()
        {
	        return adjncy.Length/2;
        }

        public int GetVerticesCount()
        {
	        return xadj.Length - 1;
        }

        public bool CoherentGraphCheck()
        {
            int size = GetVerticesCount();
            int[] vertexState = new int[size];
            bool red = false;
            int k = 0;
            int finalCount = 0;
            for (int i = 0; i < size; i++)
                vertexState[i] = 1;
            vertexState[0] = 2;
            do
            {
                red = true;

                for (int i = 0; i < size; i++)
                    if (vertexState[i] == 2)
                    {
                        vertexState[i] = 3;
                        k = i;
                        break;
                    }

                for (long i = xadj[k]; i < xadj[k + 1]; i++)
                {
                    if (vertexState[adjncy[i]] == 1)
                    {
                        vertexState[adjncy[i]] = 2;
                    }
                }

                for (int i = 0; i < size; i++)
                {
                    if (vertexState[i] == 2)
                        red = false;
                }
            } while (red == false);

            for (int i = 0; i < size; i++)
                if (vertexState[i] == 1)
                    finalCount++;

            if (finalCount == 0)
                return true;
            else
                return false;
        }
    }
}
