using DemoRogue.Entities;
using DemoRogue.Entities.Types;
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

        
        // Static values
        private static Vec2i _camOffset = new(-(Renderer.NATIVE_WIDTH / 2) + (Tileset.TILE_PIXEL_AREA / 2), -(Renderer.NATIVE_HEIGHT / 2) + (Tileset.TILE_PIXEL_AREA / 2));


        // Values
        private readonly IGenerator _generator = new DefaultGenerator();
        private Dungeon _dungeon = null!;

        private Entity _playerEntity = null!;


        // Constructor
        internal DungeonState()
        {
        }


        // State Func
        public override void Load()
        {
            // Set the dungeon up
            _dungeon = new(Game);

            //_playerTextures = Game.Tileset.LoadTextures(@"textures\player_anim");
            _playerEntity = _dungeon.Entities.Instantiate(Point8.Zero, GameRegistry.Entities.Player);
            _playerEntity.SectorChanged += (args) => RefreshVisibleTiles();

            Stopwatch sw = Stopwatch.StartNew();
            GenerateNextFloor();
            _playerEntity.SetPosition(_dungeon.GenerateValidSpawn());
            RefreshVisibleTiles();
            sw.Stop();

            PlayerType.SetCamOffset(_playerEntity);

            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        public override void Update(float deltaTime)
        {
            if(Game.Window.KeyboardState.IsKeyPressed(Keys.R))
            {
#if DEBUG
                Stopwatch sw = Stopwatch.StartNew();
#endif
                GenerateNextFloor();
                _playerEntity.SetPosition(_dungeon.GenerateValidSpawn());
                RefreshVisibleTiles();

                PlayerType.SetCamOffset(_playerEntity);
#if DEBUG
                sw.Stop();

                Debug.WriteLine($"Generation finished in {sw.Elapsed.TotalMilliseconds}ms!");
#endif

            }

            // Do the entity animation
            if (Game.Time.Frame % Entity.ANIMATION_FPS == 0)
            {
                // Update entities
                // TODO: I don't have entities yet so i just update the player anim here
                _dungeon.Entities.UpdateSprites();
            }

            RefreshVisibleTiles();
        }
        public override void Tick()
        {
            _dungeon.Entities.TickEntities();
        }


        // Func
        

        public void GenerateNextFloor()
        {
            _dungeon.GenerateFloor(0, _generator);
        }

        public void RefreshVisibleTiles()
        {
            for (int x = -Dungeon.VISIBLE_SECTORS_X_HALF; x <= Dungeon.VISIBLE_SECTORS_X_HALF; x++)
            {
                for (int y = -Dungeon.VISIBLE_SECTORS_Y_HALF; y <= Dungeon.VISIBLE_SECTORS_Y_HALF; y++)
                {
                    _dungeon.BuildSector(
                        _playerEntity.Sector.X + x,
                        _playerEntity.Sector.Y + y,
                        (_playerEntity.Sector.X + x) * Dungeon.SECTOR_TILE_AREA,
                        (_playerEntity.Sector.Y + y) * Dungeon.SECTOR_TILE_AREA,
                        Game.Tilemap);
                }
            }
        }
    }
}
#nullable enable
