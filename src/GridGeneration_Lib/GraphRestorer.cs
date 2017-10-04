using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridGeneration_Lib
{
    public static class GraphRestorer
    {
        /// <summary>
        /// Method checks that input graph corresponds to a regular grid
        /// </summary>
        /// <param name="index">Index array for graph</param>
        /// <param name="indexSize">Size of index array</param>
        /// <param name="graphData">Graph data array</param>
        /// <param name="gridDimension">Dimension of a regular grid that corresponds to input graph</param>
        /// <returns>true if graph corresponds to a regular grid, otherwise false</returns>
        public static bool Validate(int[] index, int indexSize, int[] graphData, out int gridDimension)
        {
            // TODO: Implement
            gridDimension = 0;
            return false;
        }

        /// <summary>
        /// Method restores geometry information for each graph node
        /// </summary>
        /// <param name="index">Index array for graph</param>
        /// <param name="indexSize">Size of index array</param>
        /// <param name="graphData">Graph data array</param>
        /// <param name="graphNumeration">Restored numeration for input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - successful
        /// -1 - error
        /// </returns>
        public static int Numerate(int[] index, int indexSize, int[] graphData, out int[] graphNumeration)
        {
            // TODO: Implement
            // TODO: graphNumeration may be 1-3 dimensional array
            graphNumeration = null;
            return 0;
        }
    }
}
