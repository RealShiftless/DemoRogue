namespace DemoRogue.Scenes
{
    public abstract class Scene
    {
        // Values
        private Game? _game;


        // Properties
        public Game Game => _game ?? throw new InvalidOperationException($"{nameof(Scene)} was not initialized!");


        // Func
        internal void Initialize(Game game)
        {
            _game = game;
            Load();
        }


        // Virtuals
        public virtual void Load() { }
        public virtual void Awake() { }
        public virtual void Sleep() { }
        public virtual void Unload() { }

        public virtual void Update(float deltaTime) { }
        public virtual void Tick() { }
    }
}
