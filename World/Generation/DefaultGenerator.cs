using DemoRogue.World.Building;
using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;

namespace DemoRogue.World.Generation
{
    public class DefaultGenerator : IGenerator
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
        public float PathPercentage = 0.9f;

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
            /*
            // We get the max size of the room, you can set this in code, but due to the grid like generation, this size might be bigger than actually usefull in the grid like system
            // Due to room overlap.
            // TODO: Think about a possibility of making this a toggle. Or atleast make something that fuses two rooms together might be cool :)
            int maxRoomWidth = Math.Max(MaxRoomWidth, MaxRoomGridWidth);
            int maxRoomHeight = Math.Max(MaxRoomHeight, MaxRoomGridHeight);

            // We also calculate the amount of tiles in each section of the grid :)
            int gridTileWidth = Dungeon.WIDTH / _gridWidth;
            int gridTileHeight = Dungeon.HEIGHT / _gridHeight;
            */

            // We also store valid rooms here. Because sometimes we generate dummy rooms based on roomSpawnPercentage. 
            // These dummy rooms are used to later generate paths. This way I can generate paths between rooms further appart than 1 grid cell in an easy optimized way :)
            // This way it's easier to generate random loot later on, and also get the spawn position for the player & enemies for example :)
            // List<Point8> validRooms = [];

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
                    dungeon.SetRoom(gridX, gridY, new(dummyX, dummyY, 1, 1), RoomTypes.Dummy);

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
                dungeon.SetRoom(gridX, gridY, new(localX, localY, width, height), RoomTypes.Default);
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
            //dungeon.ValidateAllPaths();

            /*
            // First generate the rooms
            EnumerateGrid((gridX, gridY) =>
            {
                // We first check if we should even generate a room.
                // If we don't we generate a dummy room
                if (RNG.NextFloat() > RoomPercentage)
                {
                    // We randomly get the dummy rooms position
                    int dummyX = gridTileWidth * gridX + RNG.Next(2, maxRoomWidth);
                    int dummyY = gridTileHeight * gridY + RNG.Next(2, maxRoomHeight);

                    // And create a new dungeon room with a width and height of 1 :)
                    _rooms[gridX, gridY] = new DungeonRoom(new(dummyX, dummyY, 1, 1));

                    return;
                }

                // We generate a room size
                int width = RNG.Next(MinRoomWidth, maxRoomWidth);
                int height = RNG.Next(MinRoomHeight, maxRoomHeight);

                // We get the maximum position the chunk can be in. This is because say the MaxRoomGridWidth is 16, the generated room is of Width 16, if we put it at anywhere else than
                // 0,0 (local) it will overlap with different sections of the grid
                // We also do it in this order because otherwise the room size randomization will be skewed towards smalled rooms
                int maxX = maxRoomWidth - width;
                int maxY = maxRoomHeight - height;

                // We get the random room position based on the max position
                int localX = RNG.Next(maxX) + GridBorder;
                int localY = RNG.Next(maxY) + GridBorder;

                // And we get the actual world position :)
                int worldX = gridTileWidth * gridX + localX;
                int worldY = gridTileHeight * gridY + localY;

                // We create the room
                _rooms[gridX, gridY] = new(new(worldX, worldY, width, height));

                // So later on we will want to check if all rooms lead to 0,0 (Source), so we set this boolean here if we are at the source
                if (gridX == 0 && gridY == 0)
                    _rooms[gridX, gridY].LeadsToSource = true;

                // Because we got here it means we generated a valid room, so we add it to the valid rooms list
                validRooms.Add(new(gridX, gridY));
            });
            */

            /*
            // Here we will generate the paths between the rooms
            EnumerateGrid((gridX, gridY) =>
            {
                // Because both the upward and right path generation func will use a source room we gat that first
                DungeonRoom sourceRoom = _rooms[gridX, gridY];

                // First generate upward path
                if (gridY != _gridHeight - 1 && RNG.NextFloat() < PathPercentage)
                {
                    // We get the destination room
                    DungeonRoom destRoom = _rooms[gridX, gridY + 1];

                    // Because we generate a path along the Y, we need to randomize the originX and destinationX, we do that based on said rooms size :)
                    int originX = RNG.Next(sourceRoom.Body.Width) + sourceRoom.Body.Left;
                    int destinationX = RNG.Next(destRoom.Body.Width) + destRoom.Body.Left;

                    // Now we get the delta between the top rooms bottom Y and the bottom rooms top Y.
                    int deltaY = destRoom.Body.Bottom - sourceRoom.Body.Top;

                    // Using said delta we can now generate an index for the path to turn on. See this as at what Y level we will travel along the X axis to get to destinationX
                    int turnIndex = RNG.Next(deltaY - 2) + 1;

                    // Now using said data we create a path :)
                    Path pathLine = new(new(originX, sourceRoom.Body.Top), new(destinationX, destRoom.Body.Bottom), PathDirection.Vertical, turnIndex);

                    // We set the path on both rooms
                    // TODO: We might wanna make pathline a class here, to avoid creating two copies, Or maybe store the paths somewhere else and store a path handle (index into an array?) instead :)
                    sourceRoom.Paths[Path.PATH_TOP] = pathLine;
                    destRoom.Paths[Path.PATH_BOTTOM] = pathLine;

                    // And now if the source room leads to the source, the destination room does aswell!
                    destRoom.LeadsToSource = sourceRoom.LeadsToSource;
                }

                // No do the right
                // We don't comment here as it's the exact same thing as for the upward path but on the x axis :)
                if (gridX != _gridWidth - 1 && RNG.NextFloat() < PathPercentage)
                {
                    DungeonRoom destRoom = _rooms[gridX + 1, gridY];

                    int originY = RNG.Next(sourceRoom.Body.Height) + sourceRoom.Body.Bottom;
                    int destinationY = RNG.Next(destRoom.Body.Height) + destRoom.Body.Bottom;

                    int deltaX = destRoom.Body.Left - sourceRoom.Body.Right;

                    int turnIndex = RNG.Next(deltaX - 2) + 1;

                    Path pathLine = new(new(sourceRoom.Body.Right, originY), new(destRoom.Body.Left, destinationY), PathDirection.Horizontal, turnIndex);
                    sourceRoom.Paths[Path.PATH_RIGHT] = pathLine;
                    destRoom.Paths[Path.PATH_LEFT] = pathLine;

                    destRoom.LeadsToSource = sourceRoom.LeadsToSource;
                }
            });
            */

            /*
            // Validate the map, so that each room leads to 0, 0. Or "Source"
            EnumerateGrid((gridX, gridY) =>
            {
                if (!_rooms[gridX, gridY].LeadsToSource)
                {
                    int direction = RNG.Next(4);

                    for(int i = 0; i < 4; i++)
                    {
                        //if()
                    }
                }
            });
            */
        }
        
        /*
        private void EnumerateGrid(Action<int, int> enumerationFunc)
        {
            for (int gridX = 0; gridX < _gridWidth; gridX++)
            {
                for (int gridY = 0; gridY < _gridHeight; gridY++)
                {
                    enumerationFunc.Invoke(gridX, gridY);
                }
            }
        }

        /*
        bool IGenerator.IsTileAir(Point8 point)
        {
            int gridX = (int)(point.X / (float)Dungeon.WIDTH  * _gridWidth);
            int gridY = (int)(point.Y / (float)Dungeon.HEIGHT * _gridHeight);

            return _rooms[gridX, gridY].Contains(point, true) == true;
        }
        */
    }
}
