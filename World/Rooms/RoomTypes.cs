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
        Default,
        Dummy
    }

    public static class RoomTypeUtil
    {
        // Room types
        private static DefaultRoomType _defaultRoomType = new();
        private static DummyRoomType _dummyRoomType = new();


        // Room Type Tings
        public static IRoomType ToObject(this RoomTypes roomType) => roomType switch
        {
            RoomTypes.Default => _defaultRoomType,
            RoomTypes.Dummy => _dummyRoomType,

            _ => throw new ArgumentException($"{nameof(roomType)} went unhandled! ({roomType})")
        };

        public static bool GetAllowsPaths(this RoomTypes roomType) => roomType.ToObject().AllowsPaths;
        public static bool IsValidSpawn(this RoomTypes roomType) => roomType.ToObject().IsValidSpawn;
    }
}
