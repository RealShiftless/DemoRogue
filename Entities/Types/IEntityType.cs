namespace DemoRogue.Entities.Types
{
    public interface IEntityType
    {
        byte[] AnimationFrameIndices { get; }
        float AnimationSpeed => 1f;

        void Load(Game game);

        void Initialize(Entity entity);
        void Tick(Entity entity);

        void Dispose(Entity entity) { }
    }
}
