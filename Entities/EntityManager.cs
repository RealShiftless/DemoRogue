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
    public sealed class EntityManager
    {
        // Values
        public readonly Dungeon Dungeon;

        private Entity[] _entities = new Entity[Entity.MAX];

        private List<byte> _activeEntities = [];
        private Queue<byte> _freeEntities = new();


        // Constructor
        internal EntityManager(Dungeon dungeon)
        {
            Dungeon = dungeon;

            // Fill up the queue with free entities
            for(byte i = 0; i < Entity.MAX; i++)
            {
                _freeEntities.Enqueue(i);
                _entities[i] = new(this, i);
            }
        }


        // Func
        internal void UpdateSprites()
        {
            foreach (byte index in _activeEntities)
                _entities[index].UpdateSprite();
        }
        internal void TickEntities()
        {
            foreach (byte index in _activeEntities)
                _entities[index].Tick();
        }

        /// <summary>
        /// Tries to instantiate a new entity.
        /// </summary>
        /// <param name="position">The position of the entity</param>
        /// <param name="type">The type of the entity</param>
        /// <returns>The entity that was instantiated.</returns>
        /// <exception cref="OutOfMemoryException">When we exceed Entity.MAX entities.</exception>
        public Entity Instantiate(Point8 position, EntityType type)
        {
            // First check if we even have any free spots left
            if (_freeEntities.Count == 0)
                throw new OutOfMemoryException($"Entity.MAX ({Entity.MAX}) reached!");

            // Get the index for the entity
            byte index = _freeEntities.Dequeue();

            // Get the entity
            Entity entity = _entities[index];

            // Set it's values
            entity.Initialize(type, position);

            // Add it to the active entities list
            _activeEntities.Add(index);

            // And return said entity
            return entity;
        }

        public Entity GetEntity(byte index) => _entities[index];

        public void Destroy(byte index)
        {
            _activeEntities.Remove(index);
            _freeEntities.Enqueue(index);

            _entities[index].Dispose();
        }
    }
}
