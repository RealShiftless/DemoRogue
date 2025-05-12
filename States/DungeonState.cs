using DemoRogue.World;
using DemoRogue.World.Generation;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Clockwork.Retro.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable
namespace DemoRogue.States
{
    internal class DungeonState : GameState
    {
        // Constants
        public const int ANIMATION_FPS = 12;
        public const float ANIMATION_DELAY = 1f / ANIMATION_FPS;

        public const int ANIMATION_FRAMES = 4;

        public const int MOVE_TICKS = 4;


        // Static values
        private static Vec2i _camOffset = new(-(Renderer.NATIVE_WIDTH / 2) + (Tileset.TILE_PIXEL_AREA / 2), -(Renderer.NATIVE_HEIGHT / 2) + (Tileset.TILE_PIXEL_AREA / 2));


        // Values
        private readonly IGenerator _generator = new DefaultGenerator();
        private Dungeon _dungeon = new();

        private int _curFrame;
        private float _animationTime;

        private byte[] _playerTextures;
        private Point8 _playerPos = new(0, 0);
        private bool _hasMoved = false;

        private int _curFloor = -1;

        private Point8 _curPlayerSector = new(0, 0);

        private int _moveTimer;


        // State Func
        public override void Load()
        {
            _playerTextures = Game.Tileset.LoadTextures(@"textures\player_anim");

            Stopwatch sw = Stopwatch.StartNew();
            GenerateNextFloor();
            MovePlayer(_dungeon.GenerateValidSpawn());
            RefreshVisibleTiles();
            sw.Stop();

            Debug.WriteLine(sw.ElapsedMilliseconds);


            Game.Tilemap.Refresh();
        }

        public override void Update(float deltaTime)
        {
            _animationTime += deltaTime;

            if(Game.Window.KeyboardState.IsKeyPressed(Keys.R))
            {
                Stopwatch sw = Stopwatch.StartNew();
                GenerateNextFloor();
                MovePlayer(_dungeon.GenerateValidSpawn());
                RefreshVisibleTiles();
                sw.Stop();

                Debug.WriteLine($"Generation finished in {sw.Elapsed.TotalMilliseconds}ms!");

            }

            if (_animationTime >= ANIMATION_DELAY)
            {
                _animationTime = 0;

                _curFrame++;

                // Update entities
                // TODO: I don't have entities yet so i just update the player anim here
                Game.Tilemap.SetTile(_playerPos, 1, _playerTextures[_curFrame % _playerTextures.Length]);

                Game.Tilemap.Refresh();
            }
        }
        public override void Tick()
        {
            Vec2i input = new(Game.Input.HorizontalI, Game.Input.VerticalI);

            if (input != Vec2i.Zero && _moveTimer == 0)
            {
                Point8 nextPos = _playerPos + input;

                if (!_dungeon.IsTileOpen(nextPos.X, _playerPos.Y) || !_dungeon.IsTileOpen(_playerPos.X, nextPos.Y) || !_dungeon.IsTileOpen(nextPos))
                    return;

                if (MovePlayer(nextPos))
                {
                    RefreshVisibleTiles();
                    Game.Tilemap.Refresh();

                }

                _moveTimer = MOVE_TICKS;
                _hasMoved = true;
            }

            if (input == Vec2i.Zero)
                _moveTimer = 0;
            else if (_moveTimer != 0)
                _moveTimer--;
        }


        // Func
        /// <summary>
        /// Moves the player to a point.
        /// </summary>
        /// <param name="pos">The point.</param>
        /// <returns>True if the player is now in a new sector.</returns>
        public bool MovePlayer(Point8 pos)
        {
            Game.Tilemap.Set(_playerPos, 1, 0, 0, 0);
            _playerPos = pos;

            Game.Tilemap.Set(pos, 1, _playerTextures[_curFrame % _playerTextures.Length], 0, PaletteIndex.Palette2);
            Game.Tilemap.Refresh();


            // Set the camera offset
            Game.Tilemap.SetOffset(0, Vec2i.Mult(pos, Tileset.TILE_PIXEL_AREA) + _camOffset);
            Game.Tilemap.SetOffset(1, Vec2i.Mult(pos, Tileset.TILE_PIXEL_AREA) + _camOffset);

            // Set the sector
            Point8 sector = pos / Dungeon.SECTOR_TILE_AREA;

            Game.WriteString($"plr pos: {_playerPos.X} {_playerPos.Y}", 0, 0, 3);
            Game.WriteString($"plr grd: {(int)(_playerPos.X / 128f * 6)} {(int)(_playerPos.Y / 128f * 6)}", 0, 2, 3);

            if (_curPlayerSector != sector)
            {
                _curPlayerSector = sector;
                Game.WriteString($"plr sec: {_curPlayerSector.X} {_curPlayerSector.Y}", 0, 1, 3);
                return true;
            }

            return false;
        }

        public void GenerateNextFloor()
        {
            _curFloor++;
            _dungeon.GenerateFloor(0, _generator);
        }

        private void LoadNewSectors()
        {
        }
        public void RefreshVisibleTiles()
        {
            for (int x = -Dungeon.VISIBLE_SECTORS_X_HALF; x <= Dungeon.VISIBLE_SECTORS_X_HALF; x++)
            {
                for (int y = -Dungeon.VISIBLE_SECTORS_Y_HALF; y <= Dungeon.VISIBLE_SECTORS_Y_HALF; y++)
                {
                    _dungeon.BuildSector(
                        _curPlayerSector.X + x,
                        _curPlayerSector.Y + y,
                        (_curPlayerSector.X + x) * 4,
                        (_curPlayerSector.Y + y) * 4,
                        Game.Tilemap);
                }
            }
        }
    }
}
#nullable enable
