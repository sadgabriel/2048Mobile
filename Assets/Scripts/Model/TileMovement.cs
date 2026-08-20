namespace Game2048.Model
{
    public readonly struct TileMovement
    {
        public int TileId { get; }
        public int FromX { get; }
        public int FromY { get; }
        public int ToX { get; }
        public int ToY { get; }
        public bool ConsumedByMerge { get; }

        public TileMovement(int tileId, int fromX, int fromY, int toX, int toY, bool consumedByMerge)
        {
            TileId = tileId;
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
            ConsumedByMerge = consumedByMerge;
        }
    }
}
