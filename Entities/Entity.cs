using DemoRogue.Entities.Types;
using DemoRogue.Scenes;
using DemoRogue.World;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Mathematics;

namespace DemoRogue.Entities
{
    public delegate void EntitySectorChangedEventHandler(EntitySectorChangedEventArgs args);

    public sealed class Entity
    {
        // Constants
        public const int MAX = 60;

        public const int ANIMATION_FPS = 12;
        internal const float ANIMATION_DELAY = 1f / ANIMATION_FPS;


        // Values
        public readonly EntityManager Manager;
        public readonly byte Id;

        private bool _isActive = false;

        private EntityType _type = null!;
        private Point8 _position;
        private Point8 _sector;
        private Point8 _chunk;


        // Properties
        public bool IsActive => _isActive;

        public EntityType Type => _type;
        public Point8 Position
        {
            get => _position;
            set => SetPosition(value);
        }
        public Point8 Sector => _sector;

        public Dungeon Dungeon => Manager.Dungeon;
        public DungeonScene Scene => Dungeon.Scene;
        public Game Game => Dungeon.Game;


        // Events
        public event EntitySectorChangedEventHandler? SectorChanged;


        // Constructor
        internal Entity(EntityManager manager, byte id)
        {
            Manager = manager;
            Id = id;
        }


        // Func
        internal void UpdateSprite()
        {
            Game.Tilemap.Set(_position, LayerIndex.Tilemap1, Type.AnimationFrameIndices[Scene.CurrentEntityAnimationFrame % Type.AnimationFrames], null, Type.Palette);
        }
        internal bool Tick()
        {
            EntityTickEventArgs args = new();
            Type.Tick(this, ref args);

            return args.IsHandled;
        }


        // Setters
        internal void Initialize(EntityType type, Point8 position)
        {
            // First set the needed values
            _type = type;
            _position = position;
            _isActive = true;

            // Calculate some stuff
            _sector = _position / Dungeon.SECTOR_TILE_AREA;
            _chunk = _position / (Dungeon.GridWidth, Dungeon.GridHeight);

            // Bind to the chunk
            Dungeon.GetChunk(_chunk).BindEntity(Id);

            // Update the tilemap
            // TODO: Only update tilemap if visisble :)
            UpdateSprite();

            // And initialize the entity using it's type
            Type.Initialize(this);
        }
        internal void Dispose()
        {
            _isActive = false;
        }

        public void SetPosition(Point8 position)
        {
            // Update the tilemap
            Game.Tilemap.Set(_position, LayerIndex.Tilemap1, 0);

            _position = position;

            Point8 oldSector = _sector;
            _sector = _position / Dungeon.SECTOR_TILE_AREA;
            if (oldSector != _sector)
                SectorChanged?.Invoke(new(this, _sector, oldSector));

            Point8 oldChunk = _chunk;
            _chunk = _position / (Dungeon.ChunkTileWidth, Dungeon.ChunkTileHeight);
            if (oldChunk != _chunk)
            {
                Dungeon.GetChunk(oldChunk).UnbindEntity(Id);
                Dungeon.GetChunk(_chunk).BindEntity(Id);
            }

            UpdateSprite();
        }
    }
}
