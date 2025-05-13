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
        public readonly Dungeon Dungeon = new Dungeon();

        private Entity[] _entities = new Entity[Entity.MAX];

        private List<byte> _activeEntities = [];
        private Queue<byte> _freeEntities = new();


        // Constructor
        internal EntityManager(Dungeon dungeon)
        {
            // Fill up the queue with free entities
            for(byte i = 0; i < Entity.MAX; i++)
            {
                _freeEntities.Enqueue(i);
                _entities[i] = new(this, i);
            }
        }


        // Func
        internal void TickEntities()
        {
            foreach (byte index in _activeEntities)
                _entities[index].Tick();
        }

        public Entity Instantiate(Point8 position, EntityTypes type)
        {
            if (_freeEntities.Count == 0)
                throw new OutOfMemoryException($"Entity.MAX ({Entity.MAX}) reached!");

            byte index = _freeEntities.Dequeue();

            Entity entity = _entities[index];

            entity.SetActive(true);
            entity.SetPosition(position);
            entity.SetType(type);

            _activeEntities.Add(index);

            return entity;
        }

        public 

        public void Destroy(byte index)
        {
            _activeEntities.Remove(index);
            _freeEntities.Enqueue(index);

            _entities[index].SetActive(false);
        }
    }
}
