using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Building
{
    public sealed class RoomBuilder
    {
        // Values
        private readonly DungeonBuilder _dungeon;
        private bool _isLocked;

        private Rect8 _body;

        private RoomTypes _roomType;

        private bool _leadsToSource = false;

        private List<byte> _paths = [];


        // Properties
        public bool IsLocked => _isLocked;

        public Rect8 Body
        {
            get => _body;
            set
            {
                if (_isLocked)
                    throw new InvalidOperationException($"{nameof(RoomBuilder)} was locked!");

                _body = value;
            }
        }

        public RoomTypes RoomType
        {
            get => _roomType;
            set
            {
                if (_isLocked)
                    throw new InvalidOperationException($"{nameof(RoomBuilder)} was locked!");

                _roomType = value;
            }
        }

        public bool LeadsToSource
        {
            get => _leadsToSource;
            internal set => _leadsToSource = value;
        }


        // Constructor
        internal RoomBuilder(DungeonBuilder dungeon) => _dungeon = dungeon;


        // Func
        internal void AddPath(byte index) => _paths.Add(index);

        
    }
}
