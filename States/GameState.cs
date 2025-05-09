using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.States
{
    public abstract class GameState
    {
        // Values
        private Game? _game;


        // Properties
        public Game Game => _game ?? throw new InvalidOperationException($"{nameof(GameState)} was not initialized!");


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
