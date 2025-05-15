using DemoRogue.Entities;
using DemoRogue.Entities.Types;
using DemoRogue.Registration;
using DemoRogue.World;
using DemoRogue.World.Generation;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shiftless.Clockwork.Retro.Rendering;
using Shiftless.Common.Mathematics;
using System.Diagnostics;

#nullable disable
namespace DemoRogue.Scenes
{
    public sealed class DungeonScene : Scene
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
        private Entity _slimeEntity = null!;

        private float _entityAnimTimer;
        private int _entityAnimFrame;


        // Properties
        public int CurrentEntityAnimationFrame => _entityAnimFrame;


        // Constructor
        internal DungeonScene()
        {
        }


        // State Func
        public override void Load()
        {
            // Set the dungeon up
            _dungeon = new(this);

            //_playerTextures = Game.Tileset.LoadTextures(@"textures\player_anim");


            Stopwatch sw = Stopwatch.StartNew();
            GenerateNextFloor();
            _playerEntity = _dungeon.Entities.Instantiate(Point8.Zero, GameRegistry.Entities.Player);
            _playerEntity.SectorChanged += (args) => RefreshVisibleTiles();

            _slimeEntity = _dungeon.Entities.Instantiate(Point8.Zero, GameRegistry.Entities.Slime);

            _slimeEntity.SetPosition(_dungeon.GenerateValidSpawn());
            _playerEntity.SetPosition(_dungeon.GenerateValidSpawn());
            RefreshVisibleTiles();
            sw.Stop();

            PlayerType.SetCamOffset(_playerEntity);

            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        public override void Update(float deltaTime)
        {
            if (Game.Window.KeyboardState.IsKeyPressed(Keys.R))
            {
#if DEBUG
                Stopwatch sw = Stopwatch.StartNew();
#endif
                GenerateNextFloor();

                _playerEntity.SetPosition(_dungeon.GenerateValidSpawn());

                Point8 slimeSpawn = _dungeon.GenerateValidSpawn();
                _slimeEntity.SetPosition(_playerEntity.Position);
                RefreshVisibleTiles();

                PlayerType.SetCamOffset(_playerEntity);
#if DEBUG
                sw.Stop();

                Debug.WriteLine($"instantiated slime at {_slimeEntity.Position}");
                Debug.WriteLine($"Generation finished in {sw.Elapsed.TotalMilliseconds}ms!");
#endif
            }

            // Do the entity animation
            _entityAnimTimer += Game.Time.Delta;
            if (_entityAnimTimer >= Entity.ANIMATION_DELAY)
            {
                _entityAnimFrame++;
                _entityAnimTimer = 0;


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
