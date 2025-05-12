using DemoRogue.World.Building;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World
{
    public interface IGenerator
    {
        Point8 CurrentSpawn { get; }

        int GridWidth { get; }
        int GridHeight { get; }

        void Generate(DungeonBuilder dungeon);
    }
}
