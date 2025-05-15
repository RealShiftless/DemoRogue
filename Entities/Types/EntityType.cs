using DemoRogue.Util;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Registration;

namespace DemoRogue.Entities.Types
{
    public abstract class EntityType : RegistryItem, ILoadsResources
    {
        // Animation Values
        public abstract byte[] AnimationFrameIndices { get; }

        public abstract PaletteIndex Palette { get; }


        // Stat Values
        public abstract byte BaseAttack { get; }
        public abstract byte BaseDefense { get; }


        // Properties
        public int AnimationFrames => AnimationFrameIndices.Length;


        // Func
        public abstract void Load(Game game);

        public abstract void Initialize(Entity entity);
        public abstract void Tick(Entity entity, ref EntityTickEventArgs args);

        public virtual void Dispose(Entity entity) { }
    }
}
