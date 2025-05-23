﻿using DemoRogue.Entities;
using DemoRogue.World.Rooms;
using Shiftless.Common.Mathematics;

namespace DemoRogue.World
{
    public readonly struct Chunk(Dungeon dungeon, Rect8? roomBody, RoomType? type, byte[] paths)
    {
        // Constants
        public const int BORDER = 2;


        // Values
        public readonly Dungeon Dungeon = dungeon;

        public readonly Rect8? RoomBody = roomBody;
        public readonly RoomType? RoomType = type;

        private readonly byte[] _paths = paths;

        private readonly List<byte> _entities = [];


        // Properties
        public bool ContainsRoom => RoomBody != null;

        public int PathCount => _paths.Length;
        public int EntityCount => _entities.Count;


        // Func
        /// <summary>
        /// Gets wether or not a tile is occupied by an entity.
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public readonly bool IsTileOccupied(int x, int y)
        {
            foreach (byte entityId in _entities)
            {
                Entity entity = Dungeon.Entities.GetEntity(entityId);

                if (entity.Position.X == x && entity.Position.Y == y)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if a tile at a position is an air tile, or not a wall.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public readonly bool IsTileAir(int x, int y)
        {
            if (RoomBody?.Contains(x, y) == true)
                return true;

            foreach (byte pathIndex in _paths)
                if (Dungeon.GetPath(pathIndex).Contains(new(x, y)))
                    return true;

            return false;
        }
        public readonly bool IsTileAir(Point8 position) => IsTileAir(position.X, position.Y);

        internal readonly void BindEntity(byte index) => _entities.Add(index);
        internal readonly void UnbindEntity(byte index) => _entities.Remove(index);
    }
}
