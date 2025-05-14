using DemoRogue.Entities.Types;
using DemoRogue.World;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.Entities
{
    public delegate void EntitySectorChangedEventHandler(EntitySectorChangedEventArgs args);

    public sealed class Entity
    {
        // Constants
        public const int MAX = 60;

        public const int ANIMATION_FPS = 12;


        // Values
        public readonly EntityManager Manager;
        public readonly byte Id;

        private bool _isActive = false;

        private EntityType _type = null!;
        private Point8 _position;
        private Point8 _sector;

        private int _lastAnimFrame = -1;


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
            Game.Tilemap.Set(_position, 1, Type.AnimationFrameIndices[Game.Time.Frame / ANIMATION_FPS % Type.AnimationFrames], null, Type.Palette);
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
            Game.Tilemap.Set(_position, 1, 0);

            _position = position;

            Point8 oldSector = _sector;
            _sector = _position / Dungeon.SECTOR_TILE_AREA;
            if (oldSector != _sector)
                SectorChanged?.Invoke(new(this, _sector, oldSector));

            UpdateSprite();
        }
    }
}
