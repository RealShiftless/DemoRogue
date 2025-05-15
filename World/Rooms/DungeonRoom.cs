using Shiftless.Common.Mathematics;

namespace DemoRogue.World.Rooms
{
    public class DungeonRoom
    {
        // Values
        public readonly Dungeon Dungeon;

        public Rect8 Body;

        public Path?[] Paths = new Path?[4];

        public bool LeadsToSource = false;


        // Constructor
        internal DungeonRoom(Dungeon dungeon, Rect8 body)
        {
            Dungeon = dungeon;
            Body = body;
        }


        // Func
        public bool Contains(Point8 point, bool includePaths = false)
        {
            if (Body.Contains(point))
                return true;

            if (!includePaths)
                return false;

            foreach (Path? line in Paths)
                if (line?.Contains(point) == true)
                    return true;

            return false;
        }
    }
}
