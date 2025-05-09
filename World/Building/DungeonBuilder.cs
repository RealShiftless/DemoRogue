using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Building
{
    public delegate void GridEnumerationEventHandler(int gridX, int gridY);

    public sealed class DungeonBuilder
    {
        // Values
        public readonly Dungeon Dungeon;

        private ChunkBuilder[,] _chunks = null!;
        private List<Path> _paths = [];

        private int _chunkTileWidth;
        private int _gridTileHeight;

        private int _maxRoomWidth;
        private int _maxRoomHeight;


        // Properties
        public int GridWidth => _chunks.GetLength(0);
        public int GridHeight => _chunks.GetLength(1);

        public int MaxRoomWidth => _maxRoomWidth;
        public int MaxRoomHeight => _maxRoomHeight;


        // Indexer
        public ChunkBuilder this[int gridX, int gridY] => _chunks[gridX, gridY];


        // Constructor
        internal DungeonBuilder(Dungeon dungeon, int gridWidth, int gridHeight)
        {
            Dungeon = dungeon;

            SetGrid(gridWidth, gridHeight);
        }


        // Setters
        internal void SetGrid(int width, int height)
        {
            // Check if the size is exactly the same and do an early return to save processing n assigments
            if (GridWidth == width && GridHeight == height) return;

            // We create a new chunks array
            _chunks = new ChunkBuilder[width, height];

            // And we create some data for chunk usage
            _chunkTileWidth = Dungeon.WIDTH / width;
            _gridTileHeight = Dungeon.HEIGHT / height;

            _maxRoomWidth = Dungeon.WIDTH / width - Dungeon.GRID_BORDER * 2;
            _maxRoomHeight = Dungeon.HEIGHT / height - Dungeon.GRID_BORDER * 2;

            // And here we initialize all chunk builders :)
            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    bool isSource = x == 0 && y == 0;

                    _chunks[x, y] = new(this, new(x, y), isSource);
                }
            }
        }


        // Func
        public void SetRoom(int gridX, int gridY, Rect8 localBody, RoomTypes type)
        {
            // First we check if the room already is created
            if (_chunks[gridX, gridY] != null)
                throw new InvalidOperationException($"Room at {gridX} {gridY} was already created!");

            // For ease of use we send a local body to this func, this means the body still has to be translated into world space :)
            Rect8 body = localBody.Translate(gridX * _chunkTileWidth, gridY * _gridTileHeight);

            // And we set the actual room
            _chunks[gridX, gridY].SetRoom(body, type);
        }
        public void CreateRoom(Point8 gridPosition, Rect8 localBody, RoomTypes type) => SetRoom(gridPosition.X, gridPosition.Y, localBody, type);

        public bool CreatePath(Point8 sourceGridPos, Direction direction)
        {
            // Now we get the actual rooms
            ChunkBuilder source = _chunks[sourceGridPos.X, sourceGridPos.Y];
            ChunkBuilder? dest = GetNextChunkWithRoom(sourceGridPos, direction);

            // If there was no valid destination we return false
            if (dest == null)
                return false;

            // Get the axis
            Axis axis = direction.ToAxis();

            // Get the distance using said axis
            int dist = source.RoomBody.DistanceFrom(dest.RoomBody, axis);

            // For now I will throw an error when rooms are touching, because they currently shouldn't. Later I will add merged rooms, then i need to implement behaviors for that.
            if (dist < 2)
                throw new NotImplementedException("Merged rooms where not yet implemented!");

            // Now we create the actual path source and destination positions
            // TODO: Add path overlap func, so like regenerate a path if it like really overlaps
            Point8 sourcePos = source.GeneratePathPoint(direction);
            Point8 destPos = dest.GeneratePathPoint(direction.Invert());

            // And using the distance between the two rooms we generate a turn point
            int turnIndex = RNG.Next(dist - 2) + 1;

            // We create and add the path
            Path path = new(sourcePos, destPos, axis, turnIndex);
            byte pathId = (byte)_paths.Count;

            _paths.Add(path);

            // And now we go over each chunk in this direction and add it to the ting until we get to the dest pos
            // Because if we do this, all rooms in between are null. This means we don't really have to set if the chunk leads to source
            Point8 gridPos = sourceGridPos;
            while ((gridPos = gridPos.Translate(direction, 1)) != dest.GridPosition)
                _chunks[gridPos.X, gridPos.Y].AddPath(pathId);

            // Finally we add the paths to the source and dest
            source.AddPath(pathId);
            dest.AddPath(pathId, source.LeadsToSource);

            // And we return true because we succesfully created the path :)))
            return true;
        }

        private ChunkBuilder? GetNextChunkWithRoom(Point8 gridPos, Direction direction)
        {
            // Here we loop over grid pos until we get a chunk with a room
            // We first translate by 1 into Direction direction because we dont have to return the chunk we are in :)
            gridPos = gridPos.Translate(direction, 1);
            while (!_chunks[gridPos.X, gridPos.Y].ContainsRoom)
            {
                // We translate
                gridPos = gridPos.Translate(direction, 1);

                // If we went out of bounds we return null
                if (gridPos.X < 0 || gridPos.Y < 0 || gridPos.X >= GridWidth || gridPos.Y >= GridHeight)
                    return null;
            }

            // And now we just return
            return _chunks[gridPos.X, gridPos.Y];
        }

        public void CreatePath(int gridX, int gridY, Direction direction)
        {
        }

        public void EnumerateGrid(GridEnumerationEventHandler action)
        {
            for (int gridX = 0; gridX < _chunkTileWidth; gridX++)
            {
                for (int gridY = 0; gridY < _gridTileHeight; gridY++)
                {
                    action.Invoke(gridX, gridY);
                }
            }
        }
    }
}
