using System;

namespace MeshRecovery_Lib
{
    public class Direction3D: IDirection
    {
        enum Direction
        {
            PositiveX = 0,
            PositiveY,
            PositiveZ,
            NegativeX,
            NegativeY,
            NegativeZ,
            Last
        }
        Direction direction = Direction.PositiveX;

        public int[] GetNextOffset()
        {
            var directionValue = (int)Math.Pow(-1, (int)direction / 3);
            int x = (int)direction % 3 == 0 ? directionValue : 0;
            int y = (int)direction % 3 == 1 ? directionValue : 0;
            int z = (int)direction % 3 == 2 ? directionValue : 0;

            ++direction;

            return new int[] { x, y, z };
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
