using Shiftless.Common.Mathematics;

namespace DemoRogue.World
{
    public readonly struct Path(Point8 origin, Point8 dest, Axis axis, int turnIndex)
    {
        // Constants
        public const int PATH_TOP = 0;
        public const int PATH_LEFT = 1;
        public const int PATH_BOTTOM = 2;
        public const int PATH_RIGHT = 3;


        // Values
        public readonly Point8 Origin = origin;
        public readonly Point8 Destination = dest;

        public readonly Axis Axis = axis;

        public readonly byte TurnIndex = (byte)turnIndex;



        // Func

        public bool Contains(Point8 point) => Axis switch
        {
            Axis.Horizontal => CheckHorizontal(this, point),
            Axis.Vertical => CheckVertical(this, point),
            _ => false
        };

        private static bool CheckVertical(Path line, Point8 point)
        {
            // First get some values
            Point8 origin = line.Origin;
            Point8 dest = line.Destination;
            byte turnIndex = line.TurnIndex;

            int minX = Math.Min(origin.X, dest.X);
            int maxX = Math.Max(origin.X, dest.X);

            int minY = Math.Min(origin.Y, dest.Y);
            int maxY = Math.Max(origin.Y, dest.Y);

            // First we check if the y is even within bounds
            if (point.Y < minY || point.Y > maxY)
                return false;

            // Now we calculate an index on the y, we do it like this because sometimes we move down and we should index inverted n shi
            int index = Math.Abs(point.Y - origin.Y);

            // If we are before the turn we check if the points x match the 
            if (index < turnIndex)
                return point.X == origin.X;
            else if (index == turnIndex)
                return point.X >= minX && point.X <= maxX;
            else
                return point.X == dest.X;
        }
        private static bool CheckHorizontal(Path line, Point8 point)
        {
            // First get some values
            Point8 origin = line.Origin;
            Point8 dest = line.Destination;
            byte turnIndex = line.TurnIndex;

            int minX = Math.Min(origin.X, dest.X);
            int maxX = Math.Max(origin.X, dest.X);

            int minY = Math.Min(origin.Y, dest.Y);
            int maxY = Math.Max(origin.Y, dest.Y);

            // Now we check if we are in bounds
            if (point.X < minX || point.X > maxX)
                return false;

            // Now we calculate an index on the x, we do it like this because sometimes we move right and we should index inverted n shi
            int index = Math.Abs(point.X - origin.X);

            // If we are before the turn we need to match to the point.Y to the origin.Y, as we are, well, before the turn haha
            if (index < turnIndex)
                return point.Y == origin.Y;

            // Now that we are in the turn we must match the y within the bounds n shi
            else if (index == turnIndex)
                return point.Y >= minY && point.Y <= maxY;

            // And now we match based on the destination
            else
                return point.Y == dest.Y;
        }
    }
}
