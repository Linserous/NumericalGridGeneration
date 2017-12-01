using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    public interface IDirection
    {
        int[] GetNextOffset();
        bool Valid();
        void Clear();
    }
}
