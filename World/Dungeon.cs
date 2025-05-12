using DemoRogue.World.Building;
using DemoRogue.World.Generation;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Clockwork.Retro.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World
{
    public sealed class Dungeon
    {
        // Constants
        public const int WIDTH = 128;
        public const int HEIGHT = 128;

        public const int SECTOR_TILE_AREA = 4;
        public const int SECTOR_SIZE = SECTOR_TILE_AREA * SECTOR_TILE_AREA;

        public const int SECTORS_X = WIDTH  / SECTOR_TILE_AREA;
        public const int SECTORS_Y = HEIGHT / SECTOR_TILE_AREA;

        public const int SECTOR_PIXEL_AREA = SECTOR_TILE_AREA * Tileset.TILE_PIXEL_AREA;

        public const int VISIBLE_SECTORS_X = 8;
        public const int VISIBLE_SECTORS_Y = 8;

        public const int VISIBLE_SECTORS_X_HALF = 4;
        public const int VISIBLE_SECTORS_Y_HALF = 4;

        public const int GRID_BORDER = 2;


        // Statics
        private static byte[]? _tileTextures;
        private static Dictionary<byte, byte> _tileLookUp = new() { { 0, 80 }, { 2, 97 }, { 8, 96 }, { 10, 65 }, { 11, 33 }, { 16, 98 }, { 18, 66 }, { 22, 34 }, { 24, 48 }, { 26, 130 }, { 27, 181 }, { 30, 179 }, { 31, 18 }, { 64, 99 }, { 66, 49 }, { 72, 64 }, { 74, 129 }, { 75, 178 }, { 80, 67 }, { 82, 131 }, { 86, 180 }, { 88, 128 }, { 90, 112 }, { 91, 192 }, { 94, 193 }, { 95, 144 }, { 104, 32 }, { 106, 182 }, { 107, 17 }, { 120, 177 }, { 122, 195 }, { 123, 147 }, { 126, 209 }, { 127, 161 }, { 208, 35 }, { 210, 190 }, { 214, 19 }, { 216, 183 }, { 218, 194 }, { 219, 208 }, { 222, 145 }, { 223, 162 }, { 248, 16 }, { 250, 146 }, { 251, 160 }, { 254, 163 }, { 255, 0 } };


        // Values
        private readonly DungeonBuilder _builder;


        // Map data
        private byte[,] _collisionMap = new byte[(WIDTH / 8), HEIGHT];

        private Chunk[,] _chunks = null!;
        private Path[] _paths = null!;
        private Point8[] _validSpawnChunks = null!;


        // Properties
        public int GridWidth => _chunks.GetLength(0);
        public int GridHeight => _chunks.GetLength(1);

        public int ChunkTileWidth => WIDTH / GridWidth;
        public int ChunkTileHeight => HEIGHT / GridHeight;


        // Constructor
        public Dungeon() => _builder = new(this, 6, 6);


        // Static Func
        public static void SetTileset(byte[] textures)
        {
            if (textures.Length != 13)
                throw new ArgumentOutOfRangeException(nameof(textures)); // TODO: better exception

            _tileTextures = textures;
        }


        // Getters
        public Chunk GetChunk(int chunkX, int chunkY) => _chunks[chunkX, chunkY];
        public Chunk GetChunk(Point8 position) => GetChunk(position.X, position.Y);

        public Path GetPath(byte pathIndex) => _paths[pathIndex];

        public Point8 GenerateValidSpawn()
        {
            int index = RNG.Next(_validSpawnChunks.Length);

            Chunk chunk = GetChunk(_validSpawnChunks[index]);

            if (chunk.RoomBody == null)
                throw new InvalidOperationException("Somehow, the valid spawn room, was not a valid spawn room?!?! :(");

            int x = RNG.Next(chunk.RoomBody.Value.Left, chunk.RoomBody.Value.Right);
            int y = RNG.Next(chunk.RoomBody.Value.Bottom, chunk.RoomBody.Value.Top);

            return new(x, y);
        }


        // Func
        public void GenerateFloor(int floor, IGenerator generator)
        {
            // First we update the grid in the builder
            _builder.SetGrid(generator.GridWidth, generator.GridHeight);

            // We use the generator to fill up the builder
            generator.Generate(_builder);

            // We get the data from the builder
            (Chunk[,] chunks, Path[] paths, Point8[] validSpawnChunks) = _builder.Build();

            // We set the data here
            _chunks = chunks;
            _paths = paths;
            _validSpawnChunks = validSpawnChunks;

            // And now we generate a collision map

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    int chunkX = Math.Clamp(x / _builder.ChunkTileWidth, 0, _builder.GridWidth - 1);
                    int chunkY = Math.Clamp(y / _builder.ChunkTileHeight, 0, _builder.GridHeight - 1);

                    bool flag = GetChunk(chunkX, chunkY).IsTileOpen(x, y);

                    SetTileCollisionFlag(x, y, flag);
                }
            }
        }


        public bool IsTileOpen(int x, int y)
        {
            if(x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
                return false;

            int packedX = x / 8;
            int bit = x % 8;
            int shift = 7 - bit % 8;

            return (_collisionMap[packedX, y] & (0b1 << shift)) != 0;
        }
        public bool IsTileOpen(Point8 p) => IsTileOpen(p.X, p.Y);

        private void SetTileCollisionFlag(int x, int y, bool value)
        {
            int packedX = x / 8;
            int bit = x % 8;
            int shift = 7 - bit % 8;

            if (value)
                _collisionMap[packedX, y] |= (byte)(1 << shift);
            else
                _collisionMap[packedX, y] &= (byte)~(1 << shift);
        }
        //private void SetTile(Point8 p) => IsTileOpen(p.X, p.Y);


        internal void BuildSector(int x, int y, int tX, int tY, Tilemap tilemap)
        {
            bool fillZero = x < 0 || x >= SECTORS_X || y < 0 || y >= SECTORS_Y;
            
            int worldX = x * SECTOR_TILE_AREA;
            int worldY = y * SECTOR_TILE_AREA;

            for (int localY = 0; localY < SECTOR_TILE_AREA; localY++)
            {
                int layerY = worldY + localY;
                int tileMapY = tY + localY;

                for (int localX = 0; localX < SECTOR_TILE_AREA; localX++)
                {
                    int layerX = worldX + localX;
                    int tileMapX = tX + localX;

                    if (fillZero || IsTileOpen(worldX + localX, worldY + localY))
                    {
                        tilemap.Set(tileMapX, tileMapY, 0, 0, 0, fillZero ? PaletteIndex.Palette0 : PaletteIndex.Palette1);
                        continue;
                    }

                    (byte texture, TileTransform transform) = GetTileState(layerX, layerY);

                    tilemap.Set(tileMapX, tileMapY, 0, texture, transform, PaletteIndex.Palette0);
                }
            }
            
        }

        private (byte, TileTransform) GetTileState(int x, int y)
        {
            if (_tileTextures == null)
                throw new InvalidOperationException();

            byte packedTile = _tileLookUp[GetNeighbourMask(x, y)];

            byte tileIndex = (byte)(packedTile >> 4);
            TileTransform transform = (TileTransform)(packedTile & 0xF);

            return (tileIndex != 0 ? _tileTextures[tileIndex - 1] : (byte)0, transform);
        }
        private byte GetNeighbourMask(int x, int y)
        {
            byte mask = 0;

            int i = 0;
            for (int localY = -1; localY <= 1; localY++)
            {
                for (int localX = -1; localX <= 1; localX++)
                {
                    if (localX == 0 && localY == 0)
                        continue;

                    if (!IsTileOpen(x + localX, y + localY))
                        mask |= (byte)(0b1 << i);

                    i++;
                }
            }

            return SanitizeNeighbourMask(mask);
        }

        private static byte SanitizeNeighbourMask(byte mask)
        {
            const byte BOTTOM_LEFT = 1 << 0;
            const byte BOTTOM = 1 << 1;
            const byte BOTTOM_RIGHT = 1 << 2;
            const byte LEFT = 1 << 3;
            const byte RIGHT = 1 << 4;
            const byte TOP_LEFT = 1 << 5;
            const byte TOP = 1 << 6;
            const byte TOP_RIGHT = 1 << 7;

            // Clear each corner if its adjacent bits aren't set
            if ((mask & (BOTTOM | LEFT)) != (BOTTOM | LEFT))
                mask &= unchecked((byte)~BOTTOM_LEFT);

            if ((mask & (BOTTOM | RIGHT)) != (BOTTOM | RIGHT))
                mask &= unchecked((byte)~BOTTOM_RIGHT);

            if ((mask & (TOP | LEFT)) != (TOP | LEFT))
                mask &= unchecked((byte)~TOP_LEFT);

            if ((mask & (TOP | RIGHT)) != (TOP | RIGHT))
                mask &= unchecked((byte)~TOP_RIGHT);

            return mask;
        }
    }
}
