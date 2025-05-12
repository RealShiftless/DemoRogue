using DemoRogue.Entities.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRogue.Entities
{
    public enum EntityTypes
    {
        Player = 0
    }

    public static class EntityTypeUtil
    {
        // Values
        private static readonly PlayerType _player = new();


        // Func
        public static IEntityType ToObject(this EntityTypes entityType) => entityType switch
        {
            EntityTypes.Player => _player,

            _ => throw new NotImplementedException($"Entity type {entityType} went unhandled!")
        };
    }
}
