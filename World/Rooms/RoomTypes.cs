using DemoRogue.World.Rooms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.World.Rooms
{
    public enum RoomTypes
    {
        Default
    }

    public static class RoomTypeUtil
    {
        // Room types
        private static DefaultRoomType _defaultRoomType = new();


        // Room Type Tings
        public static IRoomType ToObject(this RoomTypes roomType) => roomType switch
        {
            RoomTypes.Default => _defaultRoomType,

            _ => throw new ArgumentException($"{nameof(roomType)} went unhandled! ({roomType})")
        };

        public static bool AllowsPaths(this RoomTypes roomType) => roomType.ToObject().GeneratesPaths;
    }
}
