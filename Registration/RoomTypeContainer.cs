using DemoRogue.World.Rooms.Types;
using Shiftless.Common.Registration;

namespace DemoRogue.Registration
{
    public sealed class RoomTypeContainer
    {
        public readonly DefaultRoomType Default;
        public readonly DummyRoomType Dummy;

        internal RoomTypeContainer(RegistryBuilder registry)
        {
            Default = registry.Register("rooms.default", new DefaultRoomType());
            Dummy = registry.Register("rooms.dummy", new DummyRoomType());
        }
    }
}
