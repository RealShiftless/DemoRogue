using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World
{
    public readonly struct Chunk(Dungeon dungeon, Rect8? roomBody, RoomTypes? type, byte[] paths)
    {
        // Constants
        public const int BORDER = 2;


        // Values
        public readonly Dungeon Dungeon = dungeon;

        public readonly Rect8? RoomBody = roomBody;
        public readonly RoomTypes? RoomType = type;

        private readonly byte[] _paths = paths;
    }
}
