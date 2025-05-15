using DemoRogue.Entities.Types;
using Shiftless.Common.Registration;

namespace DemoRogue.Registration
{
    public sealed class EntityTypeContainer
    {
        public readonly PlayerType Player;
        public readonly SlimeType Slime;

        internal EntityTypeContainer(RegistryBuilder registry)
        {
            Player = registry.Register("entities.player", new PlayerType());
            Slime = registry.Register("entities.slime", new SlimeType());
        }
    }
}
