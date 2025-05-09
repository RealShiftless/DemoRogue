using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Generation
{
    internal class OneRoomGenerator : IGenerator
    {
        public const int BORDER = 2;

        public Point8 CurrentSpawn => new(64, 64);

        public void Generate() { }

        public bool IsTileAir(Point8 point)
        {
            return point.X > BORDER && point.X <= Dungeon.WIDTH - BORDER && point.Y > BORDER && point.Y <= Dungeon.HEIGHT - BORDER;
        }
    }
}
