using DemoRogue.World.Building;

namespace DemoRogue.World
{
    public interface IGenerator
    {
        int GridWidth { get; }
        int GridHeight { get; }

        void Generate(DungeonBuilder dungeon);
    }
}
