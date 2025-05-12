using DemoRogue.States;
using DemoRogue.World;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shiftless.Clockwork.Retro;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue
{
    public sealed class Game : GameBase
    {
        // This basically just stores the ascii values for the supported characters by the sprite font [A-Z0-9@]
        private byte[] _fontTextures = [
                97,
                98,
                99,
                100,
                101,
                102,
                103,
                104,
                105,
                106,
                107,
                108,
                109,
                110,
                111,
                112,
                113,
                114,
                115,
                116,
                117,
                118,
                119,
                120,
                121,
                122,
                49,
                50,
                51,
                52,
                53,
                54,
                55,
                56,
                57,
                48,
                64,
                46,
                33,
                64
            ];

        private Dungeon _layer;

        private GameState _gameState;

        // Constructor
        public Game() : base(new() { WindowTitle = "Demo Rogue" })
        {
            _gameState = new DungeonState();

            Debug.WriteLine(Marshal.SizeOf<Point8>());
            Debug.WriteLine(Marshal.SizeOf<Vector2i>());

            _layer = new Dungeon();
        }


        // Entry
        public static void Main(string[] args)
        {
            Game program = new();
            program.Run();


        }


        // Func
        protected override void Load()
        {
            Tileset.SetTextureOcupiedFlag(0, true);
            Tileset.LoadTextures(@"textures\font", _fontTextures);
            Dungeon.SetTileset(Tileset.LoadTextures(@"textures\walls"));

            _gameState.Initialize(this);

            Input.Up = Keys.Up;
            Input.Down = Keys.Down;
            Input.Left = Keys.Left;
            Input.Right = Keys.Right;
            /*
            Stopwatch sw = Stopwatch.StartNew();
            for(int y = 0; y < 9;  y++)
            {
                for(int x = 0; x < 15; x++)
                {
                    _layer.BuildSector(x, y, x * 4, y * 4, Tilemap);
                }
            }
            sw.Stop();

            Debug.WriteLine(sw.Elapsed.TotalNanoseconds);
            */

            Renderer.SetPalette(PaletteIndex.Palette0, new(
                new(),
                new(0x6110a2FF),
                new(0x9241f3FF),
                new(0xa271ffFF)));

            Renderer.SetPalette(PaletteIndex.Palette1, new(
                new(0x3c0a64FF),
                new(0x6110a2FF),
                new(0x9241f3FF),
                new(0xa271ffFF)));

            Renderer.SetPalette(PaletteIndex.Palette2, new(
                new(0xFF00FFFF),
                new(0x6110a2FF),
                new(0xff61b2FF),
                new(0xffcbbaFF)
                ));

            Renderer.SetPalette(PaletteIndex.Palette3, new(
                new(0xFF00FFFF),
                new(0xFF00FFFF),
                new(0xFF00FFFF),
                new(0xffffffFF)
                ));

            GC.Collect();
        }


        protected override void Update(float deltaTime)
        {
            _gameState.Update(deltaTime);
        }

        protected override void Tick()
        {
            _gameState.Tick();
        }

        public void WriteString(string str, byte x, byte y, byte layer, byte clearLength = 2)
        {
            for(int i = 0; i < str.Length + clearLength; i++)
                Tilemap.SetTile((byte)(x + i), y, layer, 0);

            for (int i = 0; i < str.Length; i++)
            {
                byte c = (byte)str[i];
                Tilemap.SetTile((byte)(x + i), y, layer, c);
                Tilemap.Set((byte)(x + i), y, layer, c, null, PaletteIndex.Palette3);
            }

            
        }
    }
}
