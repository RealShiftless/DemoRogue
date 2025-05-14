using DemoRogue.World.Rooms;
using Shiftless.Clockwork.Retro.Mathematics;

namespace DemoRogue.World.Building
{
    public sealed class ChunkBuilder
    {
        // Values
        public readonly DungeonBuilder Dungeon;
        public readonly Point8 GridPosition;

        private readonly bool _isSource;

        private Rect8? _roomBody = null;
        private RoomType? _roomType = null;

        private List<byte> _paths = [];
        private List<Point8> _pathRooms = [];

        private bool _leadsToSource = false;


        // Properties
        public bool ContainsRoom => _roomBody != null;

        public Rect8 RoomBody => _roomBody ?? throw new NullReferenceException($"{nameof(RoomBody)} was not set!");
        public RoomType RoomType => _roomType ?? throw new NullReferenceException($"{nameof(RoomType)} was not set!");

        public bool LeadsToSource => _isSource || _leadsToSource;


        // Constructor
        internal ChunkBuilder(DungeonBuilder dungeon, Point8 gridPosition, bool isSource)
        {
            Dungeon = dungeon;
            GridPosition = gridPosition;

            _isSource = isSource;
        }


        // Functions
        internal void SetRoom(Rect8 body, RoomType type)
        {
            _roomBody = body;
            _roomType = type;
        }
        internal void AddPath(byte pathIndex, Point8? destChunk = null, bool leadsToSource = false)
        {
            _paths.Add(pathIndex);
            if(destChunk != null)
                _pathRooms.Add(destChunk.Value);
            
            // If we are not already leading to the source (0,0) we set it to the leads to source bool given.
            // This way we can easily check if this chunk leads to the source
            if(leadsToSource)
                SetLeadsToSource();
        }

        private void SetLeadsToSource()
        {
            // Early return if we already lead to source
            if (_leadsToSource)
                return;

            // Set the leads to source value
            _leadsToSource = true;

            // And update all the rooms this chunk points to.
            foreach (Point8 destPos in _pathRooms)
                Dungeon.GetChunk(destPos).SetLeadsToSource();
        }


        internal Chunk Build()
        {
            Chunk chunk = new(Dungeon.Dungeon, RoomBody, RoomType, [.. _paths]);
            Reset();

            return chunk;
        }

        internal void Reset()
        {
            _leadsToSource = false;

            _roomBody = null;
            _roomType = null;
            _paths.Clear();
            _pathRooms.Clear();
        }

        /// <summary>
        /// This generates a random path point along a direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Point8 GeneratePathPoint(Direction direction) => direction switch
        {
            Direction.Up => new(RNG.Next(RoomBody.Left, RoomBody.Right), RoomBody.Top),
            Direction.Down => new(RNG.Next(RoomBody.Left, RoomBody.Right), RoomBody.Bottom),
            Direction.Left => new(RoomBody.Left, RNG.Next(RoomBody.Bottom, RoomBody.Top)),
            Direction.Right => new(RoomBody.Right, RNG.Next(RoomBody.Bottom, RoomBody.Top)),

            _ => throw new NotImplementedException($"Direction {direction} went unhandled!")
        };
    }
}
