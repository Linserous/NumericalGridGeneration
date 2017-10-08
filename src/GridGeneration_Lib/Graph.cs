using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridGeneration_Lib
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

        public long GetAdjVertexes(int v, int[] buff)
        {
            for (long i = xadj[v]; i < xadj[v + 1]; ++i)
                buff[i - xadj[v]] = adjncy[i];
            return xadj[v + 1] - xadj[v];
        }

        public long GetAdjVertexesCount(int v)
        {
            return xadj[v + 1] - xadj[v];
        }

        public int GetEdgeCount()
        {
	        return adjncy.Length;
        }

        public int GetVertexCount()
        {
	        return xadj.Length;
        }
    }
}
