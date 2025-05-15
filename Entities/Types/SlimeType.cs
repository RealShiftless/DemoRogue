using Shiftless.Clockwork.Retro.Mathematics;

namespace DemoRogue.Entities.Types
{
    public class SlimeType : EntityType
    {
        // Values
        private byte[] _animationFrameIndices = null!;


        // Properties
        public override byte[] AnimationFrameIndices => _animationFrameIndices;

        public override PaletteIndex Palette => Game.AQUA_PALETTE_2;

        public override byte BaseAttack => 30;
        public override byte BaseDefense => 20;


        // Func
        public override void Load(Game game)
        {
            _animationFrameIndices = game.Tileset.LoadTextures(@"textures\slime_anim");
        }

        public override void Initialize(Entity entity)
        {
        }
        public override void Tick(Entity entity, ref EntityTickEventArgs args)
        {
        }
    }
}
