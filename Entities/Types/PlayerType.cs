using DemoRogue.World;
using Shiftless.Clockwork.Retro;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Clockwork.Retro.Rendering;
using Shiftless.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override PaletteIndex Palette => PaletteIndex.Palette2;

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

            if (input != Vec2i.Zero && _moveTimer == 0)
            {
                Point8 nextPos = entity.Position + input;

                if(input.X != 0 && input.Y != 0)
                {
                    if (!entity.Dungeon.IsTileOpen(nextPos.X, entity.Position.Y) || !entity.Dungeon.IsTileOpen(entity.Position.X, nextPos.Y) || !entity.Dungeon.IsTileOpen(nextPos))
                        return;
                }
                else if(!entity.Dungeon.IsTileOpen(nextPos))
                {
                    return;
                }

                // Update the players position
                entity.SetPosition(nextPos);
                SetCamOffset(entity);

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
            entity.Game.Tilemap.SetOffset(0, Vec2i.Mult(entity.Position, Tileset.TILE_PIXEL_AREA) + _camOffset);
            entity.Game.Tilemap.SetOffset(1, Vec2i.Mult(entity.Position, Tileset.TILE_PIXEL_AREA) + _camOffset);
        }
    }
}
