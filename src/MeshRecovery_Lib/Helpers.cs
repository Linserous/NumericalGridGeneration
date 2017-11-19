using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    class Helpers
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }

    class NumerationHelpers
    {
        public static void Clear(ref int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                numeration[i] = null;
            }
        }

        public static bool IndexExists(int[] index, ref int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                if (numeration[i] != null &&
                    numeration[i][0] == index[0] &&
                    numeration[i][1] == index[1]) return true;
            }
            return false;
        }
    }
}
