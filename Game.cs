using DemoRogue.States;
using DemoRogue.Util;
using DemoRogue.World;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shiftless.Clockwork.Retro;
using Shiftless.Clockwork.Retro.Mathematics;
using Shiftless.Common.Registration;
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
        private Registrar _registrar = null!;
        private Registry _registry = null!;

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

        private GameState _gameState;


        // Properties
        public Registrar Registrar => _registrar;
        public Registry Registry => _registry;


        // Constructor
        public Game() : base(new() { WindowTitle = "Demo Rogue" })
        {
            _gameState = new DungeonState();
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
            // Load some default textures
            Tileset.SetTextureOcupiedFlag(0, true);
            Tileset.LoadTextures(@"textures\font", _fontTextures);
            Dungeon.SetTileset(Tileset.LoadTextures(@"textures\walls"));

            // Set the input to the arrow keys
            Input.Up = Keys.Up;
            Input.Down = Keys.Down;
            Input.Left = Keys.Left;
            Input.Right = Keys.Right;

            // Set some palletes
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

            // Initialize the registrar
            _registrar = new((registrar) =>
            {
                _registry = registrar.AddRegistry("core", new GameRegistry());
            });

            // Load the registrar resources
            _registrar.EnumerateItems((item) =>
            {
                if (item is not ILoadsResources resourceLoader)
                    return;

                resourceLoader.Load(this);
            });

            // Initialize the game state
            _gameState.Initialize(this);

            // Finally collect when we still can
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
