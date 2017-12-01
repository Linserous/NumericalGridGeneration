using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    public class Direction2D: IDirection
    {
        enum Direction
        {
            PositiveX = 0,
            PositiveY,
            NegativeX,
            NegativeY,
            Last
        }
        Direction direction;

        public int[] GetNextOffset()
        {
            var directionValue = (int)Math.Pow(-1, (int)direction / 2);
            int x = (int)direction % 2 == 0 ? directionValue : 0;
            int y = (int)direction % 2 != 0 ? directionValue : 0;

            ++direction;

            return new int[] { x, y };
        }

        public bool Valid()
        {
            return direction != Direction.Last;
        }

        public void Clear()
        {
            direction = Direction.PositiveX;
        }
    }
}
