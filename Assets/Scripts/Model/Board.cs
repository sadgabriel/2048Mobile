using System.Collections.Generic;

namespace Game2048.Model
{
    public sealed class Board
    {
        public const int Size = 4;

        private readonly Tile[] _cells = new Tile[Size * Size];
        private readonly IRandomSource _random;
        private int _nextTileId = 1;

        public int Score { get; private set; }
        public bool HasWon { get; private set; }

        public Board(IRandomSource randomSource = null)
        {
            _random = randomSource ?? new SystemRandomSource();
        }

        public Tile GetTile(int x, int y) => _cells[y * Size + x];

        public IReadOnlyList<(int x, int y)> GetEmptyCells()
        {
            var result = new List<(int x, int y)>();
            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
                if (GetTile(x, y) == null)
                    result.Add((x, y));
            return result;
        }

        public bool SpawnRandomTile()
        {
            var emptyCells = GetEmptyCells();
            if (emptyCells.Count == 0)
                return false;

            var (x, y) = emptyCells[_random.Next(0, emptyCells.Count)];
            var value = _random.Next(0, 10) < 9 ? 2 : 4;
            SetCell(x, y, new Tile(value, NextTileId()));
            return true;
        }

        public MoveResult Move(Direction direction)
        {
            var moved = false;
            var scoreGained = 0;

            for (var lineIndex = 0; lineIndex < Size; lineIndex++)
            {
                var coords = GetLine(direction, lineIndex);
                var before = new Tile[Size];
                for (var i = 0; i < Size; i++)
                    before[i] = GetTile(coords[i].x, coords[i].y);

                var (after, lineScore) = ProcessLine(before);
                scoreGained += lineScore;

                for (var i = 0; i < Size; i++)
                {
                    if (!ReferenceEquals(before[i], after[i]))
                        moved = true;
                    SetCell(coords[i].x, coords[i].y, after[i]);
                }
            }

            if (moved)
                Score += scoreGained;

            UpdateWinState();

            return moved ? new MoveResult(true, scoreGained) : MoveResult.None;
        }

        public bool IsGameOver()
        {
            if (GetEmptyCells().Count > 0)
                return false;

            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
            {
                var value = GetTile(x, y).Value;
                if (x + 1 < Size && GetTile(x + 1, y).Value == value)
                    return false;
                if (y + 1 < Size && GetTile(x, y + 1).Value == value)
                    return false;
            }

            return true;
        }

        private (Tile[] result, int scoreGained) ProcessLine(Tile[] line)
        {
            var compacted = new List<Tile>(Size);
            foreach (var tile in line)
                if (tile != null)
                    compacted.Add(tile);

            var merged = new List<Tile>(Size);
            var scoreGained = 0;
            var i = 0;
            while (i < compacted.Count)
            {
                var current = compacted[i];
                // Advancing by 2 on a merge (rather than re-checking the merged
                // result) is what stops a run like [2,2,2] from cascading into [8].
                if (i + 1 < compacted.Count && compacted[i + 1].Value == current.Value)
                {
                    var mergedTile = new Tile(current.Value * 2, NextTileId());
                    merged.Add(mergedTile);
                    scoreGained += mergedTile.Value;
                    i += 2;
                }
                else
                {
                    merged.Add(current);
                    i += 1;
                }
            }

            var result = new Tile[Size];
            for (var idx = 0; idx < merged.Count; idx++)
                result[idx] = merged[idx];

            return (result, scoreGained);
        }

        private static (int x, int y)[] GetLine(Direction direction, int lineIndex)
        {
            switch (direction)
            {
                case Direction.Left:
                    return new[] { (0, lineIndex), (1, lineIndex), (2, lineIndex), (3, lineIndex) };
                case Direction.Right:
                    return new[] { (3, lineIndex), (2, lineIndex), (1, lineIndex), (0, lineIndex) };
                case Direction.Up:
                    return new[] { (lineIndex, 0), (lineIndex, 1), (lineIndex, 2), (lineIndex, 3) };
                case Direction.Down:
                    return new[] { (lineIndex, 3), (lineIndex, 2), (lineIndex, 1), (lineIndex, 0) };
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void UpdateWinState()
        {
            if (HasWon)
                return;

            for (var y = 0; y < Size; y++)
            for (var x = 0; x < Size; x++)
            {
                var tile = GetTile(x, y);
                if (tile != null && tile.Value >= 2048)
                {
                    HasWon = true;
                    return;
                }
            }
        }

        private void SetCell(int x, int y, Tile tile) => _cells[y * Size + x] = tile;

        private int NextTileId() => _nextTileId++;

        internal void Seed(int x, int y, int value) => SetCell(x, y, new Tile(value, NextTileId()));
    }
}
