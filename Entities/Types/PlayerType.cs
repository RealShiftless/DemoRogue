using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Clockwork.Retro.Rendering;
using Shiftless.Common.Mathematics;

namespace DemoRogue.Entities.Types
{
    public sealed class PlayerType : EntityType
    {
        // Constants
        public const int MOVE_TICKS = 4;


        // Static
        private static Vec2i _camOffset = new(-(Renderer.NATIVE_WIDTH / 2) + (Tileset.TILE_PIXEL_AREA / 2), -(Renderer.NATIVE_HEIGHT / 2) + (Tileset.TILE_PIXEL_AREA / 2));


        // Values
        private byte[] _animationFrames = null!;

        private int _moveTimer = 0;


        // Properties
        public override byte[] AnimationFrameIndices => _animationFrames;

        public override PaletteIndex Palette => Game.PLAYER_PALETTE;

        public override byte BaseAttack => 80;
        public override byte BaseDefense => 80;


        // Func
        public override void Load(Game game)
        {
            _animationFrames = game.Tileset.LoadTextures(@"textures\player_anim");
        }

        public override void Initialize(Entity entity)
        {
        }
        public override void Tick(Entity entity, ref EntityTickEventArgs args)
        {
            Vec2i input = new(entity.Game.Input.HorizontalI, entity.Game.Input.VerticalI);

            if (input != Vec2i.Zero && (_moveTimer == 0 || entity.Game.Window.KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftShift)))
            {
                Point8 nextPos = entity.Position + input;

                /*
                if(input.X != 0 && input.Y != 0)
                {
                    if (!entity.Dungeon.IsTileOpen(nextPos.X, entity.Position.Y) || !entity.Dungeon.IsTileOpen(entity.Position.X, nextPos.Y) || !entity.Dungeon.IsTileOpen(nextPos))
                        return;
                }
                else if(!entity.Dungeon.IsTileOpen(nextPos))
                {
                    return;
                }
                */

                // Update the players position
                entity.SetPosition(nextPos);
                SetCamOffset(entity);

                entity.Dungeon.Game.WriteString($"plr pos {entity.Position.X:d3} {entity.Position.Y:d3}", 0, 0, LayerIndex.Tilemap3);

                // And set the move timer
                _moveTimer = MOVE_TICKS;
            }

            if (input == Vec2i.Zero)
                _moveTimer = 0;
            else if (_moveTimer != 0)
                _moveTimer--;
        }

        internal static void SetCamOffset(Entity entity)
        {
            entity.Game.Tilemap.SetOffset(LayerIndex.Tilemap0, Vec2i.Mult(entity.Position, Tileset.TILE_PIXEL_AREA) + _camOffset);
            entity.Game.Tilemap.SetOffset(LayerIndex.Tilemap1, Vec2i.Mult(entity.Position, Tileset.TILE_PIXEL_AREA) + _camOffset);
        }
    }
}
