using DemoRogue.Entities.Types;
using DemoRogue.World.Generation;
using DemoRogue.World.Rooms.Types;
using Shiftless.Common.Registration;

namespace DemoRogue.Registration
{
    /* So for my game I want to register different items to something like a string and ID pair
     * For this I have made a Registrar class in my Common library, this regisrar gets initialized with registries, with a registry initializer
     * So here I basically initialize my game registry with the different components I want to use in my game.
     * I made differnt classes for everything so it's easier to use (e.g GameRegistry.Rooms.Default) 
     * These classes are made into different files for easier programming use :) */
    public sealed class GameRegistry : IRegistryInitializer
    {
        // Containers
        public static RoomTypeContainer Rooms { get; private set; } = null!;
        public static GeneratorContainer Generators { get; private set; } = null!;
        public static EntityTypeContainer Entities { get; private set; } = null!;


        // Initialization
        void IRegistryInitializer.Initialize(RegistryBuilder registry)
        {
            Rooms = new(registry);
            Generators = new(registry);
            Entities = new(registry);
        }
    }
}
