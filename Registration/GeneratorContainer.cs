using DemoRogue.World.Generation;
using Shiftless.Common.Registration;

namespace DemoRogue.Registration
{
    public sealed class GeneratorContainer
    {
        public readonly DefaultGenerator Default;

        internal GeneratorContainer(RegistryBuilder registry)
        {
            Default = registry.Register("generators.default", new DefaultGenerator());
        }
    }
}
