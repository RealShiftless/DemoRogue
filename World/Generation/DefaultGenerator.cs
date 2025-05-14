using DemoRogue.World.Building;
using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Registration;
using System.Diagnostics;

namespace DemoRogue.World.Generation
{
    public class DefaultGenerator : RegistryItem, IGenerator
    {
        // Values
        private int _gridWidth = 6;
        private int _gridHeight = 6;

        public int GridBorder = 2;

        public int MinRoomWidth = 5;
        public int MinRoomHeight = 5;
        public int MaxRoomWidth = 16;
        public int MaxRoomHeight = 16;

        public float RoomPercentage = 0.6f;
        public float PathPercentage = 0.7f;

        //private Random RNG = new();

        //private DungeonRoom[,]? _rooms;
        //private int _roomCount;

        private Point8? _spawn;


        // Properties
        public int GridWidth => _gridWidth;
        public int GridHeight => _gridHeight;

        public int MaxRooms => _gridWidth * _gridHeight;

        public int MaxRoomGridWidth => Dungeon.WIDTH / _gridWidth - GridBorder * 2;
        public int MaxRoomGridHeight => Dungeon.HEIGHT / _gridHeight - GridBorder * 2;

        public Point8 CurrentSpawn => _spawn ?? throw new NullReferenceException("Spawn was null!");


        // Constructor
        public DefaultGenerator()
        {

        }


        // Setters
        public void SetSize(int gridWidth, int gridHeight)
        {
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
        }


        // Interface
        void IGenerator.Generate(DungeonBuilder dungeon)
        {
            // First we generate rooms
            dungeon.EnumerateGrid((gridX, gridY) =>
            {
                // First we check if we should spawn a room, otherwise make a dummy room
                // Dummy rooms are used for easier paths
                if(RNG.NextFloat() > RoomPercentage)
                {
                    // We randomly get the dummy rooms position
                    int dummyX = RNG.Next(2, dungeon.MaxRoomWidth);
                    int dummyY = RNG.Next(2, dungeon.MaxRoomHeight);

                    // And create a new dungeon room with a width and height of 1 :)
                    dungeon.SetRoom(gridX, gridY, new(dummyX, dummyY, 1, 1), GameRegistry.Rooms.Dummy);

                    return;
                }

                // We generate a room size
                int width = RNG.Next(MinRoomWidth, dungeon.MaxRoomWidth);
                int height = RNG.Next(MinRoomHeight, dungeon.MaxRoomHeight);

                // We get the maximum position the chunk can be in. This is because say the MaxRoomGridWidth is 16, the generated room is of Width 16, if we put it at anywhere else than
                // 0,0 (local) it will overlap with different sections of the grid
                // We also do it in this order because otherwise the room size randomization will be skewed towards smalled rooms
                int maxX = dungeon.MaxRoomWidth - width;
                int maxY = dungeon.MaxRoomHeight - height;

                // We get the random room position based on the max position
                int localX = RNG.Next(maxX) + GridBorder;
                int localY = RNG.Next(maxY) + GridBorder;

                // We create the room
                dungeon.SetRoom(gridX, gridY, new(localX, localY, width, height), GameRegistry.Rooms.Default);
            });

            // Now generate the paths
            dungeon.EnumerateGrid((gridX, gridY) =>
            {
                // First we try to generate one upward
                if (gridY != _gridHeight - 1 && RNG.NextFloat() < PathPercentage)
                    dungeon.CreatePath(new(gridX, gridY), Direction.Up);

                // And now we try to generate one rightward
                if (gridX != _gridWidth - 1 && RNG.NextFloat() < PathPercentage)
                    dungeon.CreatePath(new(gridX, gridY), Direction.Right);
            });

            // Now we validate all paths
            dungeon.ValidateAllPaths();

        }
    }
}
