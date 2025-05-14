using Shiftless.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Rooms
{
    public abstract class RoomType : RegistryItem
    {
        public virtual bool AllowsPaths => true;

        public virtual bool IsValidSpawn => true;
    }
}
