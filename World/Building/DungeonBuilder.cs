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

        private List<Point8> _validSpawnChunks = [];

        private int _chunkTileWidth;
        private int _chunkTileHeight;

        private int _maxRoomWidth;
        private int _maxRoomHeight;


        // Properties
        public int GridWidth => _chunks.GetLength(0);
        public int GridHeight => _chunks.GetLength(1);

        public int MaxRoomWidth => _maxRoomWidth;
        public int MaxRoomHeight => _maxRoomHeight;

        public int ChunkTileWidth => _chunkTileWidth;
        public int ChunkTileHeight => _chunkTileHeight;


        // Indexer
        public ChunkBuilder this[int gridX, int gridY] => _chunks[gridX, gridY];


        // Constructor
        internal DungeonBuilder(Dungeon dungeon, int gridWidth, int gridHeight)
        {
            Dungeon = dungeon;

            SetGridDirect(gridWidth, gridHeight);
        }


        // Setters
        internal void SetGrid(int width, int height)
        {
            // Check if the size is exactly the same and do an early return to save processing n assigments
            if (GridWidth == width && GridHeight == height) return;

            SetGridDirect(width, height);
        }
        private void SetGridDirect(int width, int height)
        {
            // We create a new chunks array
            _chunks = new ChunkBuilder[width, height];

            // And we create some data for chunk usage
            _chunkTileWidth = Dungeon.WIDTH / width;
            _chunkTileHeight = Dungeon.HEIGHT / height;

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
        public void SetRoom(int gridX, int gridY, Rect8 localBody, RoomType type)
        {
            // First we check if the room already is created
            if (_chunks[gridX, gridY].ContainsRoom)
                throw new InvalidOperationException($"Room at {gridX} {gridY} was already created!");

            // For ease of use we send a local body to this func, this means the body still has to be translated into world space :)
            Rect8 body = localBody.Translate(gridX * _chunkTileWidth, gridY * _chunkTileHeight);

            // And we set the actual room
            _chunks[gridX, gridY].SetRoom(body, type);

            // Add the room as a spawn if it be one
            if (type.IsValidSpawn)
                _validSpawnChunks.Add(new(gridX, gridY));
        }
        public void SetRoom(Point8 gridPosition, Rect8 localBody, RoomType type) => SetRoom(gridPosition.X, gridPosition.Y, localBody, type);

        public ChunkBuilder GetChunk(int x, int y) => _chunks[x, y];
        public ChunkBuilder GetChunk(Point8 position) => GetChunk(position.X, position.Y);

        private void CreatePath(ChunkBuilder sourceChunk, Point8 sourcePos, ChunkBuilder destChunk, Point8 destPos, Direction direction, int turnIndex)
        {
            // We create and add the path
            Path path = new(sourcePos, destPos, direction.ToAxis(), turnIndex);
            byte pathId = (byte)_paths.Count;

            _paths.Add(path);

            // And now we go over each chunk in this direction and add it to the ting until we get to the dest pos
            // Because if we do this, all rooms in between are null. This means we don't really have to set if the chunk leads to source
            Point8 curGridPos = sourceChunk.GridPosition;
            while ((curGridPos = curGridPos.Translate(direction, 1)) != destChunk.GridPosition)
                _chunks[curGridPos.X, curGridPos.Y].AddPath(pathId);

            // Finally we add the paths to the source and dest
            sourceChunk.AddPath(pathId, destChunk.GridPosition, destChunk.LeadsToSource);
            destChunk.AddPath(pathId, sourceChunk.GridPosition, sourceChunk.LeadsToSource);
        }

        public void CreatePath(Point8 sourcePos, Point8 destPos, Direction direction, int turnIndex)
        {
            // We get the grid positions of the start and origin
            Point8 sourceGridPos = sourcePos / (GridWidth, GridHeight);
            Point8 destGridPos = destPos / (GridWidth, GridHeight);

            // We get the actual chunks
            ChunkBuilder sourceChunk = _chunks[sourceGridPos.X, sourceGridPos.Y];
            ChunkBuilder destChunk = _chunks[destGridPos.X, destGridPos.Y];

            // And we add the path using the func
            CreatePath(sourceChunk, sourcePos, destChunk, destPos, direction, turnIndex);
        }

        public bool CreatePath(Point8 sourceGridPos, Direction direction)
        {
            // Now we get the actual rooms
            ChunkBuilder sourceChunk = _chunks[sourceGridPos.X, sourceGridPos.Y];
            ChunkBuilder? destChunk = GetNextChunkWithRoom(sourceGridPos, direction);

            // If there was no valid destination we return false
            if (destChunk == null)
                return false;

            Axis axis = direction.ToAxis();

            // Get the distance using said axis
            int dist = sourceChunk.RoomBody.DistanceFrom(destChunk.RoomBody, axis);

            // For now I will throw an error when rooms are touching, because they currently shouldn't. Later I will add merged rooms, then i need to implement behaviors for that.
            if (dist < 2)
                throw new NotImplementedException("Merged rooms where not yet implemented!");

            // Now we create the actual path source and destination positions
            // TODO: Add path overlap func, so like regenerate a path if it like really overlaps
            Point8 sourcePos = sourceChunk.GeneratePathPoint(direction);
            Point8 destPos = destChunk.GeneratePathPoint(direction.Invert());

            // And using the distance between the two rooms we generate a turn point
            int turnIndex = RNG.Next(dist - 2) + 1;

            CreatePath(sourceChunk, sourcePos, destChunk, destPos, direction, turnIndex);

            // And we return true because we succesfully created the path :)))
            return true;
        }
        public bool CreatePath(int gridX, int gridY, Direction direction) => CreatePath(new(gridX, gridY), direction);

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

        public void EnumerateGrid(GridEnumerationEventHandler action)
        {
            for (int gridX = 0; gridX < GridWidth; gridX++)
            {
                for (int gridY = 0; gridY < GridHeight; gridY++)
                {
                    action.Invoke(gridX, gridY);
                }
            }
        }

        public void ValidateAllPaths()
        {
            EnumerateGrid((gridX, gridY) =>
            {
                // First we check if the chunk even contains a room
                if (!_chunks[gridX, gridY].ContainsRoom)
                    return;

                // Now we check if it already leads to source
                if (_chunks[gridX, gridY].LeadsToSource)
                    return;

                // Here we check if the chunk allows paths
                if (!_chunks[gridX, gridY].RoomType.AllowsPaths)
                    return;

                if (gridX == 0)
                    CreatePath(gridX, gridY, Direction.Down);
                else
                    CreatePath(gridX, gridY, Direction.Left);
            });
        }

        internal (Chunk[,] chunks, Path[] paths, Point8[] spawnChunks) Build()
        {
            Chunk[,] chunks = new Chunk[GridWidth, GridHeight];
            EnumerateGrid((gridX, gridY) => chunks[gridX, gridY] = _chunks[gridX, gridY].Build());

            Path[] paths = _paths.ToArray();
            Point8[] validSpawnChunks = _validSpawnChunks.ToArray();

            _paths.Clear();
            _validSpawnChunks.Clear();

            return(chunks, paths, validSpawnChunks);
        }
    }
}
