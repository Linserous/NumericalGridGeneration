using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    public enum Error
    {
        OK = 0,
        IMPOSSIBLE_NUM = -1,
        STACKOVERFLOW = -2,
        INVALID_DIM = -3,
        NEED_MORE_DATA = -4
    }

    public interface IVertexNumerator
    {
        void Init(int vertex, Graph graph);
        /// Unambiguously numerate the vertex
        Error Numerate(int[][] graphNumeration);
        /// Try to numerate the vertex, which can not have an unambiguous index
        Error TryToNumerate(int[][] graphNumeration);
        void Clear();
        List<int> GetNumeratedAdjVertices(int[][] graphNumeration);
        List<int> GetEnumeratedAdjVertices(int[][] graphNumeration);
    }
}
