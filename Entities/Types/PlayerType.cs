using Shiftless.Clockwork.Retro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.Entities.Types
{
    public sealed class PlayerType : IEntityType
    {
        // Values
        private byte[] _animationFrames = null!;


        // Properties
        byte[] IEntityType.AnimationFrameIndices => _animationFrames;


        // Interface Func
        void IEntityType.Load(Game game)
        {
            _animationFrames = game.Tileset.LoadTextures(@"textures\player");
        }

        void IEntityType.Initialize(Entity entity)
        {

        }
        void IEntityType.Tick(Entity entity)
        {
        }
    }
}
