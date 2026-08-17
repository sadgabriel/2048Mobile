namespace Game2048.Model
{
    public readonly struct MoveResult
    {
        public static readonly MoveResult None = new MoveResult(false, 0);

        public bool Moved { get; }
        public int ScoreGained { get; }

        public MoveResult(bool moved, int scoreGained)
        {
            Moved = moved;
            ScoreGained = scoreGained;
        }
    }
}
