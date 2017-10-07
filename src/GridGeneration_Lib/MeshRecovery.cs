using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    public static class MeshRecovery
    {
        /// <summary>
        /// Method checks that input graph corresponds to a regular grid
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="meshDimension">Dimension of a regular grid that corresponds to the input graph</param>
        /// <returns>true if graph corresponds to a regular grid, otherwise false</returns>
        public static bool Validate(long[] xadj, int size, int[] adjncy, out int meshDimension)
        {
            // TODO: Implement
            meshDimension = 0;
            return false;
        }

        /// <summary>
        /// Method restores geometry information for each graph node
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        /// </returns>
        public static int Numerate(long[] xadj, int size, int[] adjncy, out int[] graphNumeration)
        {
            // TODO: Implement
            // TODO: graphNumeration may be 1-3 dimensional array
            graphNumeration = null;
            return 0;
        }
    }
}
