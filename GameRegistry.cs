using DemoRogue.Entities.Types;
using DemoRogue.World.Generation;
using DemoRogue.World.Rooms.Types;
using Shiftless.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue
{
    public sealed class GameRegistry : IRegistryInitializer
    {
        public static RoomTypeContainer Rooms { get; private set; } = null!;
        public static GeneratorContainer Generators { get; private set; } = null!;
        public static EntityTypeContainer Entities { get; private set; } = null!;

        void IRegistryInitializer.Initialize(RegistryBuilder registry)
        {
            Rooms = new(registry);
            Generators = new(registry);
            Entities = new(registry);
        }
    }

    public sealed class EntityTypeContainer
    {
        public readonly PlayerType Player;

        internal EntityTypeContainer(RegistryBuilder registry)
        {
            Player = registry.Register("entities.player", new PlayerType());
        }
    }

    public sealed class GeneratorContainer
    {
        public readonly DefaultGenerator Default;

        internal GeneratorContainer(RegistryBuilder registry)
        {
            Default = registry.Register("generators.default", new DefaultGenerator());
        }
    }

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
