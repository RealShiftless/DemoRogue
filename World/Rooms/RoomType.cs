using Shiftless.Common.Registration;

namespace DemoRogue.World.Rooms
{
    public abstract class RoomType : RegistryItem
    {
        public virtual bool AllowsPaths => true;

        public virtual bool IsValidSpawn => true;
    }
}
