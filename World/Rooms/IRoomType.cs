using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Rooms
{
    public interface IRoomType
    {
        public bool GeneratesPaths => true;

        //void PlayerEnter();
    }
}
