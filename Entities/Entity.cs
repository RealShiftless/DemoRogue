using DemoRogue.Entities.Types;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.Entities
{
    public sealed class Entity
    {
        // Constants
        public const int MAX = 60;


        // Values
        public readonly EntityManager Manager;
        public readonly byte Id;

        private bool _isActive = false;

        private EntityTypes _type;
        private Point8 _position;


        // Properties
        public bool IsActive => _isActive;

        public IEntityType Type => _type.ToObject();


        // Constructor
        internal Entity(EntityManager manager, byte id)
        {
            Manager = manager;
            Id = id;
        }


        // Func
        internal void Tick()
        {

        }

        internal void Dispose()
        {
            _isActive = false;
        }


        // Setters
        internal void SetActive(bool isActive)
        {
            _isActive = isActive;

            if (IsActive)
            {
                Type.Initialize(this);
            }
            else
                Type.Dispose(this);
        }
        internal void SetType(EntityTypes type) => _type = type;
        
        public void SetPosition(Point8 position) => _position = position;
    }
}
