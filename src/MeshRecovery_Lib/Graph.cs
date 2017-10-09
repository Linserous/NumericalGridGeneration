using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
