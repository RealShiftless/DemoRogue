using Shiftless.Common.Mathematics;

namespace DemoRogue.Entities
{
    public readonly struct EntitySectorChangedEventArgs(Entity sender, Point8 newSector, Point8 oldSector)
    {
        public readonly Entity Sender = sender;
        public readonly Point8 NewSector = newSector;
        public readonly Point8 OldSector = oldSector;
    }
}
