using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Rooms.Types
{
    public sealed class DummyRoomType : IRoomType
    {
        bool IRoomType.IsValidSpawn => false;
    }
}
