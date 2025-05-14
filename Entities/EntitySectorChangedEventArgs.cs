using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.Entities
{
    public readonly struct EntitySectorChangedEventArgs(Entity sender, Point8 newSector, Point8 oldSector)
    {
        public readonly Entity Sender = sender;
        public readonly Point8 NewSector = newSector;
        public readonly Point8 OldSector = oldSector;
    }
}
